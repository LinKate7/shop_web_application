using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication.Models;
using ShopWebApplication.Services;
using static ShopWebApplication.Exceptions.ImportException;

namespace ShopWebApplication.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ShopContext _context;
        private IDataPortServiceFactory<Category> _categoryDataPortServiceFactory;

        public CategoriesController(ShopContext context, IDataPortServiceFactory<Category> categoryDataPortServiceFactory)
        {
            _context = context;
            _categoryDataPortServiceFactory = categoryDataPortServiceFactory;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            //return View(category);
            ViewBag.CategoryName = category.CategoryName;
            return RedirectToAction("Index", "Products", new { id = category.CategoryId, name = category.CategoryName });

        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId, CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId, CategoryName")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken = default)
        {
            try
            {
                var importService = _categoryDataPortServiceFactory.GetImportService(fileExcel.ContentType);

                using var stream = fileExcel.OpenReadStream();

                await importService.ImportFromStreamAsync(stream, cancellationToken);

                return RedirectToAction(nameof(Index));
            }

            catch (InvalidPriceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            catch (InvalidSizeException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred during import";
            }

            return View();
            
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            CancellationToken cancellationToken = default)
        {
            var exportService = _categoryDataPortServiceFactory.GetExportService(contentType);

            var memoryStream = new MemoryStream();

            await exportService.WriteToAsync(memoryStream, cancellationToken);

            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;


            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"categories_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };

        }
    }
}
