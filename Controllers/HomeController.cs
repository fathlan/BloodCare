using Microsoft.AspNetCore.Mvc;

namespace BloodCare.Controllers
{
    // Landing page = akses publik, TIDAK ada [Authorize] di sini
    // supaya User Umum (tanpa login) tetap bisa buka halaman ini
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
