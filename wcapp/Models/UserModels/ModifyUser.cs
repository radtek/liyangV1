using System.ComponentModel.DataAnnotations;
using WCAPP.Types;

namespace WCAPP.Models.UserModels
{
    public class ModifyUserModels
    {
        [Required]
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string Depart { get; set; }
        public string Post { get; set; }
        public bool Authorized { get; set; }
        public bool Check { get; set; }
        public bool Management { get; set; }
    }
}