using System;
using System.ComponentModel.DataAnnotations;

namespace DiyetisyenRandevuSistemi.Models
{
	public enum RandevuDurumu { Beklemede = 0, Onaylandi = 1, Reddedildi = 2, IptalEdildi = 3 }

	public class Abonelik
	{
		public int Id { get; set; }
		public int HekimId { get; set; }
		public int UserId { get; set; }
		public int? PaketId { get; set; }

		public DateTime Baslangic { get; set; } 
		public DateTime Bitis { get; set; }     

		public RandevuDurumu Durum { get; set; } = RandevuDurumu.Beklemede;
		public DateTime OlusturmaZamani { get; set; } = DateTime.UtcNow;

		// 🔹 Navigation Properties
		public Diyetisyen Hekim { get; set; }   
		public User User { get; set; }
		public Paket Paket { get; set; }
	}
}
