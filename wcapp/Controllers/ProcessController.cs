using org.in2bits.MyXls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WCAPP.Libs;
using WCAPP.Models.Database;
using WCAPP.Models.ProcessModels;
using WCAPP.Types;
using WCAPP.Utils;

namespace WCAPP.Controllers
{
    public class ProcessController : Controller
    {
        class PdmInput
        {
            public string processPdmId;
            public string processNo;
            public string processName;
            public string partNo;
            public string partName;
            public ProcSeq[] seqs;
            public DateTime time = DateTime.Now;
        }

        // const int PageSize = 10;

        static Dictionary<string, PdmInput> pdmUrls = new Dictionary<string, PdmInput>();

        public ActionResult Index(int? crtPageNo, int? copPageNo, string searchType, string search, int? pageSize)
        {
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

            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var processes = from p in db.Processes
                            group p by p.No
                into g
                            select g.FirstOrDefault(x => x.Version == g.Max(y => y.Version));

            var creatings = processes.Include(x => x.Author).Where(x => x.ApprovalState != ApprovalState.审核通过);
            var completes = processes.Include(x => x.Author).Where(x => x.ApprovalState == ApprovalState.审核通过);

            searchType = searchType ?? "";


            switch (searchType.ToLower())
            {
                case "partno":
                    creatings = creatings.Where(x => x.PartNo.Contains(search) || x.No.Contains(search));
                    completes = completes.Where(x => x.PartNo.Contains(search) || x.No.Contains(search));
                    break;
                case "author":
                    creatings = creatings.Where(x => x.Author.Id.Contains(search) || x.Author.Name.Contains(search));
                    completes = completes.Where(x => x.Author.Id.Contains(search) || x.Author.Name.Contains(search));
                    break;
                case "test":
                    if (search != null)
                    {
                        completes = completes.Where(x => x.TestState.ToString() == search);
                    }

                    break;
                case "publish":
                    completes = completes.Where(x => (x.Published ? "true" : "false") == search);
                    break;
            }

            //到此时，creatings是所有满足查询条件的创建中规程，completes是所有满足查询条件的已完成规程
            int crtSize = creatings.Count();
            int copSize = completes.Count();

            int crtNo = crtPageNo ?? 1;
            int copNo = copPageNo ?? 1;

            // 调整当前页码不超过总页数
            crtNo = Common.FixPageNo(crtNo, PageSize, crtSize);
            crtNo = crtNo <= 0 ? 1 : crtNo;
            copNo = Common.FixPageNo(copNo, PageSize, copSize);
            copNo = copNo <= 0 ? 1 : copNo;

            var crtRet = creatings.OrderBy(x => x.No).Skip((crtNo - 1) * PageSize).Take(PageSize).ToList();
            var copRet = completes.OrderBy(x => x.No).Skip((copNo - 1) * PageSize).Take(PageSize).ToList();


            List<Process> list = copRet;
            List<Process> list2 = crtRet;
            int pageCount = (copSize - 1) / PageSize + 1;//找出总页数
            int pageCount2 = (crtSize - 1) / PageSize + 1;
            int nextPageNo = copNo >= pageCount ? pageCount : copNo + 1;//计算下一页页号
            int prevPageNo = copNo == 1 ? 1 : copNo - 1;//计算上一页页号
            int nextPageNo2 = crtNo >= pageCount2 ? pageCount2 : crtNo + 1;//计算下一页页号
            int prevPageNo2 = crtNo == 1 ? 1 : crtNo - 1;//计算上一页页号
            //使用viewbag带到视图去
            ViewBag.NextPageNo = nextPageNo;//下一页
            ViewBag.PrevPageNo = prevPageNo;//上一页
            ViewBag.PageCount = pageCount;//总页数
            ViewBag.PageNo = copNo;//当前页号

            ViewBag.NextPageNo2 = nextPageNo2;
            ViewBag.PrevPageNo2 = prevPageNo2;
            ViewBag.PageCount2 = pageCount2;
            ViewBag.PageNo2 = crtNo;

            ViewBag.SearchType = searchType;
            ViewBag.Search = search;
            ViewBag.Publish = "publish";
            ViewBag.Test = "test";
            ViewBag.Author = "author";
            ViewBag.PartNo = "partno";
            //下拉列表显示页数需要的selectlist数据
            List<int> listPage = new List<int>();
            for (int i = 1; i <= pageCount; i++)
            {
                listPage.Add(i);
            }
            List<int> listPage2 = new List<int>();
            for (int i = 1; i <= pageCount2; i++)
            {
                listPage2.Add(i);
            }
            SelectList li = new SelectList(listPage, copNo);
            SelectList li2 = new SelectList(listPage2, crtNo);
            ViewBag.PageList = li;
            ViewBag.PageList2 = li2;
            ViewBag.list = list;
            ViewBag.list2 = list2;
            return View();
        }
        public ActionResult ReviseIndex(int? crtPageNo, int? copPageNo, string searchType, string search, int? pageSize)
        {
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

            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var processes = from p in db.Processes
                            group p by p.No
                into g
                            select g.FirstOrDefault(x => x.Version == g.Max(y => y.Version));

            var creatings = processes.Include(x => x.Author).Where(x => x.ApprovalState != ApprovalState.审核通过);
            var completes = processes.Include(x => x.Author).Where(x => x.ApprovalState == ApprovalState.审核通过);

            searchType = searchType ?? "";


            switch (searchType.ToLower())
            {
                case "partno":
                    creatings = creatings.Where(x => x.PartNo.Contains(search));
                    completes = completes.Where(x => x.PartNo.Contains(search));
                    break;
                case "author":
                    creatings = creatings.Where(x => x.Author.Id.Contains(search) || x.Author.Name.Contains(search));
                    completes = completes.Where(x => x.Author.Id.Contains(search) || x.Author.Name.Contains(search));
                    break;
                case "test":
                    if (search != null)
                    {
                        completes = completes.Where(x => x.TestState.ToString() == search);
                    }

                    break;
                case "publish":
                    completes = completes.Where(x => (x.Published ? "true" : "false") == search);
                    break;
            }

            //到此时，creatings是所有满足查询条件的创建中规程，completes是所有满足查询条件的已完成规程
            int crtSize = creatings.Count();
            int copSize = completes.Count();

            int crtNo = crtPageNo ?? 1;
            int copNo = copPageNo ?? 1;

            // 调整当前页码不超过总页数
            crtNo = Common.FixPageNo(crtNo, PageSize, crtSize);
            crtNo = crtNo <= 0 ? 1 : crtNo;
            copNo = Common.FixPageNo(copNo, PageSize, copSize);
            copNo = copNo <= 0 ? 1 : copNo;

            var crtRet = creatings.OrderBy(x => x.No).Skip((crtNo - 1) * PageSize).Take(PageSize).ToList();
            var copRet = completes.OrderBy(x => x.No).Skip((copNo - 1) * PageSize).Take(PageSize).ToList();


            List<Process> list = copRet;
            List<Process> list2 = crtRet;
            int pageCount = (copSize - 1) / PageSize + 1;//找出总页数
            int pageCount2 = (crtSize - 1) / PageSize + 1;
            int nextPageNo = copNo >= pageCount ? pageCount : copNo + 1;//计算下一页页号
            int prevPageNo = copNo == 1 ? 1 : copNo - 1;//计算上一页页号
            int nextPageNo2 = crtNo >= pageCount2 ? pageCount2 : crtNo + 1;//计算下一页页号
            int prevPageNo2 = crtNo == 1 ? 1 : crtNo - 1;//计算上一页页号
            //使用viewbag带到视图去
            ViewBag.NextPageNo = nextPageNo;//下一页
            ViewBag.PrevPageNo = prevPageNo;//上一页
            ViewBag.PageCount = pageCount;//总页数
            ViewBag.PageNo = copNo;//当前页号

            ViewBag.NextPageNo2 = nextPageNo2;
            ViewBag.PrevPageNo2 = prevPageNo2;
            ViewBag.PageCount2 = pageCount2;
            ViewBag.PageNo2 = crtNo;

            ViewBag.SearchType = searchType;
            ViewBag.Search = search;
            ViewBag.Publish = "publish";
            ViewBag.Test = "test";
            ViewBag.Author = "author";
            ViewBag.PartNo = "partno";
            //下拉列表显示页数需要的selectlist数据
            List<int> listPage = new List<int>();
            for (int i = 1; i <= pageCount; i++)
            {
                listPage.Add(i);
            }
            List<int> listPage2 = new List<int>();
            for (int i = 1; i <= pageCount2; i++)
            {
                listPage2.Add(i);
            }
            SelectList li = new SelectList(listPage, copNo);
            SelectList li2 = new SelectList(listPage2, crtNo);
            ViewBag.PageList = li;
            ViewBag.PageList2 = li2;
            ViewBag.list = list;
            ViewBag.list2 = list2;
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var db = new Context();
            var process = db.Processes.SingleOrDefault(x => x.No == model.No);

            if (process != null)
            {
                ModelState.AddModelError("ExistNo", "相同编号的工艺规程已经存在!");
                return View();
            }

            var author = db.Users.Find(Session.GetSessionUser().Id);
            process = new Process
            {
                No = model.No,
                PartNo = model.PartNo,
                PartName = model.PartName,
                Author = author,
                ApprovalState = ApprovalState.未提交审核,
                TestState = ProgramTestState.未完成,
                Version = 1
            };
            var tasks = db.DispatchMessages.ToList();
            foreach (var task in tasks)
            {
                if (task.PartNo == model.PartNo)
                {
                    task.showState = false;
                }
            }
            db.Processes.Add(process);
            db.SaveChanges();
            return RedirectToAction("Creating", new { id = process.Id });
        }

        public ActionResult Detail(int id)
        {
            ViewBag.Title = "Home Page";
            var db = new Context();
            var process = db.Processes.Include(x => x.Author).Include(x => x.Procedures)
                .SingleOrDefault(x => x.Id == id);

            ViewBag.Process = process;
            ViewBag.HistoryProcesses = db.Processes.Where(x => x.No == process.No && x.Version < process.Version)
                .OrderByDescending(x => x.Version);
            return View();
        }

        public ActionResult Creating(int id)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;
            var process = db.Processes.Include(x => x.Author).Include(x => x.Procedures)
                .SingleOrDefault(x => x.Author.Id == userId && x.Id == id && x.ApprovalState != ApprovalState.审核通过);

            ViewBag.Process = process;
            ViewBag.Editable = process != null && (process.ApprovalState == ApprovalState.审核不通过 ||
                                                   process.ApprovalState == ApprovalState.未提交审核);
            ViewBag.BackEdit = process != null && (process.ApprovalState == ApprovalState.审核中);
            ViewBag.HistoryProcesses = db.Processes.Where(x => x.No == process.No && x.Version < process.Version)
                .OrderByDescending(x => x.Version);

            return View();
        }//ReviseCreating
        public ActionResult ReviseCreating(int id)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;
            var process = db.Processes.Include(x => x.Author).Include(x => x.Procedures)
                .SingleOrDefault(x => x.Author.Id == userId && x.Id == id && x.ApprovalState != ApprovalState.审核通过);

            ViewBag.Process = process;
            ViewBag.Editable = process != null && (process.ApprovalState == ApprovalState.审核不通过 ||
                                                   process.ApprovalState == ApprovalState.未提交审核);

            ViewBag.HistoryProcesses = db.Processes.Where(x => x.No == process.No && x.Version < process.Version)
                .OrderByDescending(x => x.Version);

            return View();
        }
        public ActionResult AddProcedure()
        {
            return PartialView("_AddProcedure");
        }

        [HttpPost]
        public ActionResult AddProcedure(AddProcedureModel model, int pid)
        {
            ViewBag.ProcessId = pid;

            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var process = db.Processes.Include(x => x.Procedures)
                .SingleOrDefault(x => x.Author.Id == userId && x.Id == pid);

            if (process == null || !ModelState.IsValid)
                return AddProcedure();

            if (model.WeldMethod == WeldMethod.氩弧焊 && model.AutoLevel == null)
            {
                ModelState.AddModelError("AutoLevel", "AutoLevel Required");
                return AddProcedure();
            }

            if (model.WeldMethod == WeldMethod.电阻焊 && model.ResistType == null)
            {
                ModelState.AddModelError("ResistType", "WeldWay Required");
                return AddProcedure();
            }

            if (process.ApprovalState == ApprovalState.审核中 || process.ApprovalState == ApprovalState.审核通过)
            {
                ModelState.AddModelError("Error", "工艺规程处于审核中或者已经审核通过");
                return AddProcedure();
            }

            if (process.Procedures.Exists(x => x.No == model.No))
            {
                ModelState.AddModelError("Error", "相同的工序号已经存在");
                return AddProcedure();
            }

            var proc = new Procedure
            {
                No = model.No,
                Name = model.Name,
                WeldMethod = model.WeldMethod,
                WeldType = model.WeldType,
                ResistType = model.ResistType,
                AutoLevel = model.AutoLevel,
                TestState = ProgramTestState.未完成
            };
            process.Procedures.Add(proc);
            db.SaveChanges();
            return Json(new { succeed = true });
        }
        [HttpPost]
        public ActionResult DeleteProcess(int id)
        {
            try
            {
                var db = new Context();
                var userId = Session.GetSessionUser().Id;
                var process = db.Processes.SingleOrDefault(x => x.Author.Id == userId && x.Id == id);
                if (process != null)
                {
                    if (process.Procedures != null)
                    {
                        foreach (var procedures in process.Procedures)
                        {
                            var procedure = db.Procedures.SingleOrDefault(x => x.Id == procedures.Id);
                            if (procedure != null)
                            {
                                if (procedure.Seams != null)
                                {
                                    foreach (var seams in procedure.Seams)
                                    {
                                        var seam = db.Seams.SingleOrDefault(x => x.Id == seams.Id);
                                        if (seam != null)
                                        {
                                            db.Seams.Remove(seam);
                                        }
                                    }
                                }
                                db.Procedures.Remove(procedure);
                            }
                        }
                    }
                    db.Processes.Remove(process);
                    db.SaveChanges();
                    return Json(new { succeed = true });
                }
                else
                {
                    return Json(new { succeed = false, error = "该指定工艺规程不是当前用户所创建！" });
                }

            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false, error = e.Message });
            }
        }
        [HttpPost]
        public ActionResult DeleteProcedure(int id, string proNo)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var process = db.Processes.SingleOrDefault(x => x.Author.Id == userId && x.Id == id);

            if (process == null)
            {
                return Json(new { succeed = false, error = "当前用户不拥有指定工艺规程" });
            }

            var procedure = db.Procedures.SingleOrDefault(x => x.No == proNo && x.ProcessId == id);

            if (procedure == null)
            {
                return Json(new { succeed = false, error = "指定工序不存在" });
            }

            if (process.ApprovalState == ApprovalState.审核中 || process.ApprovalState == ApprovalState.审核通过)
            {
                return Json(new { succeed = false, error = "工艺规程处于审核中或者已经审核通过" });
            }

            db.Procedures.Remove(procedure);
            db.SaveChanges();

            return Json(new { succeed = true });
        }

        public ActionResult HistoryDetail(int id, string ver)
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpPost]
        public ActionResult Publish(int id)
        {
            var db = new Context();
            var process = db.Processes.Find(id);
            if (process == null)
                return Json(new { error = "指定工艺规程不存在" });
            process.Published = true;
            db.SaveChanges();
            return Json(new { succeed = true });
        }

        [HttpPost]
        public ActionResult Revise(int id)
        {
            try
            {
                var db = new Context();
                var process = db.Processes
                    .Include("Procedures.Seams.InitialParams")
                    .Include("Procedures.Seams.RevisedParams")
                    .SingleOrDefault(x => x.Id == id);

                if (process == null)
                    return Json(new { error = "指定工艺规程不存在" });

                if (process.ApprovalState != ApprovalState.审核通过)
                    return Json(new { error = "指定工艺规程没有审核通过，不能修订" });

                var maxVersion = db.Processes.Where(x => x.No == process.No).Select(x => x.Version).Max();
                if (process.Version + 1 == maxVersion)
                {
                    var newest = db.Processes.SingleOrDefault(x => x.No == process.No && x.Version == maxVersion);
                    if (newest?.ApprovalState != ApprovalState.审核通过)
                        return Json(new { error = "指定工艺规程正在修订中" });
                }

                if (process.Version != maxVersion)
                    return Json(new { error = "指定工艺规程不是最新版本，不能修订" });

                var author = db.Users.Find(Session.GetSessionUser().Id);
                var np = new Process
                {
                    No = process.No,
                    PdmId = process.PdmId,
                    PartNo = process.PartNo,
                    PartName = process.PartName,
                    Author = author,
                    ApprovalState = ApprovalState.未提交审核,
                    TestState = ProgramTestState.未完成,
                    Version = process.Version + 1,
                    Procedures = new List<Procedure>()
                };

                foreach (var procedure in process.Procedures)
                {
                    var npro = new Procedure
                    {
                        No = procedure.No,
                        PdmId = procedure.PdmId,
                        Name = procedure.Name,
                        WeldMethod = procedure.WeldMethod,
                        WeldType = procedure.WeldType,
                        ResistType = procedure.ResistType,
                        AutoLevel = procedure.AutoLevel,
                        TestState = ProgramTestState.未完成,
                        Seams = new List<Seam>()
                    };

                    foreach (var seam in procedure.Seams)
                    {
                        var ns = new Seam
                        {
                            No = seam.No,
                            TestState = TestState.尚未进行,
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
                            var npm = new SeamParam1
                            {
                                Enum = param.Enum,
                                Value = param.Value
                            };
                            ns.InitialParams.Add(npm);
                        }

                        foreach (var param in seam.RevisedParams)
                        {
                            var npm = new SeamParam2
                            {
                                Enum = param.Enum,
                                Value = param.Value
                            };
                            ns.RevisedParams.Add(npm);
                        }

                        npro.Seams.Add(ns);
                    }

                    np.Procedures.Add(npro);
                }

                db.Processes.Add(np);
                db.SaveChanges();
                return Json(new { succeed = true, pid = np.Id });
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false });
            }

        }

        [HttpPost]
        public string PdmCreateDetailUrl(string json)
        {
            Log.Info($"PdmCreateDetailUrl({json})");
            var key = Guid.NewGuid().ToString();

            try
            {
                var input = new JavaScriptSerializer().Deserialize<PdmInput>(json);
                lock (pdmUrls)
                {
                    pdmUrls.Add(key, input);

                    var deletes = new List<string>();
                    foreach (var paire in pdmUrls)
                    {
                        if ((DateTime.Now - paire.Value.time).TotalHours > 5)
                        {
                            deletes.Add(paire.Key);
                        }
                    }
                    foreach (var del in deletes)
                    {
                        pdmUrls.Remove(del);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
            return key;
        }

        public ActionResult PdmDetail(string key)
        {
            PdmInput input = null;
            lock (pdmUrls)
            {
                pdmUrls.TryGetValue(key, out input);
                if (input == null)
                {
                    return View("PdmCreate");
                }
            }

            string processPdmId = input.processPdmId;
            string processNo = input.processNo;
            string processName = input.processName;
            string partNo = input.partNo;
            string partName = input.partName;
            var procedures = input.seqs;

            Log.Info($"PdmDetail(processPdmId:{processPdmId},processNo:{processNo},processName:{processName},partNo:{partNo},partName:{partName},seqs:{new JavaScriptSerializer().Serialize(procedures)})");


            if (procedures == null)
                procedures = new ProcSeq[0];

            var db = new Context();
            var process = db.Processes
                .Include(x => x.Author)
                .Include(x => x.Procedures)
                .Where(x => x.PdmId == processPdmId)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();
            if (process != null)
            {
                var proceduredb = db.Procedures.SingleOrDefault(x => x.Id == process.Id);
            }


            if (process == null)
            {
                process = db.Processes
                  .Include(x => x.Author)
                  .Include(x => x.Procedures)
                  .Where(x => x.No == processNo && (x.PdmId == null || x.PdmId == ""))
                  .OrderByDescending(x => x.Version)
                  .FirstOrDefault();
            }

            if (process != null)
            {
                process.PdmId = processPdmId;
                process.Procedures.Sort((x, y) => x.No.CompareTo(y.No));

                var dbProcs = process.Procedures;
                var procs = procedures.ToList();
                procs.Sort((x, y) => x.no.CompareTo(y.no));

                if (dbProcs.Count != procs.Count)
                {
                    ViewBag.UpVersion = true;
                }
                else
                {
                    for (int i = 0; i < dbProcs.Count; i++)
                    {
                        var dbproce = dbProcs[i];
                        var proce = procs[i];
                        if (dbproce.No != proce.no)
                        {
                            ViewBag.UpVersion = true;
                            break;
                        }
                        if (!dbproce.PdmId.IsEmpty() && dbproce.PdmId != proce.id)
                        {
                            ViewBag.UpVersion = true;
                            break;
                        }
                    }

                    if (ViewBag.UpVersion != true)
                    {
                        for (int i = 0; i < dbProcs.Count; i++)
                        {
                            var dbproce = dbProcs[i];
                            var proce = procs[i];
                            dbproce.PdmId = proce.id;
                        }
                    }
                }
                db.SaveChanges();
            }


            if (process != null && ViewBag.UpVersion != true)
                return RedirectToAction(process.ApprovalState == ApprovalState.审核通过 ? "Detail" : "Creating",
                    new { id = process.Id });

            var model = new PdmCreateModel
            {
                PdmId = processPdmId,
                No = processNo,
                PartNo = partNo,
                PartName = partName,
                Procedures = new List<PdmAddProcedureModel>()
            };

            foreach (var procedure in procedures)
            {
                if (process != null)
                {
                    foreach (var prodb in process.Procedures)
                    {
                        List<WeldMethod> listpage = new List<WeldMethod>();
                        List<WeldType> listpage2 = new List<WeldType>();
                        List<ResistType> listpage3 = new List<ResistType>();
                        List<AutoLevel> listpage4 = new List<AutoLevel>();
                        foreach (WeldMethod m in Enum.GetValues(typeof(WeldMethod)))
                        {
                            listpage.Add(m);
                        }
                        foreach (WeldType m in Enum.GetValues(typeof(WeldType)))
                        {
                            listpage2.Add(m);
                        }
                        foreach (ResistType m in Enum.GetValues(typeof(ResistType)))
                        {
                            listpage3.Add(m);
                        }
                        foreach (AutoLevel m in Enum.GetValues(typeof(AutoLevel)))
                        {
                            listpage4.Add(m);
                        }
                        SelectList li = new SelectList(listpage, prodb.WeldMethod);
                        SelectList li2 = new SelectList(listpage2, prodb.WeldType);
                        if (prodb.ResistType != null)
                        {
                            SelectList li3 = new SelectList(listpage3, prodb.ResistType);
                            ViewBag.ResistTypes = li3;
                        }


                        ViewBag.WeldMethods = li;
                        ViewBag.WeldTypes = li2;
                        if (prodb.AutoLevel != null)
                        {
                            SelectList li4 = new SelectList(listpage4, prodb.AutoLevel);
                            ViewBag.AutoLevels = li4;
                        }

                    }
                }

                model.Procedures.Add(new PdmAddProcedureModel
                {
                    PdmId = procedure.id,
                    No = procedure.no,
                    Name = procedure.name
                });
            }
            return View("PdmCreate", model);
        }

        [HttpPost]
        public ActionResult PdmCreate(string json)
        {
            try
            {
                var db = new Context();
                var model = new JavaScriptSerializer().Deserialize<PdmCreateModel>(json);

                var process = db.Processes.SingleOrDefault(x => x.No == model.No);

                if (process != null)
                {
                    throw new Exception("相同编号的工艺规程已经存在!");
                }

                var author = db.Users.Find(Session.GetSessionUser().Id);
                process = new Process
                {
                    PdmId = model.PdmId,
                    No = model.No,
                    PartNo = model.PartNo,
                    PartName = model.PartName,
                    Author = author,
                    ApprovalState = ApprovalState.未提交审核,
                    TestState = ProgramTestState.未完成,
                    Version = 1,
                    Procedures = new List<Procedure>()
                };

                foreach (var procedure in model.Procedures)
                {
                    if (!procedure.WeldMethod.HasValue)
                        throw new Exception("请输入焊接方法");

                    if (!procedure.WeldType.HasValue)
                        throw new Exception("请输入焊接类型");

                    if (procedure.WeldMethod == WeldMethod.氩弧焊)
                    {
                        if (!procedure.AutoLevel.HasValue)
                            throw new Exception("请输入自动化程度");
                    }

                    if (procedure.WeldMethod == WeldMethod.电阻焊)
                    {
                        if (!procedure.ResistType.HasValue)
                            throw new Exception("请输入焊接方式");
                    }

                    var proc = new Procedure
                    {
                        PdmId = procedure.PdmId,
                        No = procedure.No,
                        Name = procedure.Name,
                        WeldMethod = procedure.WeldMethod.Value,
                        WeldType = procedure.WeldType.Value,
                        ResistType = procedure.ResistType,
                        AutoLevel = procedure.AutoLevel,
                        TestState = ProgramTestState.未完成
                    };
                    process.Procedures.Add(proc);
                }
                var tasks = db.DispatchMessages.ToList();
                foreach (var task in tasks)
                {
                    if (task.PartNo == model.PartNo)
                    {
                        task.showState = false;
                    }
                }
                db.Processes.Add(process);
                db.SaveChanges();
                return Json(new { succeed = true, id = process.Id });
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { error = e.Message });
            }
        }

        [HttpPost]
        public ActionResult PdmRevise(string json)
        {
            try
            {
                var db = new Context();
                var model = new JavaScriptSerializer().Deserialize<PdmCreateModel>(json);

                var process = db.Processes
                    .Include("Procedures.Seams.InitialParams")
                    .Include("Procedures.Seams.RevisedParams")
                    .Where(x => x.PdmId == model.PdmId)
                    .OrderByDescending(x => x.Version)
                    .FirstOrDefault();

                if (process == null)
                    return Json(new { error = "指定工艺规程不存在" });

                if (process.ApprovalState != ApprovalState.审核通过)
                    return Json(new { error = "指定工艺规程没有审核通过，不能修订" });

                var maxVersion = db.Processes.Where(x => x.No == process.No).Select(x => x.Version).Max();
                if (process.Version + 1 == maxVersion)
                {
                    var newest = db.Processes.SingleOrDefault(x => x.No == process.No && x.Version == maxVersion);
                    if (newest?.ApprovalState != ApprovalState.审核通过)
                        return Json(new { error = "指定工艺规程正在修订中" });
                }

                if (process.Version != maxVersion)
                    return Json(new { error = "指定工艺规程不是最新版本，不能修订" });

                var author = db.Users.Find(Session.GetSessionUser().Id);
                var np = new Process
                {
                    No = process.No,
                    PdmId = process.PdmId,
                    PartNo = process.PartNo,
                    PartName = process.PartName,
                    Author = author,
                    ApprovalState = ApprovalState.未提交审核,
                    TestState = ProgramTestState.未完成,
                    Version = process.Version + 1,
                    Procedures = new List<Procedure>()
                };


                foreach (var procedure in model.Procedures)
                {
                    if (!procedure.WeldMethod.HasValue)
                        throw new Exception("请输入焊接方法");

                    if (!procedure.WeldType.HasValue)
                        throw new Exception("请输入焊接类型");

                    if (procedure.WeldMethod == WeldMethod.氩弧焊)
                    {
                        if (!procedure.AutoLevel.HasValue)
                            throw new Exception("请输入自动化程度");
                    }

                    if (procedure.WeldMethod == WeldMethod.电阻焊)
                    {
                        if (!procedure.ResistType.HasValue)
                            throw new Exception("请输入焊接方式");
                    }

                    var proc = new Procedure
                    {
                        No = procedure.No,
                        PdmId = procedure.PdmId,
                        Name = procedure.Name,
                        WeldMethod = procedure.WeldMethod.Value,
                        WeldType = procedure.WeldType.Value,
                        ResistType = procedure.ResistType,
                        AutoLevel = procedure.AutoLevel,
                        TestState = ProgramTestState.未完成,
                        Seams = new List<Seam>()
                    };
                    np.Procedures.Add(proc);
                }

                foreach (var procedure in process.Procedures)
                {
                    var npro = np.Procedures.Find(x => x.No == procedure.No);
                    if (npro != null)
                    {

                        foreach (var seam in procedure.Seams)
                        {
                            var ns = new Seam
                            {
                                No = seam.No,
                                TestState = TestState.尚未进行,
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
                                var npm = new SeamParam1
                                {
                                    Enum = param.Enum,
                                    Value = param.Value
                                };
                                ns.InitialParams.Add(npm);
                            }

                            foreach (var param in seam.RevisedParams)
                            {
                                var npm = new SeamParam2
                                {
                                    Enum = param.Enum,
                                    Value = param.Value
                                };
                                ns.RevisedParams.Add(npm);
                            }

                            npro.Seams.Add(ns);
                        }
                    }
                }

                db.Processes.Add(np);
                db.SaveChanges();
                return Json(new { succeed = true, id = np.Id });
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { error = e.Message });
            }
        }

        public void GetPdf(int id)
        {
            try
            {
                HttpContext.Response.ContentType = "application/pdf";
                var process = new Context()
                    .Processes
                    .Include("Procedures.Seams.InitialParams")
                    .Include("Procedures.Seams.RevisedParams")
                    .SingleOrDefault(x => x.Id == id);

                if (process == null)
                    throw new Exception("指定ID的工艺规程不存在");

                HttpContext.Response.BinaryWrite(new Reporter().CreateProcessPdf(process));
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }

        }
        public void GetRevisePdf(int id)
        {
            try
            {
                HttpContext.Response.ContentType = "application/pdf";
                var process = new Context()
                    .Processes.Include(x => x.Author)
                    .Include("Procedures.Seams.InitialParams")
                    .Include("Procedures.Seams.RevisedParams")
                    .SingleOrDefault(x => x.Id == id);

                if (process == null)
                    throw new Exception("指定ID的工艺规程不存在");

                HttpContext.Response.BinaryWrite(new Reporter().CreateReviseProcessPdf(process));
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }

        }

        public ActionResult GetByPdm(int id)
        {
            var process = new Context()
                .Processes
                .Include("Procedures.Seams.InitialParams")
                .Include("Procedures.Seams.RevisedParams")
                .SingleOrDefault(x => x.Id == id);

            if (process == null)
                throw new Exception("指定ID的工艺规程不存在");

            var proc = new Process1
            {
                processId = process.PdmId,
                procs = new List<Procedure1>()
            };

            foreach (var procedure in process.Procedures)
            {
                var proce = new Procedure1
                {
                    procId = procedure.PdmId,
                    weldMethod = procedure.WeldMethod.ToString(),
                    seams = new List<Seam1>()
                };

                foreach (var seam in procedure.Seams)
                {
                    var s = new Seam1
                    {
                        seamNo = seam.No,
                        Params = new Dictionary<string, string>()
                    };

                    if (seam.RevisedParams.Any())
                    {
                        foreach (var param in seam.RevisedParams)
                        {
                            s.Params.Add(param.Enum.ToString() + GetParamUnit(param.Enum), param.Value);
                        }
                    }
                    else
                    {
                        foreach (var param in seam.InitialParams)
                        {
                            s.Params.Add(param.Enum.ToString() + GetParamUnit(param.Enum), param.Value);
                        }
                    }

                    proce.seams.Add(s);
                }

                proc.procs.Add(proce);
            }

            return Json(new { json = proc.ToJson(), pdf = new Reporter().CreateProcessPdf(process).Base64() });
        }

        [HttpPost]
        public ActionResult GetByPdmId(string id)
        {
            Log.Info($"GetByPdmId(id:{id})");
            try
            {
                var processes = from p in new Context().Processes
                                group p by p.No
                into g
                                select g.FirstOrDefault(x => x.Version == g.Max(y => y.Version));
                var process = processes
                    .Include("Procedures.Seams.InitialParams")
                    .Include("Procedures.Seams.RevisedParams")
                .SingleOrDefault(x => x.PdmId == id);

                if (process == null)
                    return Json(new { succeed = false, error = "指定ID的工艺规程不存在" });

                if (!process.Published)
                    return Json(new { succeed = false, error = "指定工艺规程尚未发布" });

                var proc = new Process1
                {
                    processId = process.PdmId,
                    procs = new List<Procedure1>()
                };

                foreach (var procedure in process.Procedures)
                {
                    var proce = new Procedure1
                    {
                        procId = procedure.PdmId,
                        weldMethod = procedure.WeldMethod.ToString(),
                        seams = new List<Seam1>()
                    };

                    foreach (var seam in procedure.Seams)
                    {
                        var s = new Seam1
                        {
                            seamNo = seam.No,
                            Params = new Dictionary<string, string>()
                        };
                        /*
                        *氩弧焊：接头形式，焊缝间隙，验收标准，焊缝等级，钨极锥度，特殊过程确认标号
                        * 电阻焊：接头形式，焊缝间隙，验收标准，焊缝等级，焊机型别，电极直径，电极压力*2，特殊过程确认编号
                        * 电子束焊：接头类型，焊缝间隙，验收标准，焊缝等级，功率级数，加速电压，电子束束流，焊接速度，束流上升斜率，束流下降斜率，电子扫描偏移*3，特殊过程确认编号
                        *高频钎焊：接头形式，焊缝间隙，验收标准，焊缝等级，钎料牌号，填料规格，
                        */

                        IEnumerable<SeamParam> seamParams = null;

                        if (seam.RevisedParams.Any())
                        {
                            seamParams = seam.RevisedParams;
                        }
                        else
                        {
                            seamParams = seam.InitialParams;
                        }

                        if (seamParams != null)
                        {
                            foreach (var param in seamParams)
                            {
                                s.Params.Add(GetParamUnit(param.Enum), param.Value ?? "/");
                            }
                            if (procedure.WeldMethod == WeldMethod.电阻焊)
                            {
                                s.Params.Add("焊机型别", seam.WeldMachineClass ?? "/");
                                s.Params.Add("电极直径/滚轮宽度(mm)", seam.ElectrodeDiameter ?? "/");
                            }
                            if (procedure.WeldMethod == WeldMethod.氩弧焊)
                            {
                                s.Params.Add("钨极锥度", "/");
                            }
                            s.Params.Add("焊缝等级", seam.SeamLevel.ToString());
                            s.Params.Add("焊缝间隙(mm)", seam.Gap?.ToString() ?? "/");
                            s.Params.Add("接头形式", seam.JointForm.ToString());
                            s.Params.Add("验收标准", seam.CheckStandard ?? "/");
                            s.Params.Add("特殊过程确认报告编号", seam.SpecialReportFileNo ?? "/");
                        }
                        proce.seams.Add(s);
                    }

                    proc.procs.Add(proce);
                }
                Log.Info($"GetByPdmId(id:{id}) returns\r\n{new PdmResult { process = proc.ToJson() }.ToJson()}");
                return Json(new PdmResult { process = proc.ToJson(), pdf = new Reporter().CreateProcessPdf(process).Base64() });
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                var error = "";
                do
                {
                    error += e.Message;
                    e = e.InnerException;
                } while (e != null);
                return Json(new { succeed = false, error });
            }
        }

        [HttpPost]
        public ActionResult EditProcessUser(int? copId, int? creId, string Author, string nowUser)
        {
            try
            {
                var db = new Context();

                var userId = Session.GetSessionUser().Id;

                var processes = from p in db.Processes
                                group p by p.No
                    into g
                                select g.FirstOrDefault(x => x.Version == g.Max(y => y.Version));

                var completes = processes.Include(x => x.Author).Where(x => x.ApprovalState == ApprovalState.审核通过 && x.Id == copId).ToList();
                var creation = processes.Include(x => x.Author).Where(x => x.ApprovalState != ApprovalState.审核通过 && x.Id == creId).ToList();

                var User = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Name == nowUser || x.Id == nowUser);

                if (User == null)
                {
                    return Json(new { succeed = false, type = true, error = "指定现编制者不存在！" });
                    //throw new Exception("指定编制者或工艺规程不存在！");
                }
                if (completes != null)
                {
                    foreach (var process in completes)
                    {
                        var completesUser = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == userId);
                        foreach (var author in completesUser.AuthorityRs)
                        {
                            if (author.Authority == Authority.用户管理)
                            {
                                process.Author = User;
                                db.SaveChanges();
                                return Json(new { succeed = true, type = true });
                            }
                        }
                    }
                }
                if (creation != null)
                {
                    foreach (var process in creation)
                    {
                        var creationUser = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == userId);
                        foreach (var author in creationUser.AuthorityRs)
                        {
                            if (author.Authority == Authority.用户管理)
                            {
                                process.Author = User;
                                db.SaveChanges();
                                return Json(new { succeed = true, type = false });
                            }
                        }
                    }
                }

                return Json(new { succeed = false, type = true, error = "当前编制者无用户管理权限！" });
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false, error = e.Message });
            }
        }

        [HttpPost]
        public ActionResult DisPlayApprovalState(int id)
        {
            var db = new Context();

            var approves = db.Approves.SingleOrDefault(x => x.ProcessId == id);

            if (approves != null)
            {
                var userApprover = db.Users.SingleOrDefault(x => x.Id == approves.ApproverId);//审核人员ID

                var userProofer = db.Users.SingleOrDefault(x => x.Id == approves.ProoferId);//校队人员ID

                var userCurrenter = db.Users.SingleOrDefault(x => x.Id == approves.CurrenterId);//CurrenterId

                string userNameApprover = userApprover.Name;

                string userNameProofer = userProofer.Name;

                string userNameCurrenter = userCurrenter.Name;

                return Json(new { succeed = true, approverName = userNameApprover, prooferName = userNameProofer, currenterName = userNameCurrenter });

            }
            return Json(new { succeed = false, error = "暂无校对与审核数据！" });
        }

        private string GetParamUnit(WeldParam param)
        {
            switch (param)
            {
                case WeldParam.焊接电流:
                    return param.ToString() + "(A)";
                case WeldParam.焊接电压:
                    return param.ToString() + "(V)";
                case WeldParam.焊接速度:
                    return param.ToString() + "(m/min)";
                case WeldParam.氩气流量正面:
                    return param.ToString() + "(L/min)";
                case WeldParam.氩气流量反面:
                    return param.ToString() + "(L/min)";
                case WeldParam.电流衰减:
                    return param.ToString() + "(s)";
                case WeldParam.保护气滞后:
                    return param.ToString() + "(m/s)";
                case WeldParam.填充材料规格:
                    return param.ToString() + "(mm)";
                case WeldParam.送丝速度:
                    return param.ToString() + "(m/s)";
                case WeldParam.钨极直径:
                    return param.ToString() + "(mm)";
                case WeldParam.喷嘴直径:
                    return param.ToString() + "(mm)";
                case WeldParam.预压:
                    return param.ToString() + "(ms)";
                case WeldParam.抬起:
                    return "电极压力抬起(KPa)";
                case WeldParam.压下:
                    return "电极压力压下(KPa)";
                case WeldParam.脉冲1:
                    return param.ToString() + "(ms)";
                case WeldParam.焊接电流1:
                    return param.ToString() + "(A)";
                case WeldParam.冷却:
                    return param.ToString() + "(ms)";
                case WeldParam.脉冲2:
                    return param.ToString() + "(ms)";
                case WeldParam.焊接电流2:
                    return param.ToString() + "(A)";
                case WeldParam.休止:
                    return param.ToString() + "(ms)";
                case WeldParam.维持:
                    return param.ToString() + "(ms)";
                case WeldParam.下气室气压:
                    return param.ToString() + "(KPa)";
                case WeldParam.中气室气压:
                    return param.ToString() + "(KPa)";
                case WeldParam.熔核直径:
                    return param.ToString() + "(mm)";
                case WeldParam.加速电压:
                    return param.ToString() + "(mV)";
                case WeldParam.电子束流:
                    return "电子束束流(mA)";
                case WeldParam.聚焦电流:
                    return param.ToString() + "(mA)";
                case WeldParam.工作距离:
                    return param.ToString() + "(mm)";
                case WeldParam.焊接真空度:
                    return param.ToString() + "(Pa)";
                case WeldParam.氩气流量:
                    return param.ToString() + "(L/min)";
                case WeldParam.上升:
                    return "束流上升斜率控制/S";
                case WeldParam.下降:
                    return "束流下降斜率控制/S";
                case WeldParam.波形:
                    return "电子束扫描偏转波形";
                case WeldParam.频率:
                    return "电子束扫描偏转频率";
                case WeldParam.幅值:
                    return "电子束扫描偏转幅值";
                case WeldParam.钎料规格:
                    return "填料规格(mm)";
            }

            return param.ToString();
        }

        /// <summary>
        /// 批量导出excel
        /// </summary>
        /// <param name="processIdes"></param>
        public void ImportProcesses(string processIdes)
        {
            try
            {
                string fileName = string.Empty;
                fileName = DateTime.Now.ToString() + ".xls";
                string[] ids = processIdes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//剔除空值
                ExportExcelForPercentForWebs(fileName, fileName, ids);
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
        }
        public static void ExportExcelForPercentForWebs(string sheetName, string xlsname, string[] areaid)
        {
            XlsDocument xls = new XlsDocument();
            DataSet ds = GetDataTableForPercents(areaid);
            Worksheet sheet;  
            try
            {
                for (int a = 0; a < ds.Tables.Count; a++)
                {
                    sheet = xls.Workbook.Worksheets.Add(ds.Tables[a].TableName);
                    DataTable table = ds.Tables[a];
                    if (table == null || table.Rows.Count == 0) { return; }

                    //填充表头  
                    foreach (DataColumn col in table.Columns)
                    {
                        sheet.Cells.Add(1, col.Ordinal + 1, col.ColumnName);
                    }

                    //填充内容  
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            sheet.Cells.Add(i + 2, j + 1, table.Rows[i][j].ToString());
                        }
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    xls.Save(ms);
                    ms.Flush();
                    ms.Position = 0;
                    sheet = null;
                    xls = null;
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();

                    response.Charset = "UTF-8";
                    response.ContentType = "application/vnd-excel";
                    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + xlsname));

                    byte[] data = ms.ToArray();
                    System.Web.HttpContext.Current.Response.BinaryWrite(data);
                }

            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
            finally
            {
                sheet = null;
                xls = null;
            }

        }

        private static DataSet GetDataTableForPercents(string[] areaid)
        {
            var db = new Context();

            int[] ids = Array.ConvertAll<string, int>(areaid, delegate (string s) { return int.Parse(s); });

            var processes = from p in db.Processes
                            group p by p.No
               into g
                            select g.FirstOrDefault(x => x.Version == g.Max(y => y.Version));
            var processDbs = db.Processes.Include(x => x.Author).Include("Procedures.Seams.InitialParams").Where(x => ids.Contains(x.Id)).ToArray();

            DataSet ds = new DataSet();

            Dictionary<WeldMethod, DataTable> dic = new Dictionary<WeldMethod, DataTable>();

            try
            {
                foreach (var processDb in processDbs)
                {
                    foreach (var procedures in processDb.Procedures)
                    {
                        DataTable dt;
                        if (!dic.TryGetValue(procedures.WeldMethod, out dt))
                        {
                            dt = new DataTable();//创建表
                            dic.Add(procedures.WeldMethod, dt);
                            dt.Columns.Add("工艺规程编号", typeof(String));
                            dt.Columns.Add("自动化程度", typeof(String));
                            dt.Columns.Add("零件名称", typeof(String));
                            dt.Columns.Add("零件号", typeof(String));
                            dt.Columns.Add("工序名称", typeof(String));
                            dt.Columns.Add("焊接方法", typeof(String));
                            dt.Columns.Add("焊接类型", typeof(String));
                            dt.Columns.Add("工序号", typeof(String));
                            dt.Columns.Add("工步", typeof(String));
                            dt.Columns.Add("编制者", typeof(String));
                            dt.Columns.Add("电流极性", typeof(String));
                            dt.Columns.Add("是否填丝", typeof(String));
                            dt.Columns.Add("接头形式", typeof(String));
                            dt.Columns.Add("焊缝等级", typeof(String));
                            dt.Columns.Add("焊缝间隙", typeof(String));
                            dt.Columns.Add("验收标准", typeof(String));
                            dt.Columns.Add("材料牌号1", typeof(String));
                            dt.Columns.Add("材料规格1", typeof(String));
                            dt.Columns.Add("材料牌号2", typeof(String));
                            dt.Columns.Add("材料规格2", typeof(String));
                            dt.Columns.Add("焊接速度", typeof(String));
                            dt.Columns.Add("材料牌号3", typeof(String));
                            dt.Columns.Add("材料规格3", typeof(String));
                            dt.Columns.Add("材料牌号4", typeof(String));
                            dt.Columns.Add("材料规格4", typeof(String));
                            if (procedures.WeldMethod == WeldMethod.氩弧焊)
                            {
                                dt.Columns.Add("电极直径/滚轮宽度", typeof(String));
                                dt.Columns.Add("焊机型别", typeof(String));
                                dt.Columns.Add("涵盖报告编号", typeof(String));
                                dt.Columns.Add("焊接电流", typeof(String));
                                dt.Columns.Add("氩气流量正面", typeof(String));
                                dt.Columns.Add("氩气流量反面", typeof(String));
                                dt.Columns.Add("电流衰减", typeof(String));
                                dt.Columns.Add("保护气滞后", typeof(String));
                                dt.Columns.Add("填充材料牌号", typeof(String));
                                dt.Columns.Add("填充材料规格", typeof(String));
                                dt.Columns.Add("送丝速度", typeof(String));
                                dt.Columns.Add("钨极直径", typeof(String));
                                dt.Columns.Add("喷嘴直径", typeof(String));
                            }
                            if (procedures.WeldMethod == WeldMethod.电阻焊)
                            {
                                dt.Columns.Add("焊接方式", typeof(String));
                                dt.Columns.Add("焊机型别", typeof(String));
                                dt.Columns.Add("电极直径/滚轮宽度", typeof(String));
                                dt.Columns.Add("功率级数", typeof(String));
                                dt.Columns.Add("预压", typeof(String));
                                dt.Columns.Add("电极压力抬起", typeof(String));
                                dt.Columns.Add("电极压力压下", typeof(String));
                                dt.Columns.Add("脉冲1", typeof(String));
                                dt.Columns.Add("焊接电流1", typeof(String));
                                dt.Columns.Add("冷却", typeof(String));
                                dt.Columns.Add("脉冲2", typeof(String));
                                dt.Columns.Add("焊接电流2", typeof(String));
                                dt.Columns.Add("维持", typeof(String));
                                dt.Columns.Add("休止", typeof(String));
                                dt.Columns.Add("下气室气压", typeof(String));
                                dt.Columns.Add("中气室气压", typeof(String));
                                dt.Columns.Add("熔核直径", typeof(String));
                            }
                            if (procedures.WeldMethod == WeldMethod.电子束焊)
                            {
                                dt.Columns.Add("功率", typeof(String));
                                dt.Columns.Add("加速电压", typeof(String));
                                dt.Columns.Add("电子束流", typeof(String));
                                dt.Columns.Add("聚焦电流", typeof(String));
                                dt.Columns.Add("工作距离", typeof(String));
                                dt.Columns.Add("束流上升斜率控制", typeof(String));
                                dt.Columns.Add("束流下降斜率控制", typeof(String));
                                dt.Columns.Add("焊接真空度", typeof(String));
                                dt.Columns.Add("电子束扫描偏转波形", typeof(String));
                                dt.Columns.Add("电子束扫描偏转幅值", typeof(String));
                                dt.Columns.Add("电子束扫描偏转频率", typeof(String));

                            }
                            if (procedures.WeldMethod == WeldMethod.高频钎焊)
                            {
                                dt.Columns.Add("钎料牌号", typeof(String));
                                dt.Columns.Add("填料规格", typeof(String));
                                dt.Columns.Add("焊接电压", typeof(String));
                                dt.Columns.Add("氩气流量", typeof(String));
                                dt.Columns.Add("感应圈编号", typeof(String));
                                dt.Columns.Add("管子规格", typeof(String));
                            }
                            dt.Columns.Add("焊缝编号", typeof(String));
                            dt.Columns.Add("操作者", typeof(String));
                            dt.Columns.Add("检验者", typeof(String));
                            dt.Columns.Add("特殊过程确认报告编号", typeof(String));

                            dt.TableName = procedures.WeldMethod.ToString();
                            ds.Tables.Add(dt);
                        }

                        foreach (var seams in procedures.Seams)
                        {
                            var initialParamDict = new Dictionary<WeldParam, string>();

                            if (seams.InitialParams.Any())
                            {
                                var par = seams.InitialParams.Select(x => x.Enum).ToList();
                                
                                foreach (var param in seams.InitialParams)
                                {
                                    if (par.Contains(WeldParam.无效) || par.Contains(WeldParam.钨极直径) || par.Contains(WeldParam.下气室气压) || par.Contains(WeldParam.幅值))
                                    {
                                        int numEnum = (int)param.Enum;

                                        if (numEnum > 0)
                                        {
                                            WeldParam wp = (WeldParam)numEnum + 2;
                                            initialParamDict.Add(wp, param.Value);
                                        }
                                    }
                                    else
                                    {
                                        initialParamDict.Add(param.Enum, param.Value);
                                    }
                                }
                            }
                            DataRow dr;
                            dr = dt.NewRow();
                            dr["工艺规程编号"] = processDb.No;
                            dr["零件号"] = processDb.PartNo;
                            dr["零件名称"] = processDb.PartName;
                            dr["工序号"] = procedures.No;
                            dr["工序名称"] = procedures.Name;
                            dr["焊接方法"] = procedures.WeldMethod.ToString();
                            dr["焊接类型"] = procedures.WeldType.ToString();
                            dr["焊缝编号"] = seams.No;
                            dr["接头形式"] = seams.JointForm.ToString();
                            dr["焊缝等级"] = seams.SeamLevel.ToString();
                            dr["验收标准"] = seams.CheckStandard;
                            dr["材料牌号1"] = seams.Material1;
                            dr["材料规格1"] = seams.Thick1.ToString();
                            dr["材料牌号2"] = seams.Material2;
                            dr["材料规格2"] = seams.Thick2.ToString();
                            if (processDb.Author!=null)
                            {
                                dr["编制者"] = processDb.Author.Id;
                            }
                            if (procedures.WeldMethod == WeldMethod.电阻焊)
                            {
                                dr["材料牌号3"] = seams.Material3;
                                dr["材料规格3"] = seams.Thick3.ToString();
                                dr["材料牌号4"] = seams.Material4;
                                dr["材料规格4"] = seams.Thick4.ToString();
                                dr["焊接方式"] = procedures.ResistType.ToString();
                                dr["焊机型别"] = seams.WeldMachineClass;
                                dr["电极直径/滚轮宽度"] = seams.ElectrodeDiameter;
                                dr["功率级数"] = initialParamDict.GetParamValue(WeldParam.功率级数);
                                dr["预压"] = initialParamDict.GetParamValue(WeldParam.预压);
                                dr["电极压力抬起"] = initialParamDict.GetParamValue(WeldParam.抬起);
                                dr["电极压力压下"] = initialParamDict.GetParamValue(WeldParam.压下);
                                dr["脉冲1"] = initialParamDict.GetParamValue(WeldParam.脉冲1);
                                dr["焊接电流1"] = initialParamDict.GetParamValue(WeldParam.焊接电流1);
                                dr["冷却"] = initialParamDict.GetParamValue(WeldParam.冷却);
                                dr["脉冲2"] = initialParamDict.GetParamValue(WeldParam.脉冲2);
                                dr["焊接电流2"] = initialParamDict.GetParamValue(WeldParam.焊接电流2);
                                dr["维持"] = initialParamDict.GetParamValue(WeldParam.维持);
                                dr["休止"] = initialParamDict.GetParamValue(WeldParam.休止);
                                dr["下气室气压"] = initialParamDict.GetParamValue(WeldParam.下气室气压);
                                dr["中气室气压"] = initialParamDict.GetParamValue(WeldParam.中气室气压);
                                dr["焊接速度"] = initialParamDict.GetParamValue(WeldParam.焊接速度);
                                dr["熔核直径"] = initialParamDict.GetParamValue(WeldParam.熔核直径);
                            }
                            if (procedures.WeldMethod == WeldMethod.氩弧焊)
                            {
                                dr["材料牌号3"] = seams.Material3;
                                dr["材料规格3"] = seams.Thick3.ToString();
                                dr["材料牌号4"] = seams.Material4;
                                dr["材料规格4"] = seams.Thick4.ToString();
                                dr["自动化程度"] = procedures.AutoLevel.ToString();
                                dr["涵盖报告编号"] = seams.CoverReportFileNo;
                                dr["焊接电流"] = initialParamDict.GetParamValue(WeldParam.焊接电流);
                                dr["焊接速度"] = initialParamDict.GetParamValue(WeldParam.焊接速度);
                                dr["氩气流量正面"] = initialParamDict.GetParamValue(WeldParam.氩气流量正面);
                                dr["氩气流量反面"] = initialParamDict.GetParamValue(WeldParam.氩气流量反面);
                                dr["电流衰减"] = initialParamDict.GetParamValue(WeldParam.电流衰减);
                                dr["保护气滞后"] = initialParamDict.GetParamValue(WeldParam.保护气滞后);
                                dr["填充材料牌号"] = initialParamDict.GetParamValue(WeldParam.填充材料牌号);
                                dr["填充材料规格"] = initialParamDict.GetParamValue(WeldParam.填充材料规格);
                                dr["送丝速度"] = initialParamDict.GetParamValue(WeldParam.送丝速度);
                                dr["钨极直径"] = initialParamDict.GetParamValue(WeldParam.钨极直径);
                                dr["喷嘴直径"] = initialParamDict.GetParamValue(WeldParam.喷嘴直径);
                            }
                            if (procedures.WeldMethod == WeldMethod.电子束焊)
                            {
                                dr["功率"] = initialParamDict.GetParamValue(WeldParam.功率);
                                dr["加速电压"] = initialParamDict.GetParamValue(WeldParam.加速电压);
                                dr["电子束流"] = initialParamDict.GetParamValue(WeldParam.电子束流);
                                dr["焊接速度"] = initialParamDict.GetParamValue(WeldParam.焊接速度);
                                dr["聚焦电流"] = initialParamDict.GetParamValue(WeldParam.聚焦电流);
                                dr["工作距离"] = initialParamDict.GetParamValue(WeldParam.工作距离);
                                dr["束流上升斜率控制"] = initialParamDict.GetParamValue(WeldParam.上升);
                                dr["束流下降斜率控制"] = initialParamDict.GetParamValue(WeldParam.下降);
                                dr["电子束扫描偏转波形"] = initialParamDict.GetParamValue(WeldParam.波形);
                                dr["电子束扫描偏转幅值"] = initialParamDict.GetParamValue(WeldParam.幅值);
                                dr["电子束扫描偏转频率"] = initialParamDict.GetParamValue(WeldParam.频率);
                            }
                            if (procedures.WeldMethod == WeldMethod.高频钎焊)
                            {
                                dr["钎料牌号"] = initialParamDict.GetParamValue(WeldParam.钎料牌号);
                                dr["填料规格"] = initialParamDict.GetParamValue(WeldParam.钎料规格);
                                dr["焊接电压"] = initialParamDict.GetParamValue(WeldParam.焊接电压);
                                dr["氩气流量"] = initialParamDict.GetParamValue(WeldParam.氩气流量);
                                dr["感应圈编号"] = initialParamDict.GetParamValue(WeldParam.感应圈编号);
                                dr["管子规格"] = initialParamDict.GetParamValue(WeldParam.管子规格);
                            }
                            dr["操作者"] = seams.TestWelder;
                            dr["检验者"] = seams.TestChecker;
                            dr["特殊过程确认报告编号"] = seams.SpecialReportFileNo;
                            dt.Rows.Add(dr);
                        }
                    }
                }
                return ds;
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return null;
            }

        }
        [HttpPost]
        public ActionResult EditApprove(int pid, string ApproverId, string ProoferId)
        {
            var db = new Context();
            var usrid = Session.GetSessionUser().Id;

            var approverIdUser = db.Users.SingleOrDefault(x => x.Name == ApproverId || x.Id == ApproverId);
            var prooferIdUser = db.Users.SingleOrDefault(x => x.Name == ProoferId || x.Id == ProoferId);
            if (ApproverId == ProoferId)
            {
                return Json(new { succeed = false, type = true, error = "审核人和校队人不能是同一人！" });
            }
            var processes = from p in db.Processes
                            group p by p.No
                    into g
                            select g.FirstOrDefault(x => x.Version == g.Max(y => y.Version));

            var completes = processes.Include(x => x.Author).Where(x => x.ApprovalState != ApprovalState.审核通过 && x.Id == pid).ToList();
            foreach (var process in completes)
            {
                if (process.Author != null)
                {
                    if (usrid == process.Author.Id)
                    {
                        var approves = db.Approves.Include("Process").SingleOrDefault(x => x.ProcessId == pid);
                        if (process.Author.Id == approves.ProoferId || process.Author.Id == approves.ApproverId)
                        {
                            return Json(new { succeed = false, type = true, error = "不能交给自己审核或者提交！" });
                        }

                        if (approverIdUser != null)
                        {
                            if (approverIdUser.Id != approves.ProoferId)
                            {
                                approves.ApproverId = approverIdUser.Id;
                                approves.CurrenterId = approverIdUser.Id;
                                db.SaveChanges();
                                return Json(new { succeed = true });
                            }
                            else
                            {
                                return Json(new { succeed = false, type = true, error = "审核人和校队人不能是同一人！" });
                            }

                        }
                        else if (prooferIdUser != null)
                        {
                            if (prooferIdUser.Id != approves.ApproverId)
                            {
                                approves.ProoferId = prooferIdUser.Id;
                                approves.CurrenterId = prooferIdUser.Id;
                                db.SaveChanges();
                                return Json(new { succeed = true });
                            }
                            else
                            {
                                return Json(new { succeed = false, type = true, error = "审核人和校队人不能是同一人！" });
                            }

                        }
                        else if (prooferIdUser != null && approverIdUser != null)
                        {
                            if (prooferIdUser.Id != approves.ApproverId && approverIdUser.Id != approves.ProoferId)
                            {
                                approves.ApproverId = approverIdUser.Id;
                                approves.ProoferId = prooferIdUser.Id;

                                approves.CurrenterId = prooferIdUser.Id;

                                db.SaveChanges();
                                return Json(new { succeed = true });
                            }
                            else
                            {
                                return Json(new { succeed = false, type = true, error = "审核人和校队人不能是同一人！" });
                            }

                        }
                        else
                        {
                            return Json(new { succeed = false, type = true, error = "修改失败！" });
                        }

                    }
                    else
                    {
                        return Json(new { succeed = false, type = true, error = "不是当前用户，不可修改！" });
                    }
                }
                else
                {
                    return Json(new { succeed = false, type = true, error = "编制者不存在！" });
                }

            }
            return Json(new { succeed = false, type = true, error = "修改失败！" });

        }


        public void CheckMessage()
        {
            try
            {
                var db = new Context();
                var processes = db
                        .Processes.Include(x => x.Author)
                        .Include("Procedures.Seams.InitialParams")
                        .Include("Procedures.Seams.RevisedParams")
                        .ToList();
                List<string> list = new List<string>();
               
                foreach (var process in processes)
                {
                    foreach (var procedure in process.Procedures)
                    {
                        foreach (var seam in procedure.Seams)
                        {
                            if (!seam.InitialParams.Any())
                                continue;

                            var std = GlobalData.MethodParamMap[procedure.WeldMethod];
                            var par = seam.InitialParams.Select(x => x.Enum).ToList();
                            
                            if (std.Count != par.Count || par.Count(t => !std.Contains(t)) > 0)
                                list.Add(process.Id.ToString());
                           
                        }
                    }
                }

                List<string> newList = list.Distinct().ToList();//实现去重

                if (newList.Any())
                {
                    string[] arrayList = newList.ToArray();
                    string fileName = string.Empty;
                    fileName = DateTime.Now.ToString() + ".xls";
                    ExportExcelForPercentForWebs(fileName, fileName, arrayList);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
        }

        [HttpPost]
        public ActionResult CheckMessageWords()
        {
            var db = new Context();
            var seams = db.Seams
               .Include(x => x.Procedure)
               .Include(x => x.InitialParams)
               .Include(x => x.RevisedParams).ToList();

            foreach (var seam in seams)
            {
                if (!seam.InitialParams.Any())
                    continue;

                if (!seam.ElectrodeDiameter.IsEmpty())
                {

                    seam.ElectrodeDiameter = seam.ElectrodeDiameter.Replace("φ", "");


                    seam.ElectrodeDiameter = seam.ElectrodeDiameter.Replace("ψ", "");


                    seam.ElectrodeDiameter = seam.ElectrodeDiameter.Replace("Φ", "");


                    seam.ElectrodeDiameter = seam.ElectrodeDiameter.Replace("～", "-");
                }

                foreach (var parm in seam.InitialParams)
                {
                    if (!parm.Value.IsEmpty())
                    {

                        parm.Value = parm.Value.Replace("φ", "");


                        parm.Value = parm.Value.Replace("ψ", "");


                        parm.Value = parm.Value.Replace("Φ", "");


                        parm.Value = parm.Value.Replace("～", "-");
                    }

                }
                if (seam.RevisedParams.Any())
                {
                    foreach (var parm in seam.RevisedParams)
                    {
                        if (!parm.Value.IsEmpty())
                        {

                            parm.Value = parm.Value.Replace("φ", "");


                            parm.Value = parm.Value.Replace("ψ", "");


                            parm.Value = parm.Value.Replace("Φ", "");


                            parm.Value = parm.Value.Replace("～", "-");
                        }
                    }
                }
                db.SaveChanges();
            }

            return Json(new { succeed = true });
        }
        public void checkM(List<string> newList)
        {
            XlsDocument xls = new XlsDocument();
            string[] newArray = newList[0].Split(',');
            Worksheet sheet;
            DataTable dt = new DataTable();

            dt.Columns.Add("问题数据:");

            DataRow dr;
            for (int i = 0; i < newArray.Length; i++)
            {
                dr = dt.NewRow();
                dr["问题数据:"] = newArray[i];
                dt.Rows.Add(dr);
            }

            try
            {
                sheet = xls.Workbook.Worksheets.Add("修正数据");
                DataTable table = dt;
                //if (table == null || table.Rows.Count == 0) { continue; }

                //填充表头  
                foreach (DataColumn col in table.Columns)
                {
                    sheet.Cells.Add(1, col.Ordinal + 1, col.ColumnName);
                }

                //填充内容  
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        sheet.Cells.Add(i + 2, j + 1, table.Rows[i][j].ToString());
                    }
                }
                string xlsname = DateTime.Now.ToString() + ".xls";
                using (MemoryStream ms = new MemoryStream())
                {
                    xls.Save(ms);
                    ms.Flush();
                    ms.Position = 0;
                    sheet = null;
                    xls = null;
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();

                    response.Charset = "UTF-8";
                    response.ContentType = "application/vnd-excel";
                    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + xlsname));

                    byte[] data = ms.ToArray();
                    System.Web.HttpContext.Current.Response.BinaryWrite(data);
                }

            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
            finally
            {
                sheet = null;
                xls = null;
            }
        }

    }
}