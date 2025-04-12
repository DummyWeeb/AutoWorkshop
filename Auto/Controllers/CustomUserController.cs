using Auto.Data;
using Auto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Auto.Controllers
{
    [Authorize(Roles = "IT")]
    public class CustomUserController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomUserController> _logger;

        public CustomUserController(UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ILogger<CustomUserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        // GET: CustomUser/Create
        public IActionResult Create()
        {
            ViewBag.PodrazdelenieId = new SelectList(_context.Podrazdelenies, "PodrazdelenieId", "PodrazdelenieName");
            return View();
        }

        // POST: CustomUser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomUser model, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new CustomUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Surname = model.Surname,
                    Ima = model.Ima,
                    SecSurname = model.SecSurname,
                    Age = model.Age,
                    PodrazdelenieId = model.PodrazdelenieId
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Добавление пользователя в роль
                    await _userManager.AddToRoleAsync(user, "User");

                    // Оставляем текущего пользователя в системе
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.PodrazdelenieId = new SelectList(_context.Podrazdelenies, "PodrazdelenieId", "PodrazdelenieName", model.PodrazdelenieId);
            return View(model);
        }

        // GET: CustomUser/Index
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: CustomUser/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.PodrazdelenieId = new SelectList(_context.Podrazdelenies, "PodrazdelenieId", "PodrazdelenieName", user.PodrazdelenieId);
            return View(user);
        }

        // POST: CustomUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CustomUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.Surname = model.Surname;
                user.Ima = model.Ima;
                user.SecSurname = model.SecSurname;
                user.Age = model.Age;
                user.PodrazdelenieId = model.PodrazdelenieId;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Model state error: {ErrorMessage}", error.ErrorMessage);
                }
            }
            ViewBag.PodrazdelenieId = new SelectList(_context.Podrazdelenies, "PodrazdelenieId", "PodrazdelenieName", model.PodrazdelenieId);
            _logger.LogWarning("Model state is invalid.");
            return View(model);
        }

        // GET: CustomUser/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: CustomUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(user);
        }
    }
}