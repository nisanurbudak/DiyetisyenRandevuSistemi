using DiyetisyenRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

public class HomeController : Controller
{
	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	public IActionResult KlinikHakkinda()
	{
		return View();
	}

	public IActionResult Hekimler()
	{
		var diyetisyenler = new List<Diyetisyen>
	{
		new Diyetisyen
		{
			Id = 1,
			Adi = "Dyt. Ayþe Yýlmaz",
			Uzmanlik = "Kilo Kontrolü ve Sporcu Beslenmesi",
			Aciklama = "Saðlýklý yaþam için sürdürülebilir beslenme planlarý hazýrlýyorum.",
			ResimUrl = "/Ýmages/diyetisyen1.jpg"
		},
		new Diyetisyen
		{
			Id = 2,
			Adi = "Dyt. Mehmet Can",
			Uzmanlik = "Çocuk ve Ergen Beslenmesi",
			Aciklama = "Çocuklar için saðlýklý alýþkanlýklarý oyunlaþtýrarak öðretiyorum.",
			ResimUrl = "/Ýmages/diyetisyen2.jpg"
		},
		new Diyetisyen
		{
			Id = 3,
			Adi = "Dyt. Selin Koç",
			Uzmanlik = "Hastalýkta Beslenme (Diyabet, Tiroid vb.)",
			Aciklama = "Kronik hastalýklarda kiþiye özel beslenme desteði saðlýyorum.",
			ResimUrl = "/Ýmages/diyetisyen3.jpg"
		},
		new Diyetisyen
		{
			Id = 4,
			Adi = "Uzm. Dyt. Barýþ Kaya",
			Uzmanlik = "Gebelik ve Emzirme Dönemi Beslenme",
			Aciklama = "Anne adaylarýna saðlýklý ve bilinçli beslenme planlarý sunuyorum.",
			ResimUrl = "/Ýmages/diyetisyen4.jpg"
		},
		new Diyetisyen
		{
			Id = 5,
			Adi = "Dyt. Elif Güneþ",
			Uzmanlik = "Vejetaryen/Vegan Beslenme",
			Aciklama = "Bitkisel beslenme tarzýna uygun dengeli menüler oluþturuyorum.",
			ResimUrl = "/Ýmages/diyetisyen5.jpg"
		},
	};

		return View(diyetisyenler);
	}

	public IActionResult RandevuAl()
	{
		if (User.Identity.IsAuthenticated) 
		{
			return View("RandevuAl"); 
		}
		else
		{
			return RedirectToAction("Login", "Account");
		}
	}

}
