using WCAPP.Libs;
using WCAPP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WCAPP.Models.Database;

namespace WCAPP.Sevices
{
    public class SyncManager
    {
        /*
        public string SynchronToFile(int[] wpsIds)
        {
            var db = new Context();
            Dictionary<int, Wps> wpses;

            if (wpsIds == null)
            {
                wpses = db.Wpses
                    .Include(x => x.ParamList)
                    .Include("Tests.ParamList").ToDictionary(x => x.Id);
            }
            else
            {
                wpses = db.Wpses
                    .Include(x => x.ParamList)
                    .Include("Tests.ParamList")
                    .Where(x => wpsIds.Contains(x.Id))
                    .ToDictionary(x => x.Id);

                foreach (var id in wpsIds)
                {
                    Wps wps;
                    if (!wpses.TryGetValue(id, out wps))
                    {
                        throw new Exception("编号为" + id + "的工艺规程不存在");
                    }
                }
            }

            WcappFile WCAPPFile = new WcappFile(wpses.Values.ToArray());
            DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "SyncFiles");
            if (!dirInfo.Exists)
                dirInfo.Create();

            var fileName = "SyncFiles/" + Guid.NewGuid().ToString() + ".WCAPP";
            WCAPPFile.SaveAs(AppDomain.CurrentDomain.BaseDirectory + fileName);
            return fileName;
        }

        public string SynchronAllToFile()
        {
            return SynchronToFile(null);
        }

        // 为了减小数据同步的复杂度，此处鉴于内网和设备网提供的功能
        // 并不一样，所以不采用通用的同步方式，只考虑可能的数据流向，
        // 对设备网和内网的不同副本选择性同步数据。
        // 审核流程数据只在内网副本存在，无需同步（审核状态为WPS的属
        // 性，会随WPS数据同步）。
        public void SynchronFromFile()
        {
            var req = HttpContext.Current.Request;
            var file = req.Files[0];

            WcappFile WCAPPFile = new WcappFile(file.InputStream);

            var db = new Context();

            var wpses = WCAPPFile.ToProcesses();
            if (wpses == null)
                return;

            foreach (var wps in wpses)
            {
                var dbwps = db.Wpses
                    .Include(x => x.Tests)
                    .Include(x => x.ParamList)
                    .SingleOrDefault(x => x.Id == wps.Id);
                if (WCAPP.GlobalData.IsDeviceNet)
                {
                    // 当前副本运行在设备网中
                    // 试焊数据都由设备网副本管理，无需从内网副本中同步
                    // 只需要同步新增的WPS或者修改已有WPS的审核状态即可

                    if (dbwps == null)
                    {
                        db.Wpses.Add(wps);
                    }
                    else
                    {
                        dbwps.ApprovalState = wps.ApprovalState;
                    }
                }
                else
                {
                    // 当前副本运行在内网中
                    // 需要从设备网副本中同步试焊数据

                    // 正常情况下不可能设备网中存在的WPS在内网中不存在
                    if (dbwps == null)
                        throw new Exception("与设备网中WCAPP数据有冲突");

                    // 同步试焊状态和当前试焊
                    dbwps.TestState = wps.TestState;
                    dbwps.CurrTestSeqNo = wps.CurrTestSeqNo;

                    if (dbwps.Tests == null)
                    {
                        // 内网中不存在试焊数据，则复制设备网中所有试焊数据
                        dbwps.Tests = wps.Tests;
                    }
                    else
                    {
                        // 内网中存在试焊数据

                        // 正常情况下不可能内网中有试焊数据而设备网中没有
                        if (wps.Tests == null)
                        {
                            throw new Exception("同步文件数据有异常");
                        }

                        foreach (var item in wps.Tests)
                        {
                            var destItem = dbwps.Tests.Find(x => x.SeqNo == item.SeqNo);
                            if (destItem == null)
                            {
                                // 内网中不存在该条试焊数据，则添加
                                dbwps.Tests.Add(item);
                            }
                            else
                            {
                                // 内网中存在该条试焊数据，则同步试焊基本信息
                                // 由于试焊参数不可更改，因此无需再次同步

                                destItem.WelderName = item.WelderName;
                                destItem.FinishTime = item.FinishTime;
                                destItem.TestResult = item.TestResult;
                                destItem.IsFinished = item.IsFinished;
                            }
                        }
                    }

                    // 设备网中试焊结果会填到WPS的参数中，因此需要同步
                    foreach (var item in wps.ParamList)
                    {
                        var destItem = dbwps.ParamList.Find(x => x.Enum == item.Enum);
                        destItem.Value = item.Value;
                    }
                }
            }

            db.SaveChanges();
        }

        public bool IsSyncFileCorrect()
        {
            var req = HttpContext.Current.Request;
            var file = req.Files[0];
            return WcappFile.IsFileCorrect(file.InputStream);
        }*/
    }
}