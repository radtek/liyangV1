using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using WCAPP.Models.ProcessModels;
using System.Web.Mvc;
using WCAPP.Libs;
using WCAPP.Types;
using WCAPP.Utils;
using WCAPP.Models.Database;
using System.Web.Script.Serialization;

namespace WCAPP.Controllers
{
    public class SyncController : Controller
    {
        public ActionResult Index(int? pageNo, string searchType, string search, int? pageSize)
        {
            var db = new Context();
            int PageSize = pageSize ?? 10;
            List<int> lint = new List<int>();
            lint.Add(10);
            lint.Add(20);
            lint.Add(50);
            lint.Add(100);
            lint.Add(200);
            SelectList lints = new SelectList(lint, PageSize);
            ViewBag.PageSize = lints;
            ViewBag.PageSizes = PageSize;

            var processes = from p in db.Processes
                            group p by p.No
                           into g
                            select g.FirstOrDefault(x => x.Version == g.Max(y => y.Version));

            var process = processes.Include(x => x.Author).Where(x => x.ApprovalState == ApprovalState.审核通过);
            searchType = searchType ?? "";
            switch (searchType.ToLower())
            {
                case "partno":
                    process = process.Where(x => x.PartNo.Contains(search));
                   
                    break;
                case "author":
                    process = process.Where(x => x.Author.Id.Contains(search) || x.Author.Name.Contains(search));
                    
                    break;
                
                case "publish":
                    process = process.Where(x => (x.importState ? "true" : "false") == search);
                    break;
            }
            //到此时，creatings是所有满足查询条件的创建中规程，completes是所有满足查询条件的已完成规程
            int crtSize = process.Count();
          

            int crtNo = pageNo ?? 1;
           

            // 调整当前页码不超过总页数
            crtNo = Common.FixPageNo(crtNo, PageSize, crtSize);
            crtNo = crtNo <= 0 ? 1 : crtNo;
           
            var crtRet = process.OrderBy(x => x.No).Skip((crtNo - 1) * PageSize).Take(PageSize).ToList();
            

            List<Process> list = crtRet;
           
           
            int pageCount = (crtSize - 1) / PageSize + 1;
            
            int nextPageNo = crtNo >= pageCount ? pageCount : crtNo + 1;//计算下一页页号
            int prevPageNo = crtNo == 1 ? 1 : crtNo - 1;//计算上一页页号         

            ViewBag.NextPageNo = nextPageNo;
            ViewBag.PrevPageNo = prevPageNo;
            ViewBag.PageCount = pageCount;
            ViewBag.PageNo = crtNo;

            ViewBag.SearchType = searchType;
            ViewBag.Search = search;
            ViewBag.Publish = "publish";           
            ViewBag.Author = "author";
            ViewBag.PartNo = "partno";
          
            List<int> listPage = new List<int>();
            for (int i = 1; i <= pageCount; i++)
            {
                listPage.Add(i);
            }
           
            SelectList li = new SelectList(listPage, crtNo);
           
            ViewBag.PageList = li;
            ViewBag.list = list;
            return View();
        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "ExcelFiles");
            if (!dirInfo.Exists)
                dirInfo.Create();

            var fileName = AppDomain.CurrentDomain.BaseDirectory + "ExcelFiles/" + Guid.NewGuid() + ".xls";

            var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            file.InputStream.CopyTo(fs);
            fs.Close();

