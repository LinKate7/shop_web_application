using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication;
using ShopWebApplication.Models;

public class CartRepository : ICartRepository
{
    private readonly ShopContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CartRepository> _logger;

    public CartRepository(ShopContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, ILogger<CartRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger;
    }

    public async Task<int> AddItemAsync(int productId, int quantity, int sizeId)
    {
        var userId = GetUserId();

        if (userId == null)
        {
            throw new Exception("User is not authenticated. Please log in or register.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cart = await GetCartAsync(userId);
            var product = _context.Products
                        .Include(p => p.ProductSizes)
                        .ThenInclude(ps => ps.Size)
                        .FirstOrDefault(p => p.ProductId == productId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
           
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == productId && ci.SizeId == sizeId); //added sizeId condition
            var selectedSize = product?.ProductSizes.FirstOrDefault(ps => ps.SizeId == sizeId)?.Size;

            if (cartItem != null)
            {
                cartItem.CartItemQuantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    SizeId = sizeId,
                    CartItemQuantity = quantity,
                    Size = selectedSize
           
                };
                _context.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync();
            UpdateTotalPrice(cart.CartId);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            //throw;
            throw new Exception("Failed to add item to the cart. Please try again later.", ex);
        }

        return await GetCartItemCountAsync(); //rem
    }

    public async Task<int> RemoveItemAsync(int productId)
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
            throw new Exception("user is not logged-in");

        var cart = await GetCartAsync(userId);
        if (cart == null)
        {
            throw new Exception("There is no such cart.");
        }

        var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == productId);
        if (cartItem == null)
        {
            throw new Exception("The cart is already empty.");
        }

        if (cartItem.CartItemQuantity > 1)
        {
            cartItem.CartItemQuantity -= 1;
        }
        else
        {
            _context.CartItems.Remove(cartItem);
        }

        UpdateTotalPrice(cart.CartId);
        await _context.SaveChangesAsync();

        return await GetCartItemCountAsync();
    }

    public async Task<Cart> GetCartAsync(string userId)
    {

        return await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<IEnumerable<Cart>> GetUserCartAsync() //changed to 
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Enumerable.Empty<Cart>();
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.ProductSizes)
            .ThenInclude(ps => ps.Size)
            .Where(c => c.UserId == userId)
            .ToListAsync();
        
        return cart;
    }

    public string GetUserId()
    {
        var user = _httpContextAccessor?.HttpContext?.User;
        return _userManager.GetUserId(user);
    }

    public async Task<int> GetCartItemCountAsync(string userId = "") //add string userId parameter
    {
        if(string.IsNullOrEmpty(userId))
        {
            userId = GetUserId();
        }
        var cartItemData = await _context.Carts
            .Where(c => c.UserId == userId)
            .SelectMany(c => c.CartItems)
            .ToListAsync();

        //return cartItemData.Sum(item => item.CartItemQuantity);
        return cartItemData.Count();
    }

    public void UpdateTotalPrice(int cartId)
    {
        var cartItems = _context.CartItems
                           .Where(ci => ci.CartId == cartId)
                           .Include(ci => ci.Product)
                           .ToList();


        foreach (var cartItem in cartItems)
        {
            if(cartItem.Product != null)
            {
                cartItem.TotalPrice = cartItem.CartItemQuantity * cartItem.Product.Price;
            }
        }

        _context.SaveChanges();

        var cart = _context.Carts.FirstOrDefault(c => c.CartId == cartId);
        if (cart != null)
        {
            cart.TotalPrice = cartItems.Sum(ci => ci.TotalPrice);
            _context.SaveChanges();
        }
    }

    public async Task<bool> MakePurchaseAsync()
    {
        _logger.LogInformation("Transaction started");
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("Щоб оформити замовлення, будь ласка, увійдіть або зареєструйтесь.");
            }

            var cart = await GetCartAsync(userId);
            if (cart is null)
            {
                throw new Exception("Invalid cart");
            }

            var clientInfoId = await _context.ClientInfos
                                .Where(ci => ci.UserId == userId)
                                .Select(ci => ci.ClientInfoId)
                                .FirstOrDefaultAsync();

            if (clientInfoId == 0)
            {
                throw new Exception("Щось пішло не так");
            }
            var cartItems = _context.CartItems.Where(ci => ci.CartId == cart.CartId).ToList();
            if (cartItems.Count() == 0)
            {
                throw new Exception("В корзині поки що немає товарів.");
            }
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Price = cart.TotalPrice,
                StatusId = 1,
                ClientInfoId = clientInfoId
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach(var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    OrderItemQuantity = item.CartItemQuantity,
                    SizeId = item.SizeId

                };
                _context.OrderItems.Add(orderItem);
            }
            _context.SaveChanges();

            //remove CartItems after creation of Order

            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("Transaction committed");
            return true;
        }

        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred during transaction");

            transaction.Rollback();

            _logger.LogInformation("Transaction rolled back");

            return false;
        }
    }
}
