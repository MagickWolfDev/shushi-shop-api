using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using shushi_shop_api.Data;
using shushi_shop_api.Models;

namespace shushi_shop_api.Controllers
{
    public class ProductTypesController : Controller
    {
        private readonly shushi_shop_apiContext _context;

        public ProductTypesController(shushi_shop_apiContext context)
        {
            _context = context;
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

        // GET: ProductTypes
        [Route("ProductTypes")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
              return _context.ProductType != null ? 
                          View(await _context.ProductType.ToListAsync()) :
                          Problem("Entity set 'shushi_shop_apiContext.ProductType'  is null.");
        }

        // GET: ProductTypes/Details/5
        [Route("ProductTypes/Detail/{id}")]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductType == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // GET: ProductTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("ProductTypes/Create")]
        [HttpPost]
        [Authorize(Roles = "admin, manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody][Bind("Id,Name")] ProductType productType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productType);
        }

        // GET: ProductTypes/Edit/5

        // POST: ProductTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("ProductTypes/Edit/{id}")]
        [Authorize(Roles = "admin, manager")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody][Bind("Id,Name")] ProductType productType)
        {
            if (id != productType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductTypeExists(productType.Id))
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
            return View(productType);
        }

        // GET: ProductTypes/Delete/5
        /*  public async Task<IActionResult> Delete(int? id)
          {
              if (id == null || _context.ProductType == null)
              {
                  return NotFound();
              }

              var productType = await _context.ProductType
                  .FirstOrDefaultAsync(m => m.Id == id);
              if (productType == null)
              {
                  return NotFound();
              }

              return View(productType);
          }*/

        // POST: ProductTypes/Delete/5
        [Route("ProductTypes/Delete/{id}")]
        [HttpDelete, ActionName("Delete")]
        [Authorize(Roles = "admin, manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductType == null)
            {
                return Problem("Entity set 'shushi_shop_apiContext.ProductType'  is null.");
            }
            var productType = await _context.ProductType.FindAsync(id);
            if (productType != null)
            {
                _context.ProductType.Remove(productType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductTypeExists(int id)
        {
          return (_context.ProductType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
