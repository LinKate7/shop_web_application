using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication.ViewModels;

public class RegisterViewModel
{
	[Required]
	[Display(Name = "Ім'я")]
	public string FirstName { get; set; }

	[Required]
	[Display(Name = "Прізвище")]
	public string LastName { get; set; }

	[Required]
	[Display(Name = "Email")]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[Display(Name = "Номер телефону")]
	public string PhoneNumber { get; set; }

	[Required]
	[Display(Name = "Пароль")]
	public string Password { get; set; }

	[Required]
	[Compare("Password", ErrorMessage = "Паролі не співпадають")]
	[Display(Name = "Підтвердження паролю")]
	[DataType(DataType.Password)]
	public string PasswordConfirm { get; set; }
}