using System.ComponentModel.DataAnnotations;
using WCAPP.Types;

namespace WCAPP.Models.ProcedureModels
{
    public class GenParamModel
    {
        [Required]
        public AutoLevel AutoLevel { get; set; }
        [Required]
        public CheckStandard CheckStandard { get; set; }
        [Required]
        public SeamLevel SeamLevel { get; set; }
        [Required]
        public JointForm JointForm { get; set; }

        [Required]
        public string Material1 { get; set; }
        [Required]
        public string Material2 { get; set; }
        [Required]
        public double Gap { get; set; }
        [Required]
        public double Thick1 { get; set; }
        [Required]
        public double Thick2 { get; set; }
        
        public double ElectrodeDeameter { get; set; }  // 电极直径/滚盘宽度
        public MaterialState? MaterialState { get; set; }
    }
}