using WCAPP.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WCAPP.Models.Database
{
    public class Test
    {
        public int Id { get; set; }

        [Required] public Seam Seam { get; set; }
        public bool IsFinished { get; set; }
        public string WelderName { get; set; }
        public ReportFile ReportFile { get; set; }
    }

    //public class Test0
    //{
    //    public int Id { get; set; }
    //    public Wps Wps { get; set; }

    //    [Index("TestSeq", IsUnique = true, Order = 0)]
    //    public int WpsId { get; set; }

    //    [Index("TestSeq", IsUnique = true, Order = 1)]
    //    public int SeqNo { get; set; }

    //    public string WelderName { get; set; }
    //    public string CheckerName { get; set; }
    //    public string TestReportNo { get; set; }
    //    public TestResult? TestResult { get; set; }
    //    public bool IsFinished { get; set; }
    //    public DateTime? CreateTime { get; set; }
    //    public DateTime? FinishTime { get; set; }

    //    [NotMapped]
    //    public Dictionary<string, string> Params
    //    {
    //        get { return ParamList == null ? null : ParamList.ToDictionary(p => p.Enum.ToString(), p => p.Value); }
    //        set
    //        {
    //            if (ParamList == null)
    //                ParamList = new List<TestParam>();

    //            var exsit = new HashSet<WeldParam>();

    //            foreach (var p in ParamList)
    //            {
    //                string v;
    //                exsit.Add(p.Enum);
    //                if (value.TryGetValue(p.Enum.ToString(), out v))
    //                    p.Value = v;
    //            }

    //            foreach (var p in value)
    //            {
    //                WeldParam wp;
    //                if (Enum.TryParse(p.Key, out wp))
    //                {
    //                    if (!exsit.Contains(wp))
    //                        ParamList.Add(new TestParam() {Enum = wp, Value = p.Value});
    //                }
    //            }
    //        }
    //    }

    //    public List<TestParam> ParamList { get; set; }

    //    /// <summary>
    //    /// 试验报告PDF
    //    /// </summary>
    //    public byte[] TestReport { get; set; }

    //    [NotMapped]
    //    public bool HasReport
    //    {
    //        get { return TestReport != null; }
    //    }

    //    public Test0(Wps wps)
    //    {
    //        WpsId = wps.Id;

    //        if (wps.ParamList != null)
    //        {
    //            ParamList = new List<TestParam>();
    //            foreach (var wp in wps.ParamList)
    //            {
    //                ParamList.Add(new TestParam
    //                {
    //                    Enum = wp.Enum,
    //                    Value = wp.Value
    //                });
    //            }
    //        }
    //    }

    //    public Test0()
    //    {
    //    }

    //    public class TestParam
    //    {
    //        public int Id { get; set; }

    //        public TestParam()
    //        {
    //        }

    //        public WeldParam Enum { get; set; }
    //        public string Value { get; set; }
    //    }
    //}


    //public class TestProc
    //{
    //    public int WpsId { get; set; }
    //    public string PartNo { get; set; }
    //    public string PartName { get; set; }
    //    public string ProcNo { get; set; }
    //    public string ProcName { get; set; }
    //    public string ProcStep { get; set; }
    //    public TestState TestState { get; set; }
    //    public List<Test0> Tests { get; set; }
    //}
}