using System.ComponentModel.DataAnnotations;
namespace DiyetisyenRandevuSistemi.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Ad gerekli")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email gerekli")]
		[EmailAddress]
		public string Email { get; set; }

		[Required(ErrorMessage = "Şifre gerekli")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

	}
}