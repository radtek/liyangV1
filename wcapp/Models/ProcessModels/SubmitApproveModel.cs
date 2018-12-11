using System.ComponentModel.DataAnnotations;

namespace WCAPP.Models.ProcessModels
{
    public class SubmitApproveModel
    {
        [Required]
        public string ProoferId { get; set; }
        [Required]
        public string ApproverId { get; set; }
    }
}