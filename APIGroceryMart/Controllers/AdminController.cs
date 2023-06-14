using GroceryStore.Data;
using GroceryStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GroceryStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly GroceryContext _context;

        public AdminController(GroceryContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.ToListAsync();
        }
        [HttpGet("{id}")]

        public async Task<ActionResult<IEnumerable<Product>>> SearchProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [Route("AddProduct")]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (_context.Products.Where(n => n.PrName == product.PrName).Any())
            {
                return Ok("Product already Available");
            }
            else
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProduct), new { id = product.PrID }, product);
            }
        }

        [HttpPut]
        [Route("UpdateProduct")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.PrID)
            {
                return BadRequest();
            }
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }


            }
            return Ok();
        }

        private bool ProductAvailable(int id)
        {
            return (_context.Products?.Any(p => p.PrID == id)).GetValueOrDefault();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
}
