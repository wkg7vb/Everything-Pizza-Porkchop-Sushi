using BAR.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

namespace BAR.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _context.ApplicationUsers.ToListAsync();
            return new ObjectResult(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(a => a.Id == id);
            return new ObjectResult(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ApplicationUser applicationuser)
        {
            _context.Add(applicationuser);
            await _context.SaveChangesAsync();
            return new ObjectResult(applicationuser.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(ApplicationUser applicationuser)
        {
            _context.Entry(applicationuser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = new ApplicationUser { Id = id };
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }   
}
