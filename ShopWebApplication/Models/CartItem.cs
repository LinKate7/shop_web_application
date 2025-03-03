namespace ShopWebApplication.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int CartItemQuantity { get; set; }

    public int TotalPrice { get; set; }

    public int SizeId { get; set; }

    public virtual Size Size { get; set; } = null!;

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
