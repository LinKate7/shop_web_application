using ShopWebApplication.Models;

namespace ShopWebApplication.Services
{
	public interface IImportService<TEntity> where TEntity : Category
	{
		Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
	}
}

