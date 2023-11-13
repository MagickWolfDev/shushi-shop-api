using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using shushi_shop_api.Data;
using shushi_shop_api.Middleware;
using shushi_shop_api.Models;

namespace shushi_shop_api.Controllers
{
    [EnableCors]
    public class UsersController : Controller
    {
        private readonly shushi_shop_apiContext _context;
        private readonly IConfiguration _config;

        public UsersController(shushi_shop_apiContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            
        }

        private User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new User
                {
                    Name = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }

        // POST: Users/Autorization
        [Route("Autorization")]
        [HttpPost]
        public async Task<IActionResult> Autorization([FromBody][Bind("Name, Password")] AutorizeData data)
        {
            var User = _context.User.Where(user => user.Name == data.Name).FirstOrDefault();

            if(User == null)
                return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(data.Password, User.Password))
                return BadRequest("password incorect");

            var generateJWT = new GenerateJWT(_config);


            return CreatedAtAction(nameof(Autorization),  new { token = generateJWT.NewToken(User)});
        }

        /*[Route("Loggout")]
        [HttpPost]

        public async Task<IActionResult> Loggout([FromBody][Bind("Id")] int id)
        {
            
        }*/

        // POST: Users/ChangePassword
        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody][Bind("Id, Password, NewPassword")] ChangePasswordData data)
        {
            return CreatedAtAction(nameof(Autorization), new { token = "12312312" });
        }

        // GET: Users
        [Route("Users")]
        //[Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            if(_context.User == null)
                return Problem("Entity set 'shushi_shop_apiContext.User'  is null.");

            var Users = await _context.User.ToListAsync();
            Response.Headers.AccessControlAllowOrigin = "*";
            Response.Headers.AccessControlAllowCredentials = "true";
            Response.Headers.AccessControlAllowMethods="GET,HEAD,OPTIONS,POST,PUT";
            Response.Headers.AccessControlAllowHeaders="Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers";

            //TODO Решить проблему с лишними полями данных

            /*var ProtectedUsers = new List<object>();

            for(int i = 0;  i < Users.Count; i++)
            {
                ProtectedUsers[i] = new { Id = Users[i].Id, Name = Users[i].Name, Role = Users[i].role };
            }*/
            var currentUser = this.GetCurrentUser();
            Console.WriteLine(currentUser.Name);

            return CreatedAtAction(nameof(Index), Users);
                          
        }

        // GET: Users/Details/5
        [Route("Users/Details/{id}")]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(Details), new { id = user.Id, name = user.Name, role = user.role});
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View("123");
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Users/Create")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody][Bind("Name,Role,Password")] User user)
        {
            if (ModelState.IsValid)
            {
               user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Details), new { id = user.Id, name = user.Name, role = user.role });
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Users/Edit/{id}")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,[FromBody][Bind("Name, role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5

        /*public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }*/

        // POST: Users/Delete/5
        [Route("Users/Delete/{id}")]
        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'shushi_shop_apiContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
