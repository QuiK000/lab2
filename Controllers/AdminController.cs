using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TaxiContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            TaxiContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Users(string? searchString)
        {
            ViewBag.CurrentFilter = searchString;

            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.FullName.Contains(searchString) ||
                                        u.Email!.Contains(searchString) ||
                                        u.PhoneNumber!.Contains(searchString));
            }

            var usersList = await users.ToListAsync();
            var usersViewModel = new List<UserManagementViewModel>();

            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var ordersCount = await _context.Orders.CountAsync(o => o.UserId == user.Id);

                usersViewModel.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber ?? "",
                    RegistrationDate = user.RegistrationDate,
                    Roles = roles.ToList(),
                    IsLocked = await _userManager.IsLockedOutAsync(user),
                    OrdersCount = ordersCount
                });
            }

            return View(usersViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();

            var model = new EditUserRoleViewModel
            {
                UserId = user.Id,
                UserName = user.FullName,
                CurrentRoles = currentRoles.ToList(),
                AllRoles = allRoles,
                SelectedRole = currentRoles.FirstOrDefault() ?? "Client"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, selectedRole);

            TempData["Success"] = "Роль успішно змінено";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = "Користувача розблоковано";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["Success"] = "Користувача заблоковано";
            }

            return RedirectToAction(nameof(Users));
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}
