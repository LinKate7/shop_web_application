using ShopWebApplication.Models;
namespace ShopWebApplication.Services
{
	public interface IDataPortServiceFactory<TEntity> where TEntity : Category
	{
		IImportService<TEntity> GetImportService(string contentType);
		IExportService<TEntity> GetExportService(string contentType);
	}
}

