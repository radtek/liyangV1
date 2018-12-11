using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WCAPP.Types;

namespace WCAPP.Models.ProcedureModels
{
    public class AddSeamModel
    {
        [Required]
        [DisplayName("焊缝编号")]
        public string No { get; set; }
        [Required]
        [DisplayName("接头形式")]
        public JointForm JointForm { get; set; }
        [Required]
        [DisplayName("焊缝等级")]
        public SeamLevel SeamLevel { get; set; }
        [Required]
        [DisplayName("验收标准")]
        public string CheckStandard { get; set; }
        [Required]
        [DisplayName("材料牌号1")]
        public string Material1 { get; set; }
        [Required]
        [DisplayName("材料牌号2")]
        public string Material2 { get; set; }
        [DisplayName("材料牌号3")]
        public string Material3 { get; set; }
        [DisplayName("材料牌号4")]
        public string Material4 { get; set; }
        [Required]
        [DisplayName("材料厚度1")]
        public double Thick1 { get; set; }
        [Required]
        [DisplayName("材料厚度2")]
        public double Thick2 { get; set; }
        [DisplayName("材料厚度3")]
        public double? Thick3 { get; set; }
        [DisplayName("材料厚度4")]
        public double? Thick4 { get; set; }
        [DisplayName("焊缝间隙")]
        public double? Gap { get; set; }
        [DisplayName("焊机型别")]
        public string WeldMachineClass { get; set; }
        public string ElectrodeDiameter { get; set; }
    }
}