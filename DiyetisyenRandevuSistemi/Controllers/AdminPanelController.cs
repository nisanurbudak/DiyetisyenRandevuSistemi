using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DiyetisyenRandevuSistemi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;

namespace DiyetisyenRandevuSistemi.Controllers
{
	public class AdminPanelController : Controller
	{
		private readonly AppDbContext _ctx;

		public AdminPanelController(AppDbContext ctx)
		{
			_ctx = ctx;
		}

		// 🔹 Admin Paneli Ana Sayfa
		public IActionResult Index()
		{
			var adminAdi = HttpContext.Session.GetString("AdminAdi");
			if (string.IsNullOrEmpty(adminAdi))
				return RedirectToAction("Login", "Admin");

			ViewBag.AdminAdi = adminAdi;
			return View();
		}

		public async Task<IActionResult> Users()
		{
			var adminAdi = HttpContext.Session.GetString("AdminAdi");
			if (string.IsNullOrEmpty(adminAdi))
				return RedirectToAction("Login", "Admin");

			var users = await _ctx.Users.ToListAsync();
			return View(users);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _ctx.Users.FindAsync(id);
			if (user != null)
			{
				_ctx.Users.Remove(user);
				await _ctx.SaveChangesAsync();
			}
			return RedirectToAction("Users");
		}

		[HttpGet]
		public IActionResult AddUser()
		{
			return View();
		}

		[HttpPost]
		public IActionResult AddUser(User model)
		{
			if (ModelState.IsValid)
			{
				_ctx.Users.Add(model);
				_ctx.SaveChanges();
				return RedirectToAction("Users"); // Kullanıcı listesine dön
			}

			ViewBag.ErrorMessage = "Bilgileri kontrol edin.";
			return View(model);
		}


		// GET: Kullanıcıyı düzenleme sayfası
		[HttpGet]
		public async Task<IActionResult> EditUser(int id)
		{
			var user = await _ctx.Users.FindAsync(id);
			if (user == null) return NotFound();
			return View(user);
		}

		// POST: Düzenlenen kullanıcıyı kaydet
		[HttpPost]
		public async Task<IActionResult> EditUser(User model)
		{
			if (!ModelState.IsValid)
				return View(model);

			_ctx.Users.Update(model);
			await _ctx.SaveChangesAsync();
			return RedirectToAction("Users");
		}

		// 🔹 Randevu Listesi
		public async Task<IActionResult> Appointments()
		{
			var adminAdi = HttpContext.Session.GetString("AdminAdi");
			if (string.IsNullOrEmpty(adminAdi))
				return RedirectToAction("Login", "Admin");

			var randevular = await _ctx.Randevular
				.Include(r => r.User)
				.Include(r => r.Hekim)
				.OrderBy(r => r.Baslangic)
				.ToListAsync();

			return View(randevular);
		}

		// 🔹 Randevu Silme
		[HttpPost]
		public async Task<IActionResult> DeleteAppointment(int id)
		{
			var randevu = await _ctx.Randevular.FindAsync(id);
			if (randevu != null)
			{
				_ctx.Randevular.Remove(randevu);
				await _ctx.SaveChangesAsync();
			}
			return RedirectToAction("Appointments");
		}

		[HttpGet]
		public IActionResult AddAppointment()
		{
			ViewBag.Users = _ctx.Users.ToList();
			ViewBag.Hekimler = _ctx.Diyetisyenler.ToList();
			return View();
		}

		[HttpPost]
		public IActionResult AddAppointment(Abonelik model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					_ctx.Randevular.Add(model);
					_ctx.SaveChanges();
					return RedirectToAction("Appointments"); // Randevu listesine dön
				}
				catch (Exception ex)
				{
					ViewBag.ErrorMessage = "Randevu kaydedilemedi: " + ex.Message;
				}
			}

			// Tekrar select listeleri yükle
			ViewBag.Users = _ctx.Users.ToList();
			ViewBag.Hekimler = _ctx.Diyetisyenler.ToList();
			return View(model);
		}


		// 🔹 İstatistikler ve Raporlama
		public IActionResult Statistics()
		{
			var adminAdi = HttpContext.Session.GetString("AdminAdi");
			if (string.IsNullOrEmpty(adminAdi))
				return RedirectToAction("Login", "Admin");

			// Genel istatistikler
			ViewBag.TotalUsers = _ctx.Users.Count();
			ViewBag.TotalAppointments = _ctx.Randevular.Count();

			// 🔹 Randevuları DB'den çekiyoruz (C# tarafına)
			var randevular = _ctx.Randevular.AsNoTracking().ToList();

			// Gün bazında grupla ve liste oluştur
			var appointmentsByDay = randevular
				.GroupBy(r => r.Baslangic.Date)
				.Select(g => new
				{
					Day = g.Key.ToString("yyyy-MM-dd"),
					Count = g.Count()
				})
				.OrderBy(g => g.Day)
				.ToList();

			ViewBag.AppointmentsLabels = appointmentsByDay.Select(x => x.Day).ToList();
			ViewBag.AppointmentsData = appointmentsByDay.Select(x => x.Count).ToList();

			// Kullanıcı faaliyet raporu
			var users = _ctx.Users.AsNoTracking().ToList();
			var userActivity = users
				.Select(u => new
				{
					UserName = u.Name,
					AppointmentCount = randevular.Count(r => r.UserId == u.Id)
				})
				.ToList();

			ViewBag.UserLabels = userActivity.Select(x => x.UserName).ToList();
			ViewBag.UserData = userActivity.Select(x => x.AppointmentCount).ToList();

			return View();
		}

		// 🔹 Çıkış
		[HttpPost]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Admin");
		}
	}
}
