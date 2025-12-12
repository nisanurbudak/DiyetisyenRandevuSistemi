using DiyetisyenRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DiyetisyenRandevuSistemi.Controllers
{
	public class AppointmentsController : Controller
	{
		private readonly AppDbContext _context;

		public AppointmentsController(AppDbContext context)
		{
			_context = context;
		}

		// 1️ Hekim seçme sayfası
		public async Task<IActionResult> Index()
		{
			var hekimler = await _context.Diyetisyenler.ToListAsync();
			return View(hekimler);
		}

		// 2️ Takvim gösterme
		public async Task<IActionResult> Calendar(int hekimId, DateTime? baslangic)
		{
			var hekim = await _context.Diyetisyenler
				.Include(h => h.CalismaPlanlari)
				.FirstOrDefaultAsync(h => h.Id == hekimId);

			if (hekim == null) return NotFound();

			// Seçilen tarihten itibaren 1 ayı getiriyoruz
			DateTime startDate = baslangic ?? DateTime.Today;
			DateTime endDate = startDate.AddMonths(1);

			var planlar = hekim.CalismaPlanlari.ToList();

			// Mevcut randevuları al
			//var doluRandevular = await _context.Randevular
			//.Where(r => r.HekimId == hekimId && r.Baslangic >= startDate && r.Baslangic <= endDate)
			//.ToListAsync();
			var doluRandevular = await _context.Randevular
                .Where(r => r.HekimId == hekimId
		           && r.Baslangic >= startDate
		           && r.Baslangic <= endDate
		           && (r.Durum == RandevuDurumu.Beklemede || r.Durum == RandevuDurumu.Onaylandi)) // 🔹 sadece aktif randevular
                .ToListAsync();


			var tumSlotlar = new System.Collections.Generic.List<(DateTime Baslangic, DateTime Bitis, bool Dolu)>();

			for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
			{
				var gunPlanlari = planlar.Where(p => p.Gun == d.DayOfWeek);
				foreach (var plan in gunPlanlari)
				{
					DateTime slotBas = d.Date + plan.Baslangic;
					while (slotBas + TimeSpan.FromMinutes(plan.SlotDakika) <= d.Date + plan.Bitis)
					{
						DateTime slotBit = slotBas.AddMinutes(plan.SlotDakika);
						bool dolu = doluRandevular.Any(r => r.Baslangic == slotBas);
						tumSlotlar.Add((slotBas, slotBit, dolu));
						slotBas = slotBit;
					}
				}
			}

			ViewBag.Hekim = hekim;
			return View(tumSlotlar);
		}

		// 3️ Onay sayfası (ilk girişte hekim seçimi yapılacak)
		public async Task<IActionResult> Create(int? hekimId, DateTime? baslangic)
		{
			if (hekimId == null || baslangic == null)
			{
				// Hekim listesi getir
				var hekimler = await _context.Diyetisyenler.ToListAsync();
				return View("SelectDoctor", hekimler); // ayrı view
			}

			var hekim = await _context.Diyetisyenler.FindAsync(hekimId);
			if (hekim == null) return NotFound();

			var plan = await _context.HekimCalismaPlanlari.FirstOrDefaultAsync(p =>
				p.DiyetisyenId == hekimId && p.Gun == baslangic.Value.DayOfWeek);

			if (plan == null) return BadRequest("Bu saatte çalışmıyor");

			var model = new Abonelik
			{
				HekimId = hekimId.Value,
				Baslangic = baslangic.Value,
				Bitis = baslangic.Value.AddMinutes(plan.SlotDakika)
			};

			ViewBag.Hekim = hekim;
			return View(model); // normal Create view
		}

		// 4️⃣ Randevu Kaydetme
		[HttpPost]
		public async Task<IActionResult> Create(Abonelik model)
		{
			var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdString))
				return Unauthorized();

			int userId = int.Parse(userIdString);
			model.UserId = userId;

			model.OlusturmaZamani = DateTime.UtcNow;
			model.Durum = RandevuDurumu.Beklemede;

			// 🔹 DB'de aynı slotta iptal veya reddedilmiş randevu var mı kontrol et
			var eskiRandevu = await _context.Randevular
				.FirstOrDefaultAsync(r => r.HekimId == model.HekimId
									   && r.Baslangic == model.Baslangic
									   && (r.Durum == RandevuDurumu.IptalEdildi || r.Durum == RandevuDurumu.Reddedildi));

			if (eskiRandevu != null)
			{
				_context.Randevular.Remove(eskiRandevu); // varsa sil
				await _context.SaveChangesAsync();
			}

			// 🔹 Çakışma kontrolü (artık sadece aktif randevulara bakıyor)
			bool dolu = await _context.Randevular
				.AnyAsync(r => r.HekimId == model.HekimId && r.Baslangic == model.Baslangic);

			if (dolu)
			{
				var hekim = await _context.Diyetisyenler.FindAsync(model.HekimId);
				ViewBag.Hekim = hekim;

				ModelState.AddModelError("", "Bu saat dolu!");
				return View(model);
			}

			_context.Randevular.Add(model);
			await _context.SaveChangesAsync();

			return RedirectToAction("MyAppointments");
		}


		// 5️ Kullanıcı randevuları
		public async Task<IActionResult> MyAppointments()
		{
			var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userIdString))
				return Unauthorized(); // Giriş yapılmamışsa

			if (!int.TryParse(userIdString, out int userId))
				return BadRequest("Geçersiz kullanıcı bilgisi.");

			var randevular = await _context.Randevular
				.Include(r => r.Hekim)
				.Where(r => r.UserId == userId)
				.OrderByDescending(r => r.Baslangic)
				.ToListAsync();

			return View(randevular);
		}


		// 6️ Hekim/Admin randevuları
		public async Task<IActionResult> Manage(int hekimId)
		{
			var randevular = await _context.Randevular
				.Include(r => r.User)
				.Where(r => r.HekimId == hekimId)
				.OrderBy(r => r.Baslangic)
				.ToListAsync();

			ViewBag.HekimId = hekimId;
			return View(randevular);
		}

		[HttpPost]
		public async Task<IActionResult> Approve(int id)
		{
			var randevu = await _context.Randevular.FindAsync(id);
			if (randevu != null)
			{
				randevu.Durum = RandevuDurumu.Onaylandi;
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("Manage", new { hekimId = randevu.HekimId });
		}

		[HttpPost]
		public async Task<IActionResult> Reject(int id)
		{
			var randevu = await _context.Randevular.FindAsync(id);
			if (randevu != null)
			{
				int hekimId = randevu.HekimId; // önce al
				_context.Randevular.Remove(randevu); // tamamen sil
				await _context.SaveChangesAsync();
				return RedirectToAction("Manage", new { hekimId });
			}
			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> Cancel(int id)
		{
			var randevu = await _context.Randevular.FindAsync(id);
			if (randevu != null)
			{
				_context.Randevular.Remove(randevu); // 🔹 tamamen sil
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("MyAppointments");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var randevu = await _context.Randevular.FindAsync(id);
			if (randevu != null)
			{
				_context.Randevular.Remove(randevu);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("MyAppointments"); // kendi sayfa action ismine göre değiştir
		}

		public IActionResult Hekimler()
		{
			var diyetisyenler = new List<Diyetisyen>
	{
		new Diyetisyen
		{
			Id = 1,
			Adi = "Dyt. Ayşe Yılmaz",
			Uzmanlik = "Kilo Kontrolü ve Sporcu Beslenmesi",
			Aciklama = "Sağlıklı yaşam için sürdürülebilir beslenme planları hazırlıyorum.",
			ResimUrl = "/İmages/diyetisyen1.jpg"
		},
		new Diyetisyen
		{
			Id = 2,
			Adi = "Dyt. Mehmet Can",
			Uzmanlik = "Çocuk ve Ergen Beslenmesi",
			Aciklama = "Çocuklar için sağlıklı alışkanlıkları oyunlaştırarak öğretiyorum.",
			ResimUrl = "/İmages/diyetisyen2.jpg"
		},
		new Diyetisyen
		{
			Id = 3,
			Adi = "Dyt. Selin Koç",
			Uzmanlik = "Hastalıkta Beslenme (Diyabet, Tiroid vb.)",
			Aciklama = "Kronik hastalıklarda kişiye özel beslenme desteği sağlıyorum.",
			ResimUrl = "/İmages/diyetisyen3.jpg"
		},
		new Diyetisyen
		{
			Id = 4,
			Adi = "Uzm. Dyt. Barış Kaya",
			Uzmanlik = "Gebelik ve Emzirme Dönemi Beslenme",
			Aciklama = "Anne adaylarına sağlıklı ve bilinçli beslenme planları sunuyorum.",
			ResimUrl = "/İmages/diyetisyen4.jpg"
		},
		new Diyetisyen
		{
			Id = 5,
			Adi = "Dyt. Elif Güneş",
			Uzmanlik = "Vejetaryen/Vegan Beslenme",
			Aciklama = "Bitkisel beslenme tarzına uygun dengeli menüler oluşturuyorum.",
			ResimUrl = "/İmages/diyetisyen5.jpg"
		},
	};

			return View(diyetisyenler);
		}

		// 7️ Abone Ol
		[HttpPost]
		public async Task<IActionResult> Subscribe(int hekimId, PaketTipi tip, DayOfWeek? haftalikGun, int? aylikGun)
		{
			var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdString))
				return Unauthorized();

			int userId = int.Parse(userIdString);

			DateTime baslangic = DateTime.Today;

			if (tip == PaketTipi.Aylik && aylikGun.HasValue)
			{
				int gun = aylikGun.Value;
				if (gun < DateTime.Today.Day)
					baslangic = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(gun - 1);
				else
					baslangic = new DateTime(DateTime.Today.Year, DateTime.Today.Month, gun);
			}
			else if (tip == PaketTipi.Haftalik && haftalikGun.HasValue)
			{
				int diff = ((int)haftalikGun.Value - (int)DateTime.Today.DayOfWeek + 7) % 7;
				baslangic = DateTime.Today.AddDays(diff);
			}

			var paket = new Paket
			{
				UserId = userId,
				HekimId = hekimId,
				Tip = tip,
				HaftalikGun = haftalikGun,
				AylikGun = aylikGun,
				BaslangicTarihi = baslangic,
				Aktif = true
			};

			_context.Paketler.Add(paket);
			await _context.SaveChangesAsync();

			// 🔹 Paket kapsamındaki tüm randevuları oluştur
			int kacAy = 3; // örnek: 3 ay boyunca otomatik randevu oluştur
			DateTime endDate = baslangic.AddMonths(kacAy);

			var planlar = await _context.HekimCalismaPlanlari
				.Where(p => p.DiyetisyenId == hekimId)
				.ToListAsync();

			for (DateTime d = baslangic; d <= endDate; d = d.AddDays(1))
			{
				bool ekle = false;

				if (tip == PaketTipi.Aylik && aylikGun.HasValue)
				{
					if (d.Day == aylikGun.Value) ekle = true;
				}
				else if (tip == PaketTipi.Haftalik && haftalikGun.HasValue)
				{
					if (d.DayOfWeek == haftalikGun.Value) ekle = true;
				}

				if (ekle)
				{
					// Hekimin planına uygun saati al (varsayılan 11:00-11:30)
					var yeniRandevu = new Abonelik
					{
						UserId = userId,
						HekimId = hekimId,
						PaketId = paket.Id,
						Baslangic = d.AddHours(11),
						Bitis = d.AddHours(11).AddMinutes(30),
						Durum = RandevuDurumu.Beklemede,
						OlusturmaZamani = DateTime.UtcNow
					};

					// Aynı gün ve saat için randevu yoksa ekle
					bool varMi = await _context.Randevular.AnyAsync(r =>
						r.HekimId == hekimId &&
						r.Baslangic == yeniRandevu.Baslangic);

					if (!varMi)
						_context.Randevular.Add(yeniRandevu);
				}
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("MySubscriptions");
		}


		// 8️ Kullanıcının Aboneliklerini Listele
		public async Task<IActionResult> MySubscriptions()
		{
			var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdString))
				return Unauthorized();

			int userId = int.Parse(userIdString);

			var paketler = await _context.Paketler
				.Where(p => p.UserId == userId)
				.Include(p => p.Hekim)
				.Include(p => p.Randevular) // 🔹 Paket randevuları
				.ToListAsync();

			return View(paketler);
		}

		// 9️ Abonelik İptali
		[HttpPost]
		public async Task<IActionResult> CancelSubscription(int id)
		{
			var paket = await _context.Paketler
				.Include(p => p.Randevular)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (paket == null)
				return NotFound();

			// Pakete bağlı randevuları sil
			if (paket.Randevular != null && paket.Randevular.Any())
			{
				_context.Randevular.RemoveRange(paket.Randevular);
			}

			// Paketi pasif yap veya tamamen silebilirsin
			//paket.Aktif = false; // pasif yapmak
			_context.Paketler.Remove(paket); // tamamen silmek istersen bunu kullan

			await _context.SaveChangesAsync();

			return RedirectToAction("MySubscriptions");
		}


	}
}


