using System.ComponentModel.DataAnnotations;

namespace AppWebAPI.Models
{
    public class RoleModel
    {
        [Required(ErrorMessage = "Please Write to a Role Name")]
        public string name { get; set; }
    }
}
