using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication.Models;

namespace ShopWebApplication.Controllers
{
    public class ClientInfosController : Controller
    {
        private readonly ShopContext _context;
        private readonly ICartRepository _cartRepository;

        public ClientInfosController(ShopContext context, ICartRepository cartRepository)
        {
            _context = context;
            _cartRepository = cartRepository;
        }

        // GET: ClientInfos
        public async Task<IActionResult> Index()
        {
            var shopContext = _context.ClientInfos.Include(c => c.User);
            return View(await shopContext.ToListAsync());
        }

        // GET: ClientInfos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientInfo = await _context.ClientInfos
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ClientInfoId == id);
            if (clientInfo == null)
            {
                return NotFound();
            }

            return View(clientInfo);
        }

        // GET: ClientInfos/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ClientInfos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientInfoId,UserId,FirstName,LastName,PhoneNumber,Email,Address")] ClientInfo clientInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clientInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", clientInfo.UserId);
            return View(clientInfo);
        }

        // GET: ClientInfos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientInfo = await _context.ClientInfos.FindAsync(id);
            if (clientInfo == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", clientInfo.UserId);
            return View(clientInfo);
        }

        // POST: ClientInfos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientInfoId,UserId,FirstName,LastName,PhoneNumber,Email,Address")] ClientInfo clientInfo)
        {
            if (id != clientInfo.ClientInfoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientInfoExists(clientInfo.ClientInfoId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", clientInfo.UserId);
            return View(clientInfo);
        }

        // GET: ClientInfos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientInfo = await _context.ClientInfos
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ClientInfoId == id);
            if (clientInfo == null)
            {
                return NotFound();
            }

            return View(clientInfo);
        }

        // POST: ClientInfos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientInfo = await _context.ClientInfos.FindAsync(id);
            if (clientInfo != null)
            {
                _context.ClientInfos.Remove(clientInfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientInfoExists(int id)
        {
            return _context.ClientInfos.Any(e => e.ClientInfoId == id);
        }

        //added by me
        // GET: ClientInfos/EnterClientInfo
        public IActionResult EnterClientInfo()
        {
            return View();
        }

        // POST: ClientInfos/EnterClientInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterClientInfo([Bind("UserId, FirstName,LastName,PhoneNumber,Email,Address")] ClientInfo clientInfo)
        {
            if (ModelState.IsValid)
            {
                _context.ClientInfos.Add(clientInfo);
                await _context.SaveChangesAsync(); 

                var success = await _cartRepository.MakePurchaseAsync();

                if (success)
                {
                    return RedirectToAction("Index", "Carts"); 
                }
                else
                {
                    return RedirectToAction("Index", "Home"); 
                }

            }

            if (!ModelState.IsValid)
            {
                // Log or handle validation errors
                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        // Log validation error to the console
                        Console.WriteLine($"Validation error: {error.ErrorMessage}");
                    }
                }

                // Return the view with validation errors
                return View(clientInfo);
            }

            return View(clientInfo);
        }
    }
}
