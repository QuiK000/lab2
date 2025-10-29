
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly TaxiContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(TaxiContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _context.Orders
                .Include(o => o.Service)
                .Include(o => o.AssignedDriver)
                .Include(o => o.AssignedCar)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.Service)
                .Include(o => o.AssignedDriver)
                .Include(o => o.AssignedCar)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        public async Task<IActionResult> Create(int? serviceId)
        {
            var services = await _context.Services.ToListAsync();
            ViewBag.Services = new SelectList(services, "Id", "Name", serviceId);

            var order = new Order();
            if (serviceId.HasValue)
            {
                order.ServiceId = serviceId.Value;
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerName,Phone,PickupAddress,DestinationAddress,Distance,ServiceId")] Order order)
        {
            ModelState.Remove("Service");
            ModelState.Remove("User");
            ModelState.Remove("AssignedDriver");
            ModelState.Remove("AssignedCar");

            if (ModelState.IsValid)
            {
                var service = await _context.Services.FindAsync(order.ServiceId);
                if (service != null)
                {
                    order.TotalPrice = service.BasePrice + (service.PricePerKm * order.Distance);
                    order.OrderDate = DateTime.Now;
                    order.Status = "Нове";
                    order.UserId = _userManager.GetUserId(User);

                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Success), new { id = order.Id });
                }
                else
                {
                    ModelState.AddModelError("ServiceId", "Послуга не знайдена");
                }
            }

            var services = await _context.Services.ToListAsync();
            ViewBag.Services = new SelectList(services, "Id", "Name", order.ServiceId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "Замовлення не знайдено";
                return RedirectToAction(nameof(Index));
            }

            if (order.Status != "Нове" && order.Status != "Виконується")
            {
                TempData["Error"] = "Це замовлення не можна скасувати";
                return RedirectToAction(nameof(Index));
            }

            order.Status = "Скасовано";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Замовлення успішно скасовано";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Success(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.Service)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}