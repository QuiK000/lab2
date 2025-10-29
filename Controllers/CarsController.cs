using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class CarsController : Controller
    {
        private readonly TaxiContext _context;

        public CarsController(TaxiContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string brand, int? year, bool? available)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.SelectedBrand = brand;
            ViewBag.SelectedYear = year;
            ViewBag.AvailableFilter = available;

            ViewBag.Brands = await _context.Cars
                .Select(c => c.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            var cars = _context.Cars.Include(c => c.Drivers).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c => c.Brand.Contains(searchString) ||
                                       c.Model.Contains(searchString) ||
                                       c.LicensePlate.Contains(searchString) ||
                                       c.Color.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                cars = cars.Where(c => c.Brand == brand);
            }

            if (year.HasValue)
            {
                cars = cars.Where(c => c.Year == year.Value);
            }

            if (available.HasValue)
            {
                cars = cars.Where(c => c.IsAvailable == available.Value);
            }

            var result = await cars.OrderBy(c => c.Brand).ThenBy(c => c.Model).ToListAsync();
            return View(result);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Drivers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,LicensePlate,Color,Year,BodyType,IsAvailable")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,LicensePlate,Color,Year,BodyType,IsAvailable")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Drivers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Drivers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}