using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication.Models;

namespace ShopWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly ShopContext _context;

        public ChartController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet("countByCategory")]

        public async Task<IActionResult> GetCountByCategory()
        {
            var result = await _context.Products
                .GroupBy(product => product.Category.CategoryName)
                .Select(productGroup => new CountByCategoryResponseItem(productGroup.Key.ToString(), productGroup.Count())).ToListAsync();

            return new JsonResult(result);
        }

        [HttpGet("piechart-data")]
        public async Task<JsonResult> GetPieChartData()
        {
            var orders = await _context.Statuses
                .Include(s => s.Orders)
                .ToListAsync();

            var orderStatuses= new List<object>();
            orderStatuses.Add(new[] { "Стасус", "Кількість замовлень із цим статусом" });
            foreach (var c in orders)
            {
                orderStatuses.Add(new object[] { c.StatusName, c.Orders.Count() });
            }

            return new JsonResult(orderStatuses);
        }
        [HttpGet("popular-clothing-items")]
        public async Task<IActionResult> GetPopularClothingItems()
        {
            var popularItems = await _context.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => oi.Product.ProductName)
                .OrderByDescending(group => group.Count())
                .Select(group => new { ProductName = group.Key, OrderCount = group.Count() })
                .Take(3)
                .ToListAsync();

            return new JsonResult(popularItems);
        }

    }
}
