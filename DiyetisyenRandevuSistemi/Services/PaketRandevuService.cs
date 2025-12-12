using DiyetisyenRandevuSistemi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PaketRandevuService : BackgroundService
{
	private readonly IServiceProvider _services;

	public PaketRandevuService(IServiceProvider services)
	{
		_services = services;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using (var scope = _services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

				var aktifPaketler = await context.Paketler
					.Include(p => p.Hekim)
					.ToListAsync();

				DateTime bugun = DateTime.Today;

				foreach (var paket in aktifPaketler)
				{
					if (!paket.Aktif) continue;

					if (paket.Tip == PaketTipi.Haftalik && paket.HaftalikGun.HasValue)
					{
						if (bugun.DayOfWeek == paket.HaftalikGun.Value)
							await RandevuOlustur(context, paket, bugun);
					}
					else if (paket.Tip == PaketTipi.Aylik && paket.AylikGun.HasValue)
					{
						if (bugun.Day == paket.AylikGun.Value)
							await RandevuOlustur(context, paket, bugun);
					}
				}

				await context.SaveChangesAsync();
			}

			await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // her gün bir kere çalışsın
		}
	}

	private async Task RandevuOlustur(AppDbContext context, Paket paket, DateTime tarih)
	{
		// Aynı gün zaten randevu var mı kontrol et
		bool varMi = await context.Randevular.AnyAsync(r =>
			r.UserId == paket.UserId &&
			r.HekimId == paket.HekimId &&
			r.Baslangic.Date == tarih);

		if (!varMi)
		{
			var yeniRandevu = new Abonelik
			{
				UserId = paket.UserId,
				HekimId = paket.HekimId,
				Baslangic = tarih.AddHours(11),               // 🔹 11:00
				Bitis = tarih.AddHours(11).AddMinutes(30),    // 🔹 11:30
				Durum = RandevuDurumu.Beklemede,
				OlusturmaZamani = DateTime.UtcNow
			};

			context.Randevular.Add(yeniRandevu);
		}
	}

}