            try
            {
                var processes = WcappExcel.Import(fileName);
                //如果数据库有这条工艺规程
                var db = new Context();
                foreach (var process in processes)
                {
                    if (process.No==""||process.No==null)
                    {
                        continue;
                    }
                    var userId = Session.GetSessionUser().Id;
                    var user = db.Users.SingleOrDefault(x => x.Id == userId);//InitialParams
                                       
                    var procedbs = db.Processes.Include("Procedures.Seams.InitialParams").Where(x => x.No == process.No).ToList();

                    Log.Info(procedbs.Count.ToString());

                    if (procedbs.Count == 0)//数据库里没有这条数据，直接导入
                    {
                        process.Author = user;//当前编制者
                        process.Establish = true;//标记为Excel导入
                        process.excelImportTime = DateTime.Now;
                        var tasks = db.DispatchMessages.ToList();
                        foreach (var task in tasks)
                        {
                            if (task.PartNo == process.PartNo)
                            {
                                task.showState = false;
                            }
                        }                        
                        db.Processes.Add(process);
                        db.SaveChanges();                        
                    }
                    else
                    {
                        if (procedbs.Count > 1)
                        {
                            return Json(new { succeed = false, datas = process.No, error = "此条工艺规程已存在修订版本，请勿重复导入！" }, "text/html");
                                                       
                        }
                        else
                        {
                            foreach (var procedb in procedbs)
                            {
                                if (!procedb.Establish)//数据库有此条工艺规程，再判断是否为导入还是手动
                                {
                                    //手动创建的工艺规程编号和导入工艺规程编号一样
                                    return Json(new { succeed = false, datas = process.No, error = "手动创建的工艺规程编号和导入工艺规程编号一样，不可修改参数！" }, "text/html");
                                }
                                // procedb.Author = user;
                                if (procedb.Published)
                                {
                                    return Json(new { succeed = false, datas = process.No, error = "该工艺已发布！" }, "text/html");
                                }
                                procedb.Published = process.Published;//发布
                                if (procedb.TestState == ProgramTestState.已完成)
                                {
                                    return Json(new { succeed = false, datas = process.No, error = "该工艺焊缝试焊状态已完成！" }, "text/html");
                                }
                                procedb.TestState = process.TestState;//试焊
                                foreach (var procedures in process.Procedures)
                                {
                                    var proceduredb = procedb.Procedures.SingleOrDefault(x => x.No == procedures.No);
                                    if (proceduredb == null)
                                    {
                                        return Json(new { succeed = false, datas = process.No, error = "工序号不可修改！" }, "text/html");
                                    }
                                    if (proceduredb.TestState == ProgramTestState.已完成)
                                    {
                                        return Json(new { succeed = false, datas = process.No, error = "该工艺焊缝试焊状态已完成！" }, "text/html");
                                    }
                                    proceduredb.TestState = procedures.TestState;
                                    foreach (var seam in procedures.Seams)
                                    {
                                        var seamdb = proceduredb.Seams.SingleOrDefault(x => x.No == seam.No);
                                        if (seamdb == null)
                                        {
                                            return Json(new { succeed = false, datas = process.No, error = "焊缝号不可修改！" }, "text/html");
                                        }
                                        if (seamdb.TestState == TestState.已完成)
                                        {
                                            return Json(new { succeed = false, datas = process.No, error = "该工艺焊缝试焊状态已完成！" }, "text/html");
                                        }
                                        seamdb.TestState = seam.TestState;
                                        if (!seamdb.InitialParams.Any())
                                        {
                                            return Json(new { succeed = false, datas = process.No, error = "该工艺焊缝无信息！" }, "text/html");
                                        }
                                        if (seamdb.InitialParams.Any())
                                        {
                                            foreach (var param in seam.InitialParams)
                                            {
                                                foreach (var smdb in seamdb.InitialParams)
                                                {
                                                    if (smdb.Enum == param.Enum)
                                                    {
                                                        smdb.Value = param.Value;
                                                    }
                                                }
                                            }
                                        }

                                    }

                                }
                                var tasks = db.DispatchMessages.ToList();
                                foreach (var task in tasks)
                                {
                                    if (task.PartNo == process.PartNo)
                                    {
                                        task.showState = true;
                                    }
                                }
                                procedb.excelImportTime = DateTime.Now;
                                procedb.Establish = true;//将导入的工艺规程编号字段Establish改为true
                                db.SaveChanges();                               
                            }
                        }
                    }                                     
                    
                }
                return Json(new { succeed = true, message = "导入成功！" }, "text/html");
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false, error = e.Message }, "text/html");
            }            
        }

        [HttpPost]
        public ActionResult CheckWcappFile(HttpPostedFileBase file)
        {
            if (!WcappFile.IsFileCorrect(file.InputStream))
                return Json(new { succeed = false, correct = false }, "text/html");

            return Json(new { succeed = true, correct = true }, "text/html");
        }
       
        [HttpPost]
        public ActionResult ImportWcapp(HttpPostedFileBase file)
        {
            try
            {
                var processes = new WcappFile(file.InputStream).ToProcesses();
                
                var db = new Context();

                List<string> existProcesses = new List<string>();

                // TODO 将processes更新到数据库
                //IsDeviceNet为true设备网
                if (GlobalData.IsDeviceNet)
                {
                    foreach (var proce in processes)
                    {
                        var process = db.Processes.SingleOrDefault(x => x.No == proce.No && x.Version == proce.Version);                      

                        if (process != null)
                        {
                            //相同编号工艺规程编号的同一版本不能重复导入
                            existProcesses.Add($"{process.No}({process.VersionString})");
                            continue;
                        }
                        if (proce.Author==null)
                        {
                            return Json(new { succeed = false, error = "请指定导入的Wcapp文件编制者" }, "text/html");
                        }
                        
                        var author = db.Users.Find(proce.Author.Id);                      
                        

                        process = new Process {
                            PdmId = proce.No,
                            No = proce.No,
                            PartNo = proce.PartNo,
                            PartName = proce.PartName,
                            Author = author,
                            ApprovalState = proce.ApprovalState,
                            TestState = proce.TestState,
                            Version = proce.Version,
                            Procedures = new List<Procedure>()
                        };
                        foreach (var procedure in proce.Procedures)
                        {
                            var proc = new Procedure {
                                PdmId = procedure.No,
                                No = procedure.No,
                                Name = procedure.Name,
                                WeldMethod = procedure.WeldMethod,
                                WeldType = procedure.WeldType,
                                ResistType = procedure.ResistType,
                                AutoLevel = procedure.AutoLevel,
                                TestState = procedure.TestState,
                                Seams = new List<Seam>()
                            };
                            foreach (var seam in procedure.Seams)
                            {
                                var ns = new Seam {
                                    No = seam.No,
                                    TestState = seam.TestState,
                                    JointForm = seam.JointForm,
                                    SeamLevel = seam.SeamLevel,
                                    CheckStandard = seam.CheckStandard,
                                    Material1 = seam.Material1,
                                    Material2 = seam.Material2,
                                    Material3 = seam.Material3,
                                    Thick1 = seam.Thick1,
                                    Thick2 = seam.Thick2,
                                    Thick3 = seam.Thick3,
                                    Gap = seam.Gap,
                                    InitialParams = new List<SeamParam1>(),
                                    RevisedParams = new List<SeamParam2>()
                                };
                                foreach (var param in seam.InitialParams)
                                {
                                    var npm = new SeamParam1 {
                                        Enum = param.Enum,
                                        Value = param.Value
                                    };
                                    ns.InitialParams.Add(npm);
                                }

                                foreach (var param in seam.RevisedParams)
                                {
                                    var npm = new SeamParam2 {
                                        Enum = param.Enum,
                                        Value = param.Value
                                    };

                                    ns.RevisedParams.Add(npm);
                                }

                                proc.Seams.Add(ns);
                            }

                            process.Procedures.Add(proc);
                        }

                        db.Processes.Add(process);
                        db.SaveChanges();
                    }
                }
                else    // 导入到内网
                {
                    foreach (var proce in processes)
                    {
                        var processDb = db.Processes.Include("Procedures.Seams.RevisedParams").SingleOrDefault(x => x.No == proce.No && x.Version == proce.Version);
                        //var processDbs = db.Processes.SingleOrDefault(x => x.No == proce.No && x.Version == proce.Version);
                        if (processDb == null)
                        {
                            return Json(new { succeed = false, error = "零件编号的工艺规程不存在" }, "text/html");
                        }

                        if (!proce.Published)
                        {
                            return Json(new { succeed = false, error = "状态未发布" }, "text/html");
                        }

                        processDb.Published = proce.Published;

                        if (proce.TestState != ProgramTestState.已完成)
                        {
                            return Json(new { succeed = false, error = "试焊状态未完成" }, "text/html");
                        }

                        processDb.TestState = proce.TestState;

                        foreach (var procedure in proce.Procedures)
                        {
                            var prodedureDb = processDb.Procedures.SingleOrDefault(x => x.No == procedure.No);
                            if (prodedureDb.TestState != ProgramTestState.已完成)
                            {
                                return Json(new { succeed = false, error = "试焊状态未完成" }, "text/html");
                            }

                            prodedureDb.TestState = procedure.TestState;

                            foreach (var seam in procedure.Seams)
                            {
                                var seamDb = prodedureDb.Seams.SingleOrDefault(x => x.No == procedure.No);
                                if (seamDb.TestState != TestState.已完成)
                                {
                                    return Json(new { succeed = false, error = "试焊状态未完成" }, "text/html");
                                }

                                seamDb.TestState = seam.TestState;

                                if (seamDb.RevisedParams.Any())
                                {
                                    return Json(new { succeed = false, error = "试焊状态未完成" }, "text/html");
                                }

                                foreach (var param in seam.RevisedParams)
                                {
                                    seamDb.RevisedParams.Add(new Models.Database.SeamParam2 {
                                        Enum = param.Enum,
                                        Value = param.Value
                                    });
                                }
                            }
                        }

                        db.SaveChanges();
                    }
                }

                if (existProcesses.Any())
                    return Json(new { succeed = true, existProcesses }, "text/html");
                else
                    return Json(new { succeed = true }, "text/html");
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false, error = e.Message }, "text/html");
            }
        }

        [HttpPost]
        public ActionResult ExportWcapp(int[] processIds)
        {
            try
            {
                var userId = Session.GetSessionUser().Id;
                var db = new Context();
                var User = db.Users.SingleOrDefault(x => x.Id == userId);
                if (processIds != null)
                {
                    var processes = db.Processes
                    .Include(x => x.Author)
                    .Include("Procedures.Seams.InitialParams")
                    .Include("Procedures.Seams.RevisedParams")
                    .Where(x => processIds.Contains(x.Id))
                    .ToArray();

                    var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "WcappFiles");
                    if (!dirInfo.Exists)
                        dirInfo.Create();

                    var fileName = Guid.NewGuid() + ".wcapp";
                    var name = "WcappFiles/" + fileName;
                    new WcappFile(processes).SaveAs(AppDomain.CurrentDomain.BaseDirectory + name);

                    foreach (var process in processes)
                    {
                        if (process.Author == null)
                        {
                            process.Author = User;
                        }
                        process.importState = true;
                        process.importTime = DateTime.Now;
                        db.SaveChanges();
                    }

                    return Json(new { succeed = true, path = name, name = fileName });
                }
                else
                {
                    return Json(new { succeed = false });
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false });
            }
            

        }      
    }
}