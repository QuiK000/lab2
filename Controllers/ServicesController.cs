using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ServicesController : Controller
    {
        private readonly TaxiContext _context;

        public ServicesController(TaxiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .Include(s => s.Orders)
                .ToListAsync();

            return View(services);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var services = await _context.Services
                .Include(s => s.Orders)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (services == null)
            {
                return NotFound();
            }

            return View(services);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,PricePerKm,BasePrice,CarType")] Service service)
        {
            _context.Add(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
