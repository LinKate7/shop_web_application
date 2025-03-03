using ShopWebApplication.Models;

namespace ShopWebApplication;

public interface ICartRepository
{
	public Task<int> AddItemAsync(int productId, int quantity, int sizeId); //added sizeId and userId
	public Task<int> RemoveItemAsync(int productId);
	public Task<Cart> GetCartAsync(string userId);
	public Task<IEnumerable<Cart>> GetUserCartAsync(); //changed to ienumerable
	public Task<int> GetCartItemCountAsync(string userId = ""); 
	public Task<bool> MakePurchaseAsync();
}