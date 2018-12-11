using System.Collections.Generic;
using Newtonsoft.Json;
using WCAPP.Types;

namespace WCAPP.Models.ProcessModels
{
    public class PdmCreateModel
    {
        public string PdmId { get; set; }
        public string No { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }

        public List<PdmAddProcedureModel> Procedures { get; set; }
    }

    public class PdmAddProcedureModel
    {
        public string PdmId { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public WeldMethod? WeldMethod { get; set; }
        public WeldType? WeldType { get; set; }
        public ResistType? ResistType { get; set; }
        public AutoLevel? AutoLevel { get; set; }
    }

    class ProcSeq
    {
        public string id { get; set; }  //工序ID
        public string no { get; set; }  //工序号
        public string name { get; set; }  //工序名称
    }
    public class Seam2
    {
        public string CheckStandard { get; set; }//检验标准
        public double? Gap { get; set; }//焊缝间隙
        public SeamLevel SeamLevel { get; set; }//焊缝等级
        public JointForm JointForm { get; set; }//接头类型
        public string Material1 { get; set; }//材料牌号1
        public string Material2 { get; set; }//材料牌号2
        public string Material3 { get; set; }//材料牌号3
        public string Material4 { get; set; }//材料牌号3
        public double Thick1 { get; set; }//厚度1
        public double Thick2 { get; set; }//厚度2
        public double? Thick3 { get; set; }//厚度2
        public double? Thick4 { get; set; }//厚度2
        public string WeldMachineClass { get; set; }//电极直径
        public string ElectrodeDiameter { get; set; }//滚轮宽度
        public string SpecialReportFileNo { get; set; }//报告编号
        public Dictionary<string, string> dic { get; set; }  //参数表，key为参数名称，value为参数值（参数有可能为一个数字，有可能为一个字符串，有可能为一个数字范围，所以统一用String表示）
    }
    public class Seam1
    {
        public string seamNo { get; set; }   //焊缝编号
        [JsonProperty(PropertyName = "params")]
        public Dictionary<string, string> Params { get; set; }  //参数表，key为参数名称，value为参数值（参数有可能为一个数字，有可能为一个字符串，有可能为一个数字范围，所以统一用String表示）

        //public Dictionary<string, string> Dic { get; set; }
    }

    public class Procedure1
    {
        public string procId { get; set; }        //工序ID
        public string weldMethod { get; set; }    //焊接方法
        public List<Seam1> seams { get; set; }        //工序中所有焊缝的参数

        //public List<Seam2> seam2 { set; get; } //焊缝中的参数
    }

    public class Process1
    {
        public string processId { get; set; }  //工艺规程ID
        public List<Procedure1> procs { get; set; }  //工艺规程中所有工序的参数

    }

    public class PdmResult
    {
        public string process { get; set; }
        public string pdf { get; set; }
    }
}