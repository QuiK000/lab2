
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class DriversController : Controller
    {
        private readonly TaxiContext _context;

        public DriversController(TaxiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, decimal? minRating, int? minExperience)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.MinRating = minRating;
            ViewBag.MinExperience = minExperience;

            var drivers = _context.Drivers.Include(d => d.Cars).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                drivers = drivers.Where(d => d.Name.Contains(searchString) ||
                                            d.Phone.Contains(searchString) ||
                                            d.LicenseNumber.Contains(searchString));
            }

            if (minRating.HasValue)
            {
                drivers = drivers.Where(d => d.Rating >= minRating.Value);
            }

            if (minExperience.HasValue)
            {
                drivers = drivers.Where(d => d.Experience >= minExperience.Value);
            }

            var result = await drivers.OrderByDescending(d => d.Rating).ToListAsync();
            return View(result);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var driver = await _context.Drivers
                .Include(d => d.Cars)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null) return NotFound();
            return View(driver);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Cars = await _context.Cars.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,LicenseNumber,Experience,Rating")] Driver driver, int[] selectedCars)
        {
            // Видаляємо помилки для навігаційної властивості
            ModelState.Remove("Cars");

            if (ModelState.IsValid)
            {
                _context.Add(driver);
                await _context.SaveChangesAsync();

                if (selectedCars != null && selectedCars.Length > 0)
                {
                    var cars = await _context.Cars
                        .Where(c => selectedCars.Contains(c.Id))
                        .ToListAsync();

                    foreach (var car in cars)
                    {
                        driver.Cars.Add(car);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Cars = await _context.Cars.ToListAsync();
            return View(driver);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var driver = await _context.Drivers
                .Include(d => d.Cars)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null) return NotFound();
            ViewBag.Cars = await _context.Cars.ToListAsync();
            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,LicenseNumber,Experience,Rating")] Driver driver, int[] selectedCars)
        {
            if (id != driver.Id) return NotFound();

            // Видаляємо помилки для навігаційної властивості
            ModelState.Remove("Cars");

            if (ModelState.IsValid)
            {
                try
                {
                    var driverFromDb = await _context.Drivers
                        .Include(d => d.Cars)
                        .FirstOrDefaultAsync(d => d.Id == id);

                    if (driverFromDb == null) return NotFound();

                    // Оновлюємо властивості
                    driverFromDb.Name = driver.Name;
                    driverFromDb.Phone = driver.Phone;
                    driverFromDb.LicenseNumber = driver.LicenseNumber;
                    driverFromDb.Experience = driver.Experience;
                    driverFromDb.Rating = driver.Rating;

                    // Оновлюємо автомобілі
                    var allCars = await _context.Cars.ToListAsync();
                    foreach (var car in allCars)
                    {
                        bool isSelected = selectedCars != null && selectedCars.Contains(car.Id);
                        bool isCurrentlyAssigned = driverFromDb.Cars.Any(c => c.Id == car.Id);

                        if (isSelected && !isCurrentlyAssigned)
                        {
                            driverFromDb.Cars.Add(car);
                        }
                        else if (!isSelected && isCurrentlyAssigned)
                        {
                            driverFromDb.Cars.Remove(car);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Cars = await _context.Cars.ToListAsync();
            return View(driver);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var driver = await _context.Drivers
                .Include(d => d.Cars)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null) return NotFound();
            return View(driver);
        }

        [Authorize(Roles = "Admin,Dispatcher")]
        public async Task<IActionResult> Statistics(int id)
        {
            var driver = await _context.Drivers
                .Include(d => d.Cars)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null) return NotFound();

            var orders = await _context.Orders
                .Where(o => o.AssignedDriverId == id)
                .ToListAsync();

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.DriverId == id)
                .OrderByDescending(r => r.CreatedDate)
                .Take(10)
                .ToListAsync();

            var last7Days = new List<DailyOrderStats>();
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                var dayOrders = orders.Where(o => o.OrderDate.Date == date && o.Status == "Завершено");
                last7Days.Add(new DailyOrderStats
                {
                    Date = date,
                    OrderCount = dayOrders.Count(),
                    Revenue = dayOrders.Sum(o => o.TotalPrice)
                });
            }

            var model = new DriverStatisticsViewModel
            {
                Driver = driver,
                TotalOrders = orders.Count,
                CompletedOrders = orders.Count(o => o.Status == "Завершено"),
                TotalEarnings = orders.Where(o => o.Status == "Завершено").Sum(o => o.TotalPrice),
                AverageRating = reviews.Any() ? (decimal)reviews.Average(r => r.Rating) : 0,
                TotalReviews = reviews.Count,
                Last7Days = last7Days,
                RecentReviews = reviews
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers
                .Include(d => d.Cars)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver != null)
            {
                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.Id == id);
        }
    }
}