using System.ComponentModel.DataAnnotations;

namespace AppWebAPI.Models
{
    public class SignInModel
    {
        [Required(ErrorMessage = "Username write")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password write")]
        public string Password { get; set; }
    }
}
