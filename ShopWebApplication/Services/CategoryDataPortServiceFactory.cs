using ShopWebApplication.Models;


namespace ShopWebApplication.Services
{
    public class CategoryDataPortServiceFactory : IDataPortServiceFactory<Category>
    {
        private readonly ShopContext _context;
        public CategoryDataPortServiceFactory(ShopContext context)
        {
            _context = context;
        }

        public IImportService<Category> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new CategoryImportService(_context);
            }
            throw new NotImplementedException($"No import service implemented for products with content type {contentType}");
        }
        public IExportService<Category> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new CategoryExportService(_context);
            }
            throw new NotImplementedException($"No export service implemented for products with content type {contentType}");
        }
    }
}
