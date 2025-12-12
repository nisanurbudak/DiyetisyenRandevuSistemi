using Microsoft.AspNetCore.Mvc;
using DiyetisyenRandevuSistemi.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace DiyetisyenRandevuSistemi.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Aynı email ile kayıtlı kullanıcı var mı kontrol et
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Bu email ile zaten bir hesap var.");
                    return View(user);
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["Message"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Login(User user)
		{
			var existingUser = _context.Users
				.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

			if (existingUser != null)
			{
				// Kimlik bilgilerini oluştur
				var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, existingUser.Name),
			new Claim(ClaimTypes.Email, existingUser.Email),
			new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()) // burası değişti
        };

				var identity = new ClaimsIdentity(claims, "Login");
				var principal = new ClaimsPrincipal(identity);

				// Giriş yap
				await HttpContext.SignInAsync(principal);

				return RedirectToAction("Create", "Appointments");
			}

			ViewBag.Error = "Geçersiz giriş bilgisi!";
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Profile()
		{
			if (!User.Identity.IsAuthenticated)
				return RedirectToAction("Login");

			var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			var user = _context.Users.FirstOrDefault(u => u.Email == email);

			return View(user);
		}

		[HttpGet]
		public IActionResult EditProfile()
		{
			if (!User.Identity.IsAuthenticated)
				return RedirectToAction("Login");

			var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			var user = _context.Users.FirstOrDefault(u => u.Email == email);

			if (user == null)
				return NotFound();

			return View(user);
		}

		[HttpPost]
		public IActionResult EditProfile(IFormCollection form)
		{
			if (!User.Identity.IsAuthenticated)
				return RedirectToAction("Login");

			var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			var user = _context.Users.FirstOrDefault(u => u.Email == email);

			if (user == null)
				return NotFound();

			string newName = form["Name"];
			string currentPassword = form["CurrentPassword"];
			string newPassword = form["NewPassword"];

			// Ad güncelle
			user.Name = newName;

			// Şifre değiştirmek isteniyorsa kontrol yap
			if (!string.IsNullOrWhiteSpace(newPassword))
			{
				if (string.IsNullOrWhiteSpace(currentPassword))
				{
					ViewBag.Error = "Yeni şifre belirlemek için mevcut şifrenizi girin.";
					return View(user);
				}

				if (currentPassword != user.Password)
				{
					ViewBag.Error = "Mevcut şifreniz yanlış!";
					return View(user);
				}

				// Şifre doğruysa güncelle
				user.Password = newPassword;
			}

			_context.SaveChanges();

			ViewBag.Success = "Profil başarıyla güncellendi.";
			return View(user);
		}


	}
}
