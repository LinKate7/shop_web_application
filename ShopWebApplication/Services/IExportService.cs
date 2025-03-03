using ShopWebApplication.Models;

namespace ShopWebApplication.Services
{
	public interface IExportService<TEntity> where TEntity : Category
	{
		Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
	}
}

