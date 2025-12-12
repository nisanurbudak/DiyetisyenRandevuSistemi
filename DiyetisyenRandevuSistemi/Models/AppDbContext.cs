using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DiyetisyenRandevuSistemi.Models
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

		public DbSet<User> Users { get; set; }
		public DbSet<Diyetisyen> Diyetisyenler { get; set; }
		public DbSet<HekimCalismaPlani> HekimCalismaPlanlari { get; set; }
		public DbSet<Abonelik> Randevular { get; set; }
		public DbSet<Paket> Paketler { get; set; }

		protected override void OnModelCreating(ModelBuilder mb)
		{
			base.OnModelCreating(mb);

			mb.Entity<Diyetisyen>().ToTable("Diyetisyenler");
			mb.Entity<User>().ToTable("Users");
			mb.Entity<Abonelik>().ToTable("Randevular");
			mb.Entity<HekimCalismaPlani>().ToTable("HekimCalismaPlanlari");

			// Diyetisyen - HekimCalismaPlani ilişkisi
			mb.Entity<HekimCalismaPlani>()
				.HasOne(h => h.Diyetisyen)
				.WithMany(d => d.CalismaPlanlari)
				.HasForeignKey(h => h.DiyetisyenId)
				.OnDelete(DeleteBehavior.Cascade);

			// Diyetisyen - Randevu ilişkisi
			mb.Entity<Abonelik>()
				.HasOne(r => r.Hekim)
				.WithMany(d => d.Randevular)
				.HasForeignKey(r => r.HekimId)
				.OnDelete(DeleteBehavior.Cascade);

			// ÇAKIŞMAYI DB SEVİYESİNDE ENGELLE
			mb.Entity<Abonelik>()
			  .HasIndex(r => new { r.HekimId, r.Baslangic })
			  .IsUnique();

			// Örnek seed: 5 hekim (login bilgileri eklendi!)
			mb.Entity<Diyetisyen>().HasData(
			  new Diyetisyen { Id = 1, Adi = "Dyt. Ayşe Yılmaz", Uzmanlik = "Kilo Kontrolü ve Sporcu Beslenmesi", Username = "ayse", Password = "12345", ResimUrl= "/İmages/diyetisyen1.jpg" },
			  new Diyetisyen { Id = 2, Adi = "Dyt. Mehmet Can", Uzmanlik = "Çocuk ve Ergen Beslenmesi", Username = "mehmet", Password = "12345" },
			  new Diyetisyen { Id = 3, Adi = "Dyt. Selin Koç", Uzmanlik = "Hastalıkta Beslenme", Username = "selin", Password = "12345" },
			  new Diyetisyen { Id = 4, Adi = "Dyt. Barış Kaya", Uzmanlik = "Gebelik ve Emzirme Dönemi Beslenme", Username = "baris", Password = "12345" },
			  new Diyetisyen { Id = 5, Adi = "Dyt. Elif Güneş", Uzmanlik = "Vejetaryen/Vegan Beslenme", Username = "elif", Password = "12345" }
			);

			// Örnek çalışma planı (Hafta içi 09:00-17:00, 30 dk slot)
			for (int hid = 1; hid <= 5; hid++)
			{
				int seedId = 1000 + hid * 10;
				mb.Entity<HekimCalismaPlani>().HasData(
				  new HekimCalismaPlani { Id = seedId + 1, DiyetisyenId = hid, Gun = DayOfWeek.Monday, Baslangic = new TimeSpan(9, 0, 0), Bitis = new TimeSpan(17, 0, 0), SlotDakika = 30 },
				  new HekimCalismaPlani { Id = seedId + 2, DiyetisyenId = hid, Gun = DayOfWeek.Tuesday, Baslangic = new TimeSpan(9, 0, 0), Bitis = new TimeSpan(17, 0, 0), SlotDakika = 30 },
				  new HekimCalismaPlani { Id = seedId + 3, DiyetisyenId = hid, Gun = DayOfWeek.Wednesday, Baslangic = new TimeSpan(9, 0, 0), Bitis = new TimeSpan(17, 0, 0), SlotDakika = 30 },
				  new HekimCalismaPlani { Id = seedId + 4, DiyetisyenId = hid, Gun = DayOfWeek.Thursday, Baslangic = new TimeSpan(9, 0, 0), Bitis = new TimeSpan(17, 0, 0), SlotDakika = 30 },
				  new HekimCalismaPlani { Id = seedId + 5, DiyetisyenId = hid, Gun = DayOfWeek.Friday, Baslangic = new TimeSpan(9, 0, 0), Bitis = new TimeSpan(17, 0, 0), SlotDakika = 30 }
				);
			}
		}
	}
}
