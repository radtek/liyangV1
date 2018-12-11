using System.ComponentModel.DataAnnotations;
using WCAPP.Types;

namespace WCAPP.Models.UserModels
{
    public class ModifyUserModel
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public string Department { get; set; }
        public WorkPosition Position { get; set; }
        public Authority[] Authorities { get; set; }
        public int pageNo { get; set; }
    }
}