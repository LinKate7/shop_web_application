using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication.Models;

namespace ShopWebApplication.Controllers;

public class CartsController : Controller
{
    private readonly ShopContext _context;
    private readonly ICartRepository _cartRepo;
    public CartsController(ShopContext context, ICartRepository cartRepository)
    {
        _context = context;
        _cartRepo = cartRepository;
    }

    // GET: Carts
    public async Task<IActionResult> Index()
    {
        var cart = await _cartRepo.GetUserCartAsync();
        var shopContext = _context.Carts.Include(c => c.User).Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.ProductSizes)
            .ThenInclude(ps => ps.Size);
        return View(cart);
            
    }

    // GET: Carts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cart = await _context.Carts
            .Include(c => c.User)
            .FirstOrDefaultAsync(m => m.CartId == id);
        if (cart == null)
        {
            return NotFound();
        }

        return View(cart);
    }

    // GET: Carts/Create
    public IActionResult Create()
    {
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
        return View();
    }

    // POST: Carts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CartId,UserId,TotalPrice")] Cart cart)
    {
        if (ModelState.IsValid)
        {
            _context.Add(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
        return View(cart);
    }

    // GET: Carts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cart = await _context.Carts.FindAsync(id);
        if (cart == null)
        {
            return NotFound();
        }
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
        return View(cart);
    }

    // POST: Carts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("CartId,UserId,TotalPrice")] Cart cart)
    {
        if (id != cart.CartId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(cart);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(cart.CartId))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
        return View(cart);
    }

    // GET: Carts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cart = await _context.Carts
            .Include(c => c.User)
            .FirstOrDefaultAsync(m => m.CartId == id);
        if (cart == null)
        {
            return NotFound();
        }

        return View(cart);
    }

    // POST: Carts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cart = await _context.Carts.FindAsync(id);
        if (cart != null)
        {
            _context.Carts.Remove(cart);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CartExists(int id)
    {
        return _context.Carts.Any(e => e.CartId == id);
    }

    // Cart actions results

    public async Task<IActionResult> AddItem(int productId, int sizeId, int quantity = 1, int redirect = 0) //added sizeId
    {
        var cartItemsCount = await _cartRepo.AddItemAsync(productId, quantity, sizeId);
            
        if(redirect == 0)
        {
            return Ok(cartItemsCount);
        }

        var cart = GetUserCart();
            
        return RedirectToAction("Index", cart);
    }

    public async Task<IActionResult> RemoveItem(int productId)
    {
        var cartItemsCount = await _cartRepo.RemoveItemAsync(productId);
        var cart = GetUserCart();
            
        return RedirectToAction("Index", cart);
    }

    private async Task<IEnumerable<Cart>> GetUserCart() //changed
    {
        return await _cartRepo.GetUserCartAsync();
    }
    public async Task<IActionResult> GetTotalItemNumberInCart() 
    {
        int itemsCount = await _cartRepo.GetCartItemCountAsync();
        return Ok(itemsCount);
    }

    public async Task<IActionResult> Checkout()
    {
        bool isCheckedOut = await _cartRepo.MakePurchaseAsync();
        if (!isCheckedOut)
            throw new Exception("Something went wrong.");
        return RedirectToAction("Index", "Home");
    }
}