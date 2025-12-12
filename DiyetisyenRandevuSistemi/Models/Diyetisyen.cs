using System.ComponentModel.DataAnnotations;

namespace DiyetisyenRandevuSistemi.Models
{
	public class Diyetisyen
	{
		public int Id { get; set; }
		public string Adi { get; set; }
		public string Uzmanlik { get; set; }
		public string? Aciklama { get; set; }
		public string? ResimUrl { get; set; }

		// 🔑 Login için gerekli alanlar
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }

		// Navigation
		public ICollection<HekimCalismaPlani> CalismaPlanlari { get; set; }
		public ICollection<Abonelik> Randevular { get; set; }
	}

}
