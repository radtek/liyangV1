using WCAPP.Models;
using WCAPP.Types;
using WCAPP.Utils;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WCAPP.Models.Database;

namespace WCAPP.Sevices
{
    public class TestManager
    {
        public List<TestProc> ListTestProc(string userId)
        {
            var dict = new Dictionary<int, TestProc>();
            var db = new Context();

            var tests = from t in db.Test0s
                join w in db.Wpses
                    on t.WpsId equals w.Id
                //    where w.Author == userId
                select new {Test = t, Wps = w};

            foreach (var test in tests)
            {
                TestProc tp;
                if (!dict.TryGetValue(test.Wps.Id, out tp))
                {
                    tp = new TestProc()
                    {
                        WpsId = test.Wps.Id,
                        PartName = test.Wps.PartName,
                        PartNo = test.Wps.PartNo,
                        ProcName = test.Wps.ProcName,
                        ProcNo = test.Wps.ProcNo,
                        ProcStep = test.Wps.ProcStep,
                        TestState = test.Wps.TestState,
                        Tests = new List<Test0>()
                    };

                    dict.Add(test.Wps.Id, tp);
                }

                tp.Tests.Add(test.Test);
            }

            var list = dict.Values.ToList();
            foreach (var l in list)
            {
                l.Tests.Sort((x, y) => y.SeqNo - x.SeqNo);
            }

            return list;
        }

        public List<Wps> ListWpsWithoutTest(string userId)
        {
            var dict = new Dictionary<string, TestProc>();
            var db = new Context();

            var wpses = from w in db.Wpses where w.TestState == TestState.尚未进行 select w;

            return wpses.ToList();
        }

        public void AddTest(Test0 test)
        {
            var db = new Context();
            var wps = db.Wpses
                .Include("Tests")
                .SingleOrDefault(x => x.Id == test.WpsId);

            if (wps == null)
                throw new Exception("指定的工艺规程不存在");

            if (wps.ApprovalState == ApprovalState.审核通过)
                throw new Exception("指定的工艺规程已审核通过");

            if (wps.ApprovalState == ApprovalState.审核中)
                throw new Exception("指定的工艺规程正在审核");

            var currTest = wps.Tests != null ? wps.Tests.SingleOrDefault(x => x.SeqNo == wps.CurrTestSeqNo) : null;

            if (currTest != null)
            {
                if (!currTest.IsFinished)
                    throw new Exception("指定的工艺规程还有未完成的试焊");
                currTest.TestResult = TestResult.不合格;
            }

            if (wps.TestState != TestState.进行中)
            {
                wps.TestState = TestState.进行中;
            }

            if (wps.Tests == null)
                wps.Tests = new List<Test0>();

            test.CreateTime = DateTime.Now;
            test.SeqNo = currTest != null ? currTest.SeqNo + 1 : 1;

            wps.Tests.Add(test);
            wps.CurrTestSeqNo = test.SeqNo;
            db.SaveChanges();
        }

        public void FinishTest(int wpsId, string welderName, string checkerName, TestResult result)
        {
            var db = new Context();
            var wps = db.Wpses
                .Include(x => x.ParamList)
                .SingleOrDefault(x => x.Id == wpsId);

            if (wps == null)
                throw new Exception("指定的工艺规程不存在");

            var test = db.Test0s
                .Include(x => x.ParamList)
                .SingleOrDefault(x => x.SeqNo == wps.CurrTestSeqNo && x.WpsId == wpsId);

            if (test == null)
                throw new Exception("指定工艺规程尚未添加试焊");

            if (test.IsFinished)
                throw new Exception("当前试焊已完成");


            test.WelderName = welderName;
            test.FinishTime = DateTime.Now;
            test.TestResult = result;
            test.CheckerName = checkerName;
            test.IsFinished = true;

            foreach (var item in test.ParamList)
            {
                var destItem = wps.ParamList.Find(x => x.Enum == item.Enum);
                destItem.Value = item.Value;
            }

            var req = HttpContext.Current.Request;
            if (req.Files.Count > 0)
            {
                var file = req.Files[0].InputStream;
                test.TestReport = new byte[file.Length];
                file.Read(test.TestReport, 0, test.TestReport.Length);
            }

            db.SaveChanges();
        }

        public void FinishTestProc(int wpsId)
        {
            var db = new Context();
            var wps = db.Wpses.Include("Tests").SingleOrDefault(w => w.Id == wpsId);

            if (wps == null)
                throw new Exception("指定工艺规程不存在");

            var currTest = wps.Tests != null ? wps.Tests.SingleOrDefault(x => x.SeqNo == wps.CurrTestSeqNo) : null;

            if (currTest == null)
                throw new Exception("指定工艺规程还没有进行过试焊");

            if (!currTest.IsFinished)
                throw new Exception("指定工艺规程当前还有试焊未完成");

            if (currTest.TestResult != TestResult.合格)
                throw new Exception("指定工艺规程的试焊未成功");

            wps.TestState = TestState.已完成;
            db.SaveChanges();
        }

        public Wps QueryTestPorc(int wpsId)
        {
            var db = new Context();
            var wps = db.Wpses
                .SingleOrDefault(w => w.Id == wpsId);

            if (wps == null)
                throw new Exception("指定工艺规程不存在");

            wps.Tests = db.Test0s.Include(x => x.ParamList).Where(x => x.WpsId == wpsId).OrderByDescending(x => x.SeqNo)
                .ToList();

            return wps;
        }

        public void GetTestPdf(int wpsId, int seqNo)
        {
            HttpContext.Current.Response.ContentType = "application/pdf";
            var test = new Context().Test0s.SingleOrDefault(x => x.WpsId == wpsId && x.SeqNo == seqNo);

            if (test == null)
                throw new Exception("试焊记录不存在");

            if (test.TestReport == null)
                throw new Exception("该试焊记录不包含试验报告");

            HttpContext.Current.Response.BinaryWrite(test.TestReport);
        }
    }
}