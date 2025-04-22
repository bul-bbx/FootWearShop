
using FootFetish.Data;
using FootFetish.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var orders = _context.Orders.ToList(); 
        return View(orders);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Details(int id)
    {
        ViewData["OrderItems"] = new SelectList(_context.OrderItems, "ProductId", "Name");
        var order = _context.Orders
            .Include(o => o.OrderItems) 
            .FirstOrDefault(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cartItems = await _context.CartItems
            .Include(ci => ci.Product)
            .ToListAsync();

        var viewModel = new OrderViewModel
        {
            CartItems = cartItems,
            TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity)
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(OrderViewModel model)
    {
        var cartItems = await _context.CartItems.Include(c => c.Product).ToListAsync();

        if (cartItems.Count == 0)
        {
            ModelState.AddModelError("", "Cart is empty.");
            return View(model);
        }

        var order = new Order
        {
            PaymentMethod = model.PaymentMethod,
            TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity),
            Address = model.Address,
            DeliveryMethod = model.DeliveryMethod,
            OrderItems = cartItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price,
                ProductName = item.Product.Name
            }).ToList(),
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        };

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
    }

    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        return View(order);
    }

    public async Task<IActionResult> MyOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var orders = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }
}
