using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly TaxiContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(TaxiContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int orderId)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.AssignedDriver)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && o.Status == "Завершено");

            if (order == null || order.AssignedDriver == null)
            {
                TempData["Error"] = "Замовлення не знайдено або ще не завершено";
                return RedirectToAction("Index", "Orders");
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.OrderId == orderId && r.UserId == userId);

            if (existingReview != null)
            {
                TempData["Error"] = "Ви вже залишили відгук для цього замовлення";
                return RedirectToAction("Index", "Orders");
            }

            var review = new Review
            {
                OrderId = orderId,
                DriverId = order.AssignedDriver.Id,
                UserId = userId
            };

            ViewBag.Order = order;
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == review.OrderId && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "Помилка створення відгуку";
                return RedirectToAction("Index", "Orders");
            }

            review.UserId = userId;
            review.CreatedDate = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await UpdateDriverRating(review.DriverId);

            TempData["Success"] = "Дякуємо за відгук!";
            return RedirectToAction("Index", "Orders");
        }

        private async Task UpdateDriverRating(int driverId)
        {
            var driver = await _context.Drivers.FindAsync(driverId);
            if (driver != null)
            {
                var avgRating = await _context.Reviews
                    .Where(r => r.DriverId == driverId)
                    .AverageAsync(r => (decimal)r.Rating);

                driver.Rating = Math.Round(avgRating, 2);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IActionResult> DriverReviews(int driverId)
        {
            var driver = await _context.Drivers.FindAsync(driverId);
            if (driver == null) return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Order)
                .Where(r => r.DriverId == driverId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            ViewBag.Driver = driver;
            return View(reviews);
        }
    }
} 