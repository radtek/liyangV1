using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WCAPP.Libs;
using WCAPP.Models.Database;
using WCAPP.Models.Home;
using WCAPP.Models.HomeModels;
using WCAPP.Models.UserModels;
using WCAPP.Types;
using WCAPP.Utils;

namespace WCAPP.Controllers
{
    public class TaskController : Controller
    {
        public ActionResult Index(int? pageNo,string searchStartTime,string searchEndTime, string searchState, string searchId, int? pageSize)
        {
            var db = new Context();
            List<int> lint = new List<int>();
            lint.Add(10);
            lint.Add(20);
            lint.Add(50);
            lint.Add(100);
            lint.Add(200);

            int pNo = pageNo ?? 1;
            int pSize = pageSize ?? 10;

            SelectList lints = new SelectList(lint, pSize);
            ViewBag.PageSize = lints;
            ViewBag.PageSizes = pSize;
            int rowsCount = 0;
            int crtNo = 0;
            List<DispatchMessage> list = new List<DispatchMessage>();

            if (searchId != null|| searchStartTime != null || searchEndTime != null || searchState!= null)
            {   
                //时间
                if (searchStartTime != ""&& searchEndTime!= "" && searchId== "" && searchState== "")
                {
                    //ViewBag.SearchTime = searchType;
                    ViewBag.SearchStartTime = searchStartTime;
                    ViewBag.SearchEndTime = searchEndTime;
                    if (searchStartTime != ""&& searchEndTime != "")
                    {                        
                        DateTime startTimes = Convert.ToDateTime(searchStartTime);
                        DateTime endTimes = Convert.ToDateTime(searchEndTime);
                        rowsCount = db.DispatchMessages.Where(x => x.showState==false&&x.exportTime >= startTimes && x.exportTime <= endTimes && x.importTime >= startTimes && x.importTime <= endTimes).Count();

                        // 调整当前页码不超过总页数
                        crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                        crtNo = crtNo <= 0 ? 1 : crtNo;

                        //总记录数

                        list = db.DispatchMessages.Where(x => x.showState == false && x.exportTime >= startTimes && x.exportTime <= endTimes && x.importTime >= startTimes && x.importTime <= endTimes).OrderBy(a => a.TaskNo).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                    }
                                    
                    
                }
                //状态
                if (searchState!= "" && searchStartTime == "" && searchEndTime == "" && searchId == "")
                {
                    //ViewBag.SearchState = searchType;
                    rowsCount = db.DispatchMessages.Where(x => x.showState == false && (x.State ? "true" : "false") == searchState).Count();
                    ViewBag.SearchStates = searchState;
                    // 调整当前页码不超过总页数
                    crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                    crtNo = crtNo <= 0 ? 1 : crtNo;

                    //总记录数

                    list = db.DispatchMessages.Where(x => x.showState == false && (x.State ? "true" : "false") == searchState).OrderBy(a => a.TaskNo).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                }
                //零件号
                if (searchId != "" && searchState == "" && searchStartTime == "" && searchEndTime == "")
                {
                    //ViewBag.SearchId = searchType;
                    rowsCount = db.DispatchMessages.Where(x => x.showState == false && x.PartNo.Contains(searchId)).Count();

                    // 调整当前页码不超过总页数
                    crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                    crtNo = crtNo <= 0 ? 1 : crtNo;

                    //总记录数

                    list = db.DispatchMessages.Where(x => x.showState == false && x.PartNo.Contains(searchId)).OrderBy(a => a.State).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                }

                //时间+状态+零件号
                if (searchId != "" && searchStartTime != "" && searchEndTime != "" && searchState != "")
                {
                    DateTime startTimes = Convert.ToDateTime(searchStartTime);
                    DateTime endTimes = Convert.ToDateTime(searchEndTime);
                    rowsCount = db.DispatchMessages.Where(x => x.showState == false && ((x.State ? "true" : "false") == searchState) && x.PartNo.Contains(searchId) && x.exportTime >= startTimes && x.exportTime <= endTimes && x.importTime >= startTimes && x.importTime <= endTimes).Count();

                    // 调整当前页码不超过总页数
                    crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                    crtNo = crtNo <= 0 ? 1 : crtNo;

                    //总记录数

                    list = db.DispatchMessages.Where(x => x.showState == false && ((x.State ? "true" : "false") == searchState) && x.PartNo.Contains(searchId) && x.exportTime >= startTimes && x.exportTime <= endTimes && x.importTime >= startTimes && x.importTime <= endTimes).OrderBy(a => a.TaskNo).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                }
                //时间+状态
                if (searchStartTime != "" && searchEndTime != "" && searchState != ""&&searchId=="")
                {
                    DateTime startTimes = Convert.ToDateTime(searchStartTime);
                    DateTime endTimes = Convert.ToDateTime(searchEndTime);
                    rowsCount = db.DispatchMessages.Where(x => x.showState == false && ((x.State ? "true" : "false") == searchState) && x.exportTime >= startTimes && x.exportTime <= endTimes && x.importTime >= startTimes && x.importTime <= endTimes).Count();

                    // 调整当前页码不超过总页数
                    crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                    crtNo = crtNo <= 0 ? 1 : crtNo;

                    //总记录数

                    list = db.DispatchMessages.Where(x => x.showState == false && ((x.State ? "true" : "false") == searchState) && x.exportTime >= startTimes && x.exportTime <= endTimes && x.importTime >= startTimes && x.importTime <= endTimes).OrderBy(a => a.TaskNo).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                }
                //时间+零件号
                if (searchStartTime != "" && searchEndTime != "" && searchId != ""&&searchState=="")
                {
                    DateTime startTimes = Convert.ToDateTime(searchStartTime);
                    DateTime endTimes = Convert.ToDateTime(searchEndTime);
                    rowsCount = db.DispatchMessages.Where(x => x.showState == false && x.PartNo.Contains(searchId) && x.exportTime >= startTimes && x.exportTime <= endTimes && x.importTime >= startTimes && x.importTime <= endTimes).Count();

                    // 调整当前页码不超过总页数
                    crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                    crtNo = crtNo <= 0 ? 1 : crtNo;

                    //总记录数

                    list = db.DispatchMessages.Where(x => x.showState == false && x.PartNo.Contains(searchId) && x.exportTime >= startTimes && x.exportTime <= endTimes && x.importTime >= startTimes && x.importTime <= endTimes).OrderBy(a => a.TaskNo).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                }
                ////状态+零件号
                if (searchId != "" && searchState != ""&& searchStartTime == "" && searchEndTime == "")
                {
                    
                    rowsCount = db.DispatchMessages.Where(x => x.showState == false && ((x.State ? "true" : "false") == searchState) && x.PartNo.Contains(searchId)).Count();

                    // 调整当前页码不超过总页数
                    crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                    crtNo = crtNo <= 0 ? 1 : crtNo;

                    //总记录数

                    list = db.DispatchMessages.Where(x => x.showState == false && ((x.State ? "true" : "false") == searchState) && x.PartNo.Contains(searchId)).OrderBy(a => a.TaskNo).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                }
                if(searchId == "" && searchState == "" && searchStartTime == "" && searchEndTime == "")
                {                    
                    rowsCount = db.DispatchMessages.Where(x => x.showState == false).Count();

                    // 调整当前页码不超过总页数
                    crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                    crtNo = crtNo <= 0 ? 1 : crtNo;

                    //总记录数

                    list = db.DispatchMessages.Where(x => x.showState == false).OrderBy(a => a.TaskNo).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                }
                //总页数

                int pageCount = (rowsCount - 1) / pSize + 1;
                int nextPageNo = crtNo >= pageCount ? pageCount : crtNo + 1;//计算下一页页号
                int prevPageNo = crtNo == 1 ? 1 : crtNo - 1;//计算上一页页号
                ViewBag.NextPageNo = nextPageNo;
                ViewBag.PrevPageNo = prevPageNo; 
                ViewBag.PageCount = pageCount;  //总页数
                ViewBag.PageNo = crtNo;  //当前页号
                ViewBag.SearchId = searchId;   //搜索员工姓名

                List<int> listPage = new List<int>();

                for (int i = 1; i <= pageCount; i++)
                {
                    listPage.Add(i);
                }
                SelectList li = new SelectList(listPage, crtNo);
                ViewBag.PageList = li;
                ViewBag.task = list;
                return View();
            }
            else
            {
                rowsCount = db.DispatchMessages.Where(x => x.showState == false).Count();
                // int pCount = (int)Math.Ceiling(1.0 * rowsCount / pSize);//取天花板数
                // 调整当前页码不超过总页数
                crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                crtNo = crtNo <= 0 ? 1 : crtNo;
                //总记录数
                list = db.DispatchMessages.Where(x => x.showState == false).OrderBy(a => a.State).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                //总页数
                int pageCount = (rowsCount - 1) / pSize + 1;
                int nextPageNo = crtNo >= pageCount ? pageCount : crtNo + 1;//计算下一页页号
                int prevPageNo = crtNo == 1 ? 1 : crtNo - 1;//计算上一页页号
                ViewBag.NextPageNo = nextPageNo;
                ViewBag.PrevPageNo = prevPageNo;
                ViewBag.PageCount = pageCount;//总页数
                ViewBag.PageNo = crtNo;//当前页号
                ViewBag.SearchId = searchId;//搜索员工姓名
                List<int> listPage = new List<int>();
                for (int i = 1; i <= pageCount; i++)
                {
                    listPage.Add(i);
                }
                SelectList li = new SelectList(listPage, crtNo);
                ViewBag.PageList = li;
                ViewBag.task = list;
                return View();
            }
        }

      
        [HttpPost]
        public ActionResult ImportTask(HttpPostedFileBase file)
        {
            try
            {
                var tasks = new TaskFile(file.InputStream).ToTask();

                var db = new Context();

                List<string> existTasks = new List<string>();

                
                //IsDeviceNet为true设备网
                if (GlobalData.IsDeviceNet)
                {
                    foreach (var task in tasks)
                    {
                        var taskdb = db.DispatchMessages.SingleOrDefault(x => x.TaskNo == task.TaskNo);

                        if (taskdb != null)
                        {
                            //相同编号任务编号的同一版本不能重复导入
                            existTasks.Add($"{taskdb.TaskNo}({taskdb.PartNo})");
                            continue;
                        }

                        taskdb = new DispatchMessage
                        {
                            TaskNo = task.TaskNo,
                            PartNo = task.PartNo,
                            BatchNo = task.BatchNo,
                            SeqNo = task.SeqNo,
                            Count = task.Count,
                            FacCode = task.FacCode,
                            WeldNo = task.WeldNo,
                            WelderNo = task.WelderNo,
                            StartTime = task.StartTime,
                            EndTime = task.EndTime,
                            exportTime=task.exportTime,
                            importTime=task.importTime,
                            State = false,
                            showState=task.showState
                            
                        };                      


                        db.DispatchMessages.Add(taskdb);
                        db.SaveChanges();
                    }
                }
                

                if (existTasks.Any())
                    return Json(new { succeed = true, existTasks }, "text/html");
                else
                    return Json(new { succeed = true }, "text/html");
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false, error = e.Message }, "text/html");
            }
        }

        public ActionResult ExportTask(string[] taskId)
        {
            try
            {
                if (GlobalData.IsDeviceNet)//内网导出
                {
                    if (taskId != null)
                    {
                        var db = new Context();
                        var Tasks = db.DispatchMessages.Where(x => taskId.Contains(x.TaskNo)).ToArray();

                        var folder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Tasks");
                        if (!folder.Exists)
                        {
                            folder.Create();//如果文件夹不存在则创建一个新的
                        }
                        var fileName = Guid.NewGuid() + ".Task";
                        var name = "Tasks/" + fileName;
                        new TaskFile(Tasks).SaveAs(AppDomain.CurrentDomain.BaseDirectory + name);
                        foreach (var task in Tasks)
                        {
                            task.State =true;
                            task.importTime = DateTime.Now;
                            db.SaveChanges();
                        }
                        return Json(new { succeed = true, path = name, name = fileName });
                    }
                    else
                    {
                        return Json(new { succeed = false, error = "所选任务单号为空！" });
                    }
                }
                else
                {
                    return Json(new { succeed = false, error = "设备网不能导出！请检查是否网络是否设置成功！" });
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false, error = e.Message });
            }
            

        }
    }
}