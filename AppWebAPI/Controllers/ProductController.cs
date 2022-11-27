using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Configuration;

namespace AppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class ProductController : ControllerBase
    {

        ProductManager pm = new ProductManager(new EfProductRepository());


        [HttpGet("productlist")]
        public async Task<ActionResult> Get()
        {
            var name = User.Identity.Name;
            var name1 = "1";
            if (name1.IsNullOrEmpty()) { return BadRequest("didnt"); }
            else
            {
                Context c = new Context();

                var id = c.Users.Where(x => x.UserName == name).Select(y => y.Id).FirstOrDefault();

                var values = pm.GetProductListByUser(id);
                return Ok(values);
            }

        }
        [HttpPost("add")]
        public async Task<ActionResult> Add(Product p)
        {
            var name = User.Identity.Name;
            Context c = new Context();
            var userid = c.Users.Where(x => x.UserName == name).Select(y => y.Id).FirstOrDefault();
            p.ProductOwnerId = userid;
            pm.TAdd(p);

            return Ok(p);


        }
        [HttpPut("update")]
        public async Task<ActionResult> Update(Product p,int id)
        {
            var name = User.Identity.Name;
            Context c = new Context();
            var userid = c.Users.Where(x => x.UserName == name).Select(y => y.Id).FirstOrDefault();
            p.ProductId = id;
            p.ProductOwnerId = userid;
            pm.TUpdate(p);

            return Ok(p);


        }
        [HttpDelete("delete")]
        public async Task<ActionResult> Delete(int id)
        {
            var name = User.Identity.Name;
            Context c = new Context();
            var userid = c.Users.Where(x => x.UserName == name).Select(y => y.Id).FirstOrDefault();
            var p = c.Products.Where(x=>x.ProductId==id).FirstOrDefault();
            pm.TDelete(p);

            return Ok(p);


        }

    }
}
