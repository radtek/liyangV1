using System.ComponentModel.DataAnnotations;

namespace WCAPP.Models.HomeModels
{
    public class LoginModel
    {
        [Required]
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Password0 { get; set; }
        public string Password1 { get; set; }
        public bool IsAdmin { get; set; }
    }
}