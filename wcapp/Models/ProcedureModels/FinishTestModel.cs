using System.ComponentModel.DataAnnotations;

namespace WCAPP.Models.ProcedureModels
{
    public class FinishTestModel
    {
        [Required]
        public string Welder { get; set; }
        [Required]
        public string Checker { get; set; }
        [Required]
        public string ReportNo { get; set; }

        [Required]
        public byte[] bytePdf { get; set; }
    }
}