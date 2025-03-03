using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication.Models;

public partial class Product
{
    public int ProductId { get; set; }

    [Display(Name = "Назва")]
    public string ProductName { get; set; } = null!;

    [Display(Name = "Ціна")]
    public int Price { get; set; }

    [Display(Name = "Опис")]
    public string Description { get; set; } = null!;

    [Display(Name = "Категорія")]
    public int? CategoryId { get; set; }

   
    public string? ImageUrl { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [Display(Name = "Категорія")]
    public virtual Category? Category { get; set; }


    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [Display(Name = "Розміри")]
    [Required(ErrorMessage = "Будь ласка, оберіть розмір")]
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}
