using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repositories;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
    public class EfUserRepository : GenericRepository<AppUser>, IUserDal
    {
        public List<AppUser> GetLisUser()
        {
            using (var c = new Context())
            {
                var userroleid = c.Roles.Where(x => x.Name == "User").Select(y => y.Id).FirstOrDefault();
                var all = c.UserRoles.ToList();
                var users = new List<AppUser>();
                for (var i = 0; i < all.Count(); i++)
                {

                    if (userroleid == all[i].RoleId)
                    {
                        var itemid = all[i].UserId;
                        var item = c.Users.Where(x=>x.Id==itemid).FirstOrDefault();
                        users.Add(item);
                    }
                }
                return users;
            }
        }
    }
}
