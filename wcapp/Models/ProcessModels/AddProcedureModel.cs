using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WCAPP.Types;

namespace WCAPP.Models.ProcessModels
{
    public class AddProcedureModel
    {
        [Required]
        [DisplayName("工序号")]
        public string No { get; set; }
        [Required]
        [DisplayName("工序名称")]
        public string Name { get; set; }
        [Required]
        [DisplayName("焊接方法")]
        public WeldMethod WeldMethod { get; set; }
        [Required]
        [DisplayName("焊接类型")]
        public WeldType WeldType { get; set; }
        [DisplayName("焊接方式")]
        public ResistType? ResistType { get; set; }
        [DisplayName("自动化程度")]
        public AutoLevel? AutoLevel { get; set; }
    }
}