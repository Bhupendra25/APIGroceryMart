using GroceryStore.Data;
using GroceryStore.Models;
using GroceryStore.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GroceryStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly GroceryContext _context;
        private readonly IEmailService _emailService;


        public HomeController(GroceryContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        // [Authorize]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{productname}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct(string productname)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FirstOrDefaultAsync(p => p.PrName == productname);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [Authorize]
        [HttpPost("AddToCart")]
        public async Task<ActionResult> AddToCart(AddToCart item)
        {

            var product = await _context.Products.FirstOrDefaultAsync(p => p.PrName == item.ProductName);
            if (product == null)
            {
                return Ok("Product does not exist!!!");
            }
            var UID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Check if the user already has the product in their cart
            var existingCartItem = await _context.UserCarts.FirstOrDefaultAsync(c => c.UserID == UID && c.prName == item.ProductName);
            if (existingCartItem != null)
            {
                // If the product already exists, update the quantity and amount
                existingCartItem.Quantity += item.Quantity;
                existingCartItem.Amount += item.Quantity * product.Price;
            }
            else
            {
                // If the product is not in the cart, create a new cart item
                var newCartItem = new UserCart()
                {
                    prName = item.ProductName,
                    Quantity = item.Quantity,
                    Amount = item.Quantity * product.Price,
                    UserID = UID
                };
                await _context.UserCarts.AddAsync(newCartItem);
            }
            await _context.SaveChangesAsync();
            return Ok();

        }

        [Authorize]
        [HttpDelete("DeleteFromCart/{prName}")]
        public async Task<ActionResult> DeleteFromCart(string prName)
        {
            var UID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cartItem = await _context.UserCarts.FirstOrDefaultAsync(c => c.prName == prName && c.UserID == UID);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.UserCarts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("UpdateCart")]
        public async Task<IActionResult> PutProduct(int qty, string prN)
        {
            // Get the user ID
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Find the user's cart
            var userCart = await _context.UserCarts
                .FirstOrDefaultAsync(c => c.UserID == userId && c.prName == prN);

            if (userCart == null)
            {
                return NotFound(); // or any appropriate response if the cart doesn't exist
            }
            //get product price
            var product = await _context.Products.FirstOrDefaultAsync(p => p.PrName == prN);
            // Update the quantity
            userCart.Quantity = qty;
            userCart.Amount = qty * product.Price;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok();

        }

        [Authorize]
        /*  [HttpPost("BuyNow")]*/
        [HttpGet("BuyNow")]
        public async Task<ActionResult> BuyNow()
        {
            try
            {

                var UID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;



                var userCartItems = await _context.UserCarts.Where(u => u.UserID == UID).ToListAsync();
                if (userCartItems.Count == 0)
                {
                    return Ok("No items in cart! Please add something in your cart!!");
                }

                var products = (from cartItem in userCartItems
                                join product in _context.Products
                                on cartItem.prName equals product.PrName
                                select new
                                {
                                    prName = cartItem.prName,
                                    Quantity = cartItem.Quantity,
                                    Amount = cartItem.Amount,
                                    Price = product.Price
                                }).ToList();

                // Return list of products and total amount due
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        [Authorize]
        [HttpPost("Checkout")]
        public async Task<ActionResult> Checkout(string address)
        {
            var UID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userCartItems = await _context.UserCarts.Where(u => u.UserID == UID).ToListAsync();
            if (userCartItems.Count == 0)
            {
                return Ok("No items in cart! Please add something in your cart!!");
            }
            //Email
            var products = userCartItems.Select(c => c.prName).ToList();
            var UEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var mail = new Email
            {
                To = UEmail,
                Subject = "Order Placed Successfully",
                Body = $"Your order {string.Join(", ", products)}  has been recieved and will reach to you within 4 to 5 week Days on this address {address}"
            };
            _emailService.SendEmail(mail);

            // Clear user's cart
            _context.UserCarts.RemoveRange(userCartItems);
            await _context.SaveChangesAsync();

            foreach (var cartItem in userCartItems)
            {
                // Find corresponding product in Product table
                var product = await _context.Products.FirstOrDefaultAsync(p => p.PrName == cartItem.prName);

                if (product != null)
                {
                    // Subtract buynow quantity from product quantity
                    product.Quantity -= cartItem.Quantity;
                }
            }

            // Save changes to Product table
            await _context.SaveChangesAsync();


            return Ok("Order Successful!!");
        }
    }
}
