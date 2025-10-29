using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class DashboardController : Controller
    {
        private readonly TaxiContext _context;

        public DashboardController(TaxiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            var model = new DashboardViewModel
            {
                // Загальна статистика
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice),
                TotalDrivers = await _context.Drivers.CountAsync(),
                TotalCars = await _context.Cars.CountAsync(),

                TodayOrders = await _context.Orders.CountAsync(o => o.OrderDate.Date == today),
                TodayRevenue = await _context.Orders
                    .Where(o => o.OrderDate.Date == today)
                    .SumAsync(o => o.TotalPrice),
                MonthOrders = await _context.Orders
                    .CountAsync(o => o.OrderDate >= thisMonth),
                MonthRevenue = await _context.Orders
                    .Where(o => o.OrderDate >= thisMonth)
                    .SumAsync(o => o.TotalPrice),

                NewOrders = await _context.Orders.CountAsync(o => o.Status == "Нове"),
                InProgressOrders = await _context.Orders.CountAsync(o => o.Status == "Виконується"),
                CompletedOrders = await _context.Orders.CountAsync(o => o.Status == "Завершено"),
                CancelledOrders = await _context.Orders.CountAsync(o => o.Status == "Скасовано"),

                TopDrivers = await _context.Drivers
                    .OrderByDescending(d => d.Rating)
                    .Take(5)
                    .Select(d => new DriverStats
                    {
                        Name = d.Name,
                        Rating = d.Rating,
                        TotalOrders = _context.Orders.Count(o => o.AssignedDriverId == d.Id),
                        TotalRevenue = _context.Orders
                            .Where(o => o.AssignedDriverId == d.Id)
                            .Sum(o => o.TotalPrice)
                    })
                    .ToListAsync(),

                PopularServices = await _context.Services
                    .Select(s => new ServiceStats
                    {
                        Name = s.Name,
                        OrderCount = s.Orders.Count,
                        Revenue = s.Orders.Sum(o => o.TotalPrice)
                    })
                    .OrderByDescending(s => s.OrderCount)
                    .ToListAsync(),
                DailyStats = await GetDailyStats(7),

                RecentOrders = await _context.Orders
                    .Include(o => o.Service)
                    .Include(o => o.AssignedDriver)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }

        private async Task<List<DailyStats>> GetDailyStats(int days)
        {
            var startDate = DateTime.Today.AddDays(-days);
            var stats = new List<DailyStats>();

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var nextDate = date.AddDays(1);

                var orders = await _context.Orders
                    .Where(o => o.OrderDate >= date && o.OrderDate < nextDate)
                    .ToListAsync();

                stats.Add(new DailyStats
                {
                    Date = date,
                    OrderCount = orders.Count,
                    Revenue = orders.Sum(o => o.TotalPrice)
                });
            }

            return stats;
        }
    }
}