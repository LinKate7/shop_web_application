
namespace ShopWebApplication.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string SizeName { get; set; } = null!;

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}
