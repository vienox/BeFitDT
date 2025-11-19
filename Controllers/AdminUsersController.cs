using BeFit.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeFit.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdminUsersController> _logger;

        public AdminUsersController(UserManager<IdentityUser> userManager, ILogger<AdminUsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AdminCreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            var targetRole = model.IsAdmin ? "Admin" : "User";
            var roleResult = await _userManager.AddToRoleAsync(newUser, targetRole);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await _userManager.DeleteAsync(newUser);
                return View(model);
            }

            TempData["StatusMessage"] = $"Użytkownik {model.Email} został utworzony.";
            _logger.LogInformation("Admin {Admin} created user {User} with role {Role}.",
                User?.Identity?.Name,
                model.Email,
                targetRole);

            return RedirectToAction(nameof(Create));
        }
    }
}
