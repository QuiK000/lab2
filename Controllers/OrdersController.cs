using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TaxiContext _context;

        public OrdersController(TaxiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Service)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {

            var order = await _context.Orders
                .Include(o => o.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(order);
        }

        public async Task<IActionResult> Create(int? serviceId)
        {
            ViewBag.Services = new SelectList(await _context.Services.ToListAsync(), "Id", "Name", serviceId);

            var order = new Order();
            if (serviceId.HasValue)
            {
                order.ServiceId = serviceId.Value;
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,CustomerName,Phone,PickupAddress,DestinationAddress,Distance,ServiceId")] Order order)
        {
            if (ModelState.IsValid)
            {
                var service = await _context.Services.FindAsync(order.ServiceId);
                if (service != null)
                {
                    order.TotalPrice = service.BasePrice + (service.PricePerKm * order.Distance);
                    order.OrderDate = DateTime.Now;
                    order.Status = "Нове";

                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Success), new { id = order.Id });
                }
            }

            ViewBag.Services = new SelectList(await _context.Services.ToListAsync(), "Id", "Name", order.ServiceId);
            return View(order);
        }

        public async Task<IActionResult> Success(int? id)
        {

            var order = await _context.Orders
                .Include(o => o.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(order);
        }
    }
}
