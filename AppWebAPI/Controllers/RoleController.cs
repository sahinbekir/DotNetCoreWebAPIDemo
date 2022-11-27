using AppWebAPI.Models;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RoleController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet("rolelist")]
        public async Task<ActionResult> GetRole()
        {
            var values = _roleManager.Roles.ToList();

            return Ok(values);
        }
        [HttpPost("add")]
        public async Task<ActionResult> AddRole(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole role = new AppRole
                {
                    Name = model.name
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return Ok(model);
        }
        [HttpPut("update")]
        public async Task<ActionResult> UpdateRole(int id,RoleModel model)
        {
            var values = _roleManager.Roles.Where(x => x.Id == id).FirstOrDefault();
            values.Name = model.name;
            var result = await _roleManager.UpdateAsync(values);
            
            return Ok(model);
        }
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            var values = _roleManager.Roles.FirstOrDefault(x => x.Id == id);
            var result = await _roleManager.DeleteAsync(values);
            
            return NoContent();
        }
        [HttpGet("userrolelist")]
        public async Task<ActionResult> UserRoleList()
        {
            var values = _userManager.Users.ToList();
            return Ok(values);
        }
        public static List<RoleAssignModel> modelrole = new List<RoleAssignModel>();
        [HttpGet("giverole")]
        public async Task<ActionResult> AssignRole(int id)
        {

            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            var roles = _roleManager.Roles.ToList();
            var userroles = await _userManager.GetRolesAsync(user);
            modelrole = new List<RoleAssignModel>();
            foreach (var item in roles)
            {
                RoleAssignModel m = new RoleAssignModel();
                m.roleid = item.Id;
                m.name = item.Name;
                m.exists = userroles.Contains(item.Name);
                modelrole.Add(m);
            }


            return Ok(modelrole);

            
        }
        
        [HttpPost("giverole")]
        public async Task<IActionResult> AssignRole(int id,List<RoleAssignModel> model)
        {
            
            
           
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            
            foreach (var item in model)
            {
                if (item.exists)
                {
                    await _userManager.AddToRoleAsync(user, item.name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, item.name);
                }
            }
            return Ok(model);
        }
        


    }
}