using Microsoft.AspNetCore.Mvc;
using DiyetisyenRandevuSistemi.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiyetisyenRandevuSistemi.Controllers
{
	public class AdminController : Controller
	{
		private readonly AppDbContext _ctx;

		public AdminController(AppDbContext ctx)
		{
			_ctx = ctx;
		}

		// 🔹 Giriş Sayfası
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		// 🔹 Giriş İşlemi
		[HttpPost]
		public IActionResult Login(LoginViewModel model, string role)
		{
			if (role == "Hekim")
			{
				var hekim = _ctx.Diyetisyenler
					.FirstOrDefault(d => d.Username == model.Username && d.Password == model.Password);

				if (hekim != null)
				{
					HttpContext.Session.SetInt32("HekimId", hekim.Id);
					HttpContext.Session.SetString("HekimAdi", hekim.Adi);
					return RedirectToAction("Index"); // Hekim paneli
				}
			}
			else if (role == "Admin")
			{
				// Admin bilgilerini burada sabit olarak kontrol edelim
				var adminUsername = "admin"; // Örnek admin kullanıcı adı
				var adminPassword = "12345"; // Örnek admin şifre

				if (model.Username == adminUsername && model.Password == adminPassword)
				{
					HttpContext.Session.SetString("AdminAdi", model.Username);
					return RedirectToAction("Index", "AdminPanel"); // Admin paneline yönlendir
				}
			}

			ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı.";
			return View(model);
		}

		// 🔹 Panel (Sadece giriş yapan hekimin randevuları)
		[HttpGet]
		public IActionResult Index()
		{
			int? hekimId = HttpContext.Session.GetInt32("HekimId");
			if (hekimId == null)
				return RedirectToAction("Login");

			var randevular = _ctx.Randevular
				.Where(r => r.HekimId == hekimId)
				   .Include(r => r.User)
				.OrderBy(r => r.Baslangic)
				.ToList();

			return View(randevular);
		}

		// 🔹 Randevu Onayla
		[HttpPost]
		public IActionResult Onayla(int id)
		{
			int? hekimId = HttpContext.Session.GetInt32("HekimId");
			if (hekimId == null) return RedirectToAction("Login");

			var randevu = _ctx.Randevular.FirstOrDefault(r => r.Id == id && r.HekimId == hekimId);
			if (randevu != null)
			{
				randevu.Durum = RandevuDurumu.Onaylandi;
				_ctx.SaveChanges();
			}
			return RedirectToAction("Index");
		}

		// 🔹 Randevu Reddet
		[HttpPost]
		public IActionResult Reddet(int id)
		{
			int? hekimId = HttpContext.Session.GetInt32("HekimId");
			if (hekimId == null) return RedirectToAction("Login");

			var randevu = _ctx.Randevular.FirstOrDefault(r => r.Id == id && r.HekimId == hekimId);
			if (randevu != null)
			{
				randevu.Durum = RandevuDurumu.Reddedildi;
				_ctx.SaveChanges();
			}
			return RedirectToAction("Index");
		}

		// 🔹 Çıkış
		[HttpPost]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var randevu = await _ctx.Randevular.FindAsync(id);
			if (randevu != null)
			{
				_ctx.Randevular.Remove(randevu);
				await _ctx.SaveChangesAsync();
			}
			return RedirectToAction("Manage", new { hekimId = randevu?.HekimId });
		}

		[HttpGet]
		public IActionResult Takvim()
		{
			int? hekimId = HttpContext.Session.GetInt32("HekimId");
			if (hekimId == null)
				return RedirectToAction("Login");

			// Onaylanmış randevuları al
			var randevular = _ctx.Randevular
				.Where(r => r.HekimId == hekimId && r.Durum == RandevuDurumu.Onaylandi)
				.Include(r => r.User) // <-- User bilgisi için Include
				.OrderBy(r => r.Baslangic)
				.ToList();

			// Günlere göre grupla
			var groupedByDate = randevular
				.GroupBy(r => r.Baslangic.Date)
				.OrderBy(g => g.Key)
				.ToDictionary(
					g => g.Key.ToString("yyyy-MM-dd"),
					g => g.Select(s => new
					{
						Baslangic = s.Baslangic.ToString("yyyy-MM-ddTHH:mm:ss"),
						Bitis = s.Bitis.ToString("yyyy-MM-ddTHH:mm:ss"),
						Durum = s.Durum.ToString(),
						User = new { Name = s.User != null ? s.User.Name : "Bilinmiyor" } // <-- Hasta adı eklendi
					}).ToList()
				);

			ViewBag.GroupedRandevular = groupedByDate;
			return View();
		}


		// 1️⃣ Hekim için abonelik listesi
		public async Task<IActionResult> Paketler(int? hekimId)
		{
			// Başlangıçta tüm paketleri al, User ve Randevular ile birlikte
			IQueryable<Paket> query = _ctx.Paketler
				.Include(p => p.User)
				.Include(p => p.Randevular)
				.Include(p => p.Hekim);

			// Eğer hekimId verilmişse sadece o hekimin paketlerini filtrele
			if (hekimId.HasValue && hekimId.Value > 0)
				query = query.Where(p => p.HekimId == hekimId.Value);

			var paketler = await query.ToListAsync();

			// Opsiyonel: Navbar veya view’da hangi hekime ait olduğunu göstermek için
			ViewBag.HekimId = hekimId;

			return View(paketler);
		}

		[HttpPost]
		public async Task<IActionResult> ApprovePaket(int paketId)
		{
			var paket = await _ctx.Paketler
				.Include(p => p.Randevular)
				.FirstOrDefaultAsync(p => p.Id == paketId);

			if (paket == null) return NotFound();

			foreach (var r in paket.Randevular)
				r.Durum = RandevuDurumu.Onaylandi;

			paket.Aktif = true;
			await _ctx.SaveChangesAsync();

			return Ok(); // Ajax için yeterli
		}

		[HttpPost]
		public async Task<IActionResult> RejectPaket(int paketId)
		{
			var paket = await _ctx.Paketler
				.Include(p => p.Randevular)
				.FirstOrDefaultAsync(p => p.Id == paketId);

			if (paket == null) return NotFound();

			foreach (var r in paket.Randevular)
				r.Durum = RandevuDurumu.Reddedildi;

			await _ctx.SaveChangesAsync();

			return Ok(); // Ajax için yeterli
		}

	}
}
