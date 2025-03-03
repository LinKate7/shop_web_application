using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication.Models;

public partial class ClientInfo
{
    public int ClientInfoId { get; set; }

    public string UserId { get; set; }

    [Required(ErrorMessage = "Будь ласка, введіть своє ім'я")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Будь ласка, введіть своє прізвище")]
    public string LastName { get; set; } = null!;

    public string FullName => FirstName + " " + LastName;

    [Required(ErrorMessage = "Будь ласка, введіть свій номер телефону")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Будь ласка, введіть свою електронну адресу")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Будь ласка, введіть свою адресу")] 
    public string Address { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
