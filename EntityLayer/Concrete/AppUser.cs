using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class AppUser:IdentityUser<int>
    {
        public string? FullName { get; set; }
        public string? ImgUrl { get; set; }
        public string? MovieUrl { get; set; }
    }
}
