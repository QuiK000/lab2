using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

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

        public async Task<IActionResult> Index()
        {
            var drivers = await _context.Drivers
                .Include(d => d.Cars)
                .ToListAsync();
            return View(drivers);
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
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,LicenseNumber,Experience,Rating")] Driver driver, int[] selectedCars)
        {
            _context.Add(driver);
            await _context.SaveChangesAsync();

            if (selectedCars != null && selectedCars.Length > 0)
            {
                var cars = await _context.Cars
                    .Where(c => selectedCars.Contains(c.Id))
                    .ToListAsync();

                foreach (var car in cars) driver.Cars.Add(car);
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,LicenseNumber,Experience,Rating")] Driver driver, int[] selectedCars)
        {
            if (id != driver.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    var driverFromDb = await _context.Drivers
                        .Include(d => d.Cars)
                        .FirstOrDefaultAsync(d => d.Id == id);

                    if (driverFromDb == null) return NotFound();

                    driverFromDb.Name = driver.Name;
                    driverFromDb.Phone = driver.Phone;
                    driverFromDb.LicenseNumber = driver.LicenseNumber;
                    driverFromDb.Experience = driver.Experience;
                    driverFromDb.Rating = driver.Rating;

                    var allCars = await _context.Cars.ToListAsync();
                    foreach (var car in allCars)
                    {
                        bool isSelected = selectedCars != null && selectedCars.Contains(car.Id);
                        bool isCurrentlyAssigned = driverFromDb.Cars.Any(c => c.Id == car.Id);

                        if (isSelected && !isCurrentlyAssigned) driverFromDb.Cars.Add(car);
                        else if (!isSelected && isCurrentlyAssigned) driverFromDb.Cars.Remove(car);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.Id)) return NotFound();
                    else throw;
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

        [HttpPost, ActionName("Delete")]
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