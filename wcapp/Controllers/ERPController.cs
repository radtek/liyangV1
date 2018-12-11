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
    public class ERPController : Controller
    {
        public ActionResult Index(int? pageNo, string searchId, int? pageSize)
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

            if (searchId != null)
            {
                int rowsCount = db.ERPs.Where(x => x.ID.Contains(searchId) &&x.Kostl == "LYDL0032" && x.Eqart.Contains("2803")).Count();
                // int pCount = (int)Math.Ceiling(1.0 * rowsCount / pSize);//取天花板数
                // 调整当前页码不超过总页数
                int crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                crtNo = crtNo <= 0 ? 1 : crtNo;
                //总记录数
                List<ERP> list = db.ERPs.Where(x => x.ID.Contains(searchId)&&x.Kostl== "LYDL0032" && x.Eqart.Contains("2803")).OrderBy(a => a.importState).Skip((crtNo - 1) * pSize).Take(pSize).ToList();

                foreach (var lists in list)
                {
                    if (lists==null)
                    {
                        lists.importState = false;
                    }
                }
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
                ViewBag.ERP = list;
                return View();
            }
            else
            {
                int rowsCount = db.ERPs.Where(x => x.Kostl == "LYDL0032" && x.Eqart.Contains("2803")).Count();
                // int pCount = (int)Math.Ceiling(1.0 * rowsCount / pSize);//取天花板数
                // 调整当前页码不超过总页数
                int crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                crtNo = crtNo <= 0 ? 1 : crtNo;
                //总记录数
                List<ERP> list = db.ERPs.Where(x => x.Kostl == "LYDL0032"&&x.Eqart.Contains("2803")).OrderBy(a => a.importState).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                //总页数
                foreach (var lists in list)
                {
                    if (lists == null)
                    {
                        lists.importState = false;
                    }
                }
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
                ViewBag.ERP = list;
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImportERP(HttpPostedFileBase file)
        {
            try
            {
                var syncs = new SyncFile(file.InputStream).ToSync();

                var db = new Context();

                List<string> existSyncs = new List<string>();


                //IsDeviceNet为true设备网
                if (GlobalData.IsDeviceNet)
                {
                    foreach (var sync in syncs)
                    {
                        var syncdb = db.SynchroTables.SingleOrDefault(x => x.taskNo == sync.taskNo);

                        if (syncdb != null)
                        {
                            //相同编号任务编号的同一版本不能重复导入
                            existSyncs.Add($"{syncdb.taskNo}({syncdb.partNo})");
                            continue;
                        }

                        syncdb = new SynchroTable
                        {
                            taskNo = sync.taskNo,
                            partNo = sync.partNo,
                            batchNo = sync.batchNo,
                            seqNo = sync.seqNo,
                            countNum = sync.countNum,
                            facCode = sync.facCode,
                            weldNo = sync.weldNo,
                            welderNo = sync.welderNo,
                            status=sync.status,
                            startTime = sync.startTime,
                            endTime = sync.endTime,
                            realStartTime=sync.realStartTime,
                            realEndTime=sync.realEndTime,
                            weldTime=sync.weldTime
                        };

                        db.SynchroTables.Add(syncdb);
                        db.SaveChanges();
                    }
                }


                if (existSyncs.Any())
                    return Json(new { succeed = true, existSyncs }, "text/html");
                else
                    return Json(new { succeed = true }, "text/html");
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false, error = e.Message }, "text/html");
            }
        }
        public ActionResult ExportERP(string[] erpId)
        {
            try
            {
                if (GlobalData.IsDeviceNet)//内网导出
                {
                    if (erpId != null)
                    {
                        var db = new Context();
                        var ERPs = db.ERPs.Where(x => erpId.Contains(x.ID)).ToArray();

                        var folder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "ERPs");
                        if (!folder.Exists)
                        {
                            folder.Create();//如果文件夹不存在则创建一个新的
                        }
                        var fileName = Guid.NewGuid() + ".ERP";
                        var name = "ERPs/" + fileName;
                        new ERPFile(ERPs).SaveAs(AppDomain.CurrentDomain.BaseDirectory + name);
                        foreach (var erp in ERPs)
                        {
                            erp.importTime = DateTime.Now;
                            erp.importState = true;
                            db.SaveChanges();
                        }
                        return Json(new { succeed = true, path = name, name = fileName });
                    }
                    else
                    {
                        return Json(new { succeed = false, error = "所选任务编号为空！" });
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