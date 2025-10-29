using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class OrderManagementController : Controller
    {
        private readonly TaxiContext _context;

        public OrderManagementController(TaxiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? status, string? search, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders
                .Include(o => o.Service)
                .Include(o => o.AssignedDriver)
                .Include(o => o.AssignedCar)
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                    o.CustomerName.Contains(search) ||
                    o.Phone.Contains(search) ||
                    o.Id.ToString().Contains(search));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                var endDate = toDate.Value.AddDays(1);
                query = query.Where(o => o.OrderDate < endDate);
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.Statuses = new[] { "Нове", "Виконується", "Завершено", "Скасовано" };
            ViewBag.CurrentStatus = status;
            ViewBag.Search = search;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(orders);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Dispatcher")]
        public async Task<IActionResult> ExportToCSV(string? status, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders
                .Include(o => o.Service)
                .Include(o => o.AssignedDriver)
                .Include(o => o.AssignedCar)
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate >= fromDate.Value);

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.AddDays(1);
                query = query.Where(o => o.OrderDate < endDate);
            }

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("№,Дата,Клієнт,Телефон,Послуга,Водій,Автомобіль,Від,До,Відстань,Вартість,Статус");

            foreach (var order in orders)
            {
                csv.AppendLine($"{order.Id}," +
                    $"{order.OrderDate:dd.MM.yyyy HH:mm}," +
                    $"\"{order.CustomerName}\"," +
                    $"{order.Phone}," +
                    $"\"{order.Service?.Name}\"," +
                    $"\"{order.AssignedDriver?.Name ?? "Не призначено"}\"," +
                    $"\"{order.AssignedCar?.LicensePlate ?? "-"}\"," +
                    $"\"{order.PickupAddress}\"," +
                    $"\"{order.DestinationAddress}\"," +
                    $"{order.Distance}," +
                    $"{order.TotalPrice}," +
                    $"{order.Status}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"Orders_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            return File(bytes, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Service)
                .Include(o => o.AssignedDriver)
                .Include(o => o.AssignedCar)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Drivers = new SelectList(
                await _context.Drivers.ToListAsync(),
                "Id", "Name",
                order.AssignedDriverId
            );

            ViewBag.Cars = new SelectList(
                await _context.Cars.Where(c => c.IsAvailable).ToListAsync(),
                "Id", "LicensePlate",
                order.AssignedCarId
            );

            ViewBag.Statuses = new[] { "Нове", "Виконується", "Завершено", "Скасовано" };

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(int id, string status, int? assignedDriverId, int? assignedCarId)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            order.AssignedDriverId = assignedDriverId;
            order.AssignedCarId = assignedCarId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Замовлення успішно оновлено!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> QuickAssign(int orderId, int driverId, int carId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Замовлення не знайдено" });
            }

            order.AssignedDriverId = driverId;
            order.AssignedCarId = carId;
            order.Status = "Виконується";

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Водія та автомобіль призначено" });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Замовлення не знайдено" });
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Статус змінено на '{newStatus}'" });
        }
    }
}