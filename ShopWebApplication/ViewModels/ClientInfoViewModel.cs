using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication.ViewModels
{
	public class ClientInfoViewModel
	{
        [Required(ErrorMessage = "Будь ласка, введіть своє ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Будь ласка, введіть своє прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Будь ласка, введіть свій номер телефону")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Будь ласка, введіть свою електронну адресу")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Будь ласка, введіть свою адресу")]
        public string Address { get; set; }
    }
}

