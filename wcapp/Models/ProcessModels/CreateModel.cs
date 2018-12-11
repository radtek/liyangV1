using System.ComponentModel.DataAnnotations;

namespace WCAPP.Models.ProcessModels
{
    public class CreateModel
    {
        [Required]
        public string No { get; set; }
        [Required]
        public string PartNo { get; set; }
        [Required]
        public string PartName { get; set; }
    }
}