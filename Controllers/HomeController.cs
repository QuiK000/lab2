using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly TaxiContext _context;

        public HomeController(TaxiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        public async Task<IActionResult> Calculator(decimal? distance)
        {
            var services = await _context.Services.ToListAsync();
            var model = new PriceCalculatorViewModel
            {
                Services = services,
                Distance = distance ?? 0
            };

            if (distance.HasValue && distance > 0)
            {
                foreach (var service in services)
                {
                    var price = service.BasePrice + (service.PricePerKm * distance.Value);
                    model.CalculatedPrices[service.Id] = price;
                }
            }

            return View(model);
        }
    }
}
