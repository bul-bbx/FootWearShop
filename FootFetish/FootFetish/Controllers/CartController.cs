using FootFetish.Data;
using FootFetish.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<Cart> GetOrCreateCartAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }

    public async Task<IActionResult> Index()
    {
        var cart = await GetOrCreateCartAsync();
        return View(cart.Items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        var cart = await GetOrCreateCartAsync();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            item.Quantity += quantity;
        }
        else
        {
            CartItem ci = new CartItem
            {
                ProductId = productId,
                Cart = cart,
                Quantity = quantity,
                Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId)
            };
            cart.Items.Add(ci);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var item = await _context.CartItems.FindAsync(id);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantities(Dictionary<int, int> quantities)
    {
        var cart = await GetOrCreateCartAsync();

        foreach (var item in cart.Items)
        {
            if (quantities.TryGetValue(item.CartItemId, out int qty))
            {
                item.Quantity = qty > 0 ? qty : 1;
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ClearCart()
    {
        var cart = await GetOrCreateCartAsync();

        _context.CartItems.RemoveRange(cart.Items);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
