using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication.Models;
using static ShopWebApplication.Exceptions.ImportException;

namespace ShopWebApplication.Services
{
	public class CategoryImportService : IImportService<Category>
	{
		private readonly ShopContext _context;
        public string PriceError { get; private set; }
        public List<string> importErrors { get; private set; }

        public CategoryImportService(ShopContext context)
		{
			_context = context;
			importErrors = new List<string>();
		}

		public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
		{
			if(!stream.CanRead)
			{
				throw new ArgumentException("Data cannot be read", nameof(stream));
			}

			using (XLWorkbook workbook = new XLWorkbook(stream))
			{
				var worksheet = workbook.Worksheets.FirstOrDefault();
				if(worksheet is null)
				{
					return;
				}
				foreach (var row in worksheet.RowsUsed().Skip(1))
				{		
	
					await AddProductAsync(row, cancellationToken);	
				}
			}
			await _context.SaveChangesAsync(cancellationToken);
		}

		private async Task AddProductAsync(IXLRow row, CancellationToken cancellationToken)
		{
			string categoryName = GetProductCategory(row);
			var category = await _context.Categories.FirstOrDefaultAsync(category => category.CategoryName == categoryName, cancellationToken);
			var product = new Product();
			product.ProductName = GetProductName(row);
            int price = GetProductPrice(row);
            if (price <= 0)
            {
                throw new InvalidPriceException($"Invalid price in row {row.RowNumber()}: Price cannot be negative or zero.");
            }
            product.Price = price;
            product.Description = GetProductDescription(row);
			product.Category = category;
			product.ImageUrl = GetProductImageURL(row);

			await GetSizesAsync(row, product, cancellationToken);
		}

		private static string GetProductCategory(IXLRow row)
		{
			return row.Cell(2).Value.ToString();
		}

		private static string GetProductName(IXLRow row)
		{
			return row.Cell(1).Value.ToString();
		}

		private static int GetProductPrice(IXLRow row)
		{
			return (int)row.Cell(3).Value;
		}

        private static string GetProductDescription(IXLRow row)
        {
            return row.Cell(4).Value.ToString();
        }

        private static string GetProductImageURL(IXLRow row)
        {
            return row.Cell(5).Value.ToString();
        }

		private async Task GetSizesAsync(IXLRow row, Product product, CancellationToken cancellationToken)
		{
            List<string> validSizes = new List<string> { "S", "M", "L" }; // Valid sizes
            for (int i = 6; i <= 8; i++)
			{
				if(row.Cell(i).Value.ToString().Length > 0)
				{
					var sizeName = row.Cell(i).Value.ToString();
                    if (!validSizes.Contains(sizeName)) //added
                    {
                        throw new InvalidSizeException($"Invalid size in row {row.RowNumber()} and column {i}: {sizeName}");
                    }
                    var size = await _context.Sizes.FirstOrDefaultAsync(s => s.SizeName == sizeName, cancellationToken);
					if(size == null)
					{
						size = new Size();
						size.SizeName = sizeName;
						_context.Sizes.Add(size);
					}

					ProductSize productSize = new ProductSize();
					productSize.Product = product;
					productSize.Size = size;

					_context.Add(productSize);
                }
			}
			await _context.SaveChangesAsync();
		}
    }
}

