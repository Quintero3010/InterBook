using System.Diagnostics;
using InterBook.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace InterBook.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        public IActionResult MisLibros()
        {
            return View();
        }

        public IActionResult PublicarLibro()
        {
            return View();
        }

        public IActionResult EditarLibro()
        {
            return View();
        }

        public IActionResult Explorar()
        {
            return View();
        }

        public IActionResult Intercambios()
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}