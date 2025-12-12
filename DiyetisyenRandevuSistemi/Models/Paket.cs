using System;
using System.ComponentModel.DataAnnotations;

namespace DiyetisyenRandevuSistemi.Models
{
	public enum PaketTipi
	{
		Haftalik = 0,
		Aylik = 1
	}

	public class Paket
	{
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public int HekimId { get; set; }

		[Required]
		public PaketTipi Tip { get; set; }  // Haftalık mı Aylık mı

		public DayOfWeek? HaftalikGun { get; set; }  // Haftalık abonelikte hangi gün (örn: Cuma)
		public int? AylikGun { get; set; }           // Aylık abonelikte hangi gün (örn: 15)

		public DateTime BaslangicTarihi { get; set; } = DateTime.UtcNow;
		public bool Aktif { get; set; } = true;

		// 🔹 Navigation
		public User User { get; set; }
		public Diyetisyen Hekim { get; set; }
		public ICollection<Abonelik> Randevular { get; set; } = new List<Abonelik>();
	}
}
