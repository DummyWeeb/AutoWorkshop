using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auto.Data;
using Auto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DinkToPdf.Contracts;
using Auto.Services;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;

namespace Auto.Controllers
{
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly PdfService _pdfService;
        private readonly ICompositeViewEngine _viewEngine;

        public PartsController(ApplicationDbContext context, UserManager<CustomUser> userManager, PdfService pdfService, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _userManager = userManager;
            _pdfService = pdfService;
            _viewEngine = viewEngine;
        }

        // GET: Parts
        [Authorize(Roles = "IT, Warehouse, Administration")]
        public async Task<IActionResult> Index(int? carModelId, string searchString)
        {
            if (carModelId == null)
            {
                var parts = from p in _context.Parts select p;

                if (!string.IsNullOrEmpty(searchString))
                {
                    parts = parts.Where(p => p.Name.Contains(searchString));
                }

                return View(await parts.ToListAsync());
            }
            else
            {
                var carModel = await _context.CarModels.FindAsync(carModelId);
                if (carModel == null)
                {
                    return NotFound();
                }

                var partsForCarModel = from p in _context.Parts
                                       where p.CarModels.Any(cm => cm.CarModelId == carModelId)
                                       select p;

                if (!string.IsNullOrEmpty(searchString))
                {
                    partsForCarModel = partsForCarModel.Where(p => p.Name.Contains(searchString));
                }

                ViewBag.CarModelId = carModelId;
                ViewBag.CarModelName = carModel.Name;
                ViewBag.CurrentFilter = searchString;

                return View(await partsForCarModel.ToListAsync());
            }
        }

        // GET: Parts/Details/5
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts
                .FirstOrDefaultAsync(m => m.PartId == id);
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // GET: Parts/Create
        [Authorize(Roles = "IT, Warehouse")]
        public IActionResult Create(int? carModelId)
        {
            if (carModelId == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    ErrorMessage = "Создавать запчасть можно только перейдя в представление запчастей выбрав модель автомобиля"
                };
                return View("Error", errorViewModel);
            }

            ViewBag.CarModelId = carModelId;
            ViewBag.CarModels = _context.CarModels.ToList();
            return View();
        }

        // POST: Parts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Create([Bind("PartId,Name")] Part part, int? carModelId)
        {
            if (carModelId == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    ErrorMessage = "Создавать запчасть можно только перейдя в представление запчастей выбрав модель автомобиля"
                };
                return View("Error", errorViewModel);
            }

            if (ModelState.IsValid)
            {
                var carModel = await _context.CarModels.FindAsync(carModelId);
                if (carModel != null)
                {
                    part.Name = $"{part.Name} для {carModel.Name}";
                    part.CarModels = new List<CarModel> { carModel };
                }

                // Проверка на существование запчасти с таким же названием
                var existingPart = await _context.Parts
                    .FirstOrDefaultAsync(p => p.Name == part.Name);
                if (existingPart != null)
                {
                    ModelState.AddModelError("Name", "Запчасть с таким названием уже существует.");
                    ViewBag.CarModelId = carModelId;
                    ViewBag.CarModels = _context.CarModels.ToList();
                    return View(part);
                }

                part.Quantity = 0; // Устанавливаем количество в 0
                _context.Add(part);
                await _context.SaveChangesAsync();

                // Обновление инвентаря
                var inventory = new Inventory
                {
                    PartId = part.PartId,
                    Quantity = part.Quantity,
                    поступления = DateTime.Now,
                    Part = part
                };
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { carModelId = carModelId });
            }
            ViewBag.CarModelId = carModelId;
            ViewBag.CarModels = _context.CarModels.ToList();
            return View(part);
        }

        // GET: Parts/Edit/5
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts.FindAsync(id);
            if (part == null)
            {
                return NotFound();
            }
            ViewBag.CarModels = _context.CarModels.ToList();
            return View(part);
        }

        // POST: Parts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Edit(int id, [Bind("PartId,Name,Quantity,CarModelId")] Part part)
        {
            if (id != part.PartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(part);
                    await _context.SaveChangesAsync();

                    // Обновление инвентаря
                    var inventory = new Inventory
                    {
                        PartId = part.PartId,
                        Quantity = part.Quantity,
                        поступления = DateTime.Now,
                        Part = part
                    };
                    _context.Inventories.Add(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartExists(part.PartId))
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
            ViewBag.CarModels = _context.CarModels.ToList();
            return View(part);
        }

        // GET: Parts/Delete/5
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts
                .FirstOrDefaultAsync(m => m.PartId == id);
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // POST: Parts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part != null)
            {
                if (part.Quantity > 0)
                {
                    ModelState.AddModelError(string.Empty, "Невозможно удалить запчасть, так как её количество больше 0.");
                    return View(part);
                }

                // Помечаем запчасть как удаленную
                part.Name += " (удалено)";
                _context.Parts.Update(part);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Parts/UsePart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> UsePart(int id)
        {
            var user = await _userManager.Users
                .Include(u => u.Podrazdelenie)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (user?.Podrazdelenie?.PodrazdelenieName == "Администрация")
            {
                return Forbid();
            }

            var part = await _context.Parts.FindAsync(id);
            if (part != null && part.Quantity > 0)
            {
                part.Quantity--;
                await _context.SaveChangesAsync();

                // Обновление инвентаря
                var inventory = new Inventory
                {
                    PartId = part.PartId,
                    Quantity = -1,
                    списания = DateTime.Now,
                    Part = part
                };
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UsageReport), new { id = id });
        }

        // GET: Parts/UsageReport/5
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> UsageReport(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // GET: Parts/History/5
        [Authorize(Roles = "IT, Warehouse, Administration")]
        public async Task<IActionResult> History(int partId)
        {
            var inventories = await _context.Inventories
                .Include(i => i.Part)
                .Where(i => i.PartId == partId)
                .ToListAsync();

            return View(inventories);
        }

        private bool PartExists(int id)
        {
            return _context.Parts.Any(e => e.PartId == id);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null)
            {
                return NotFound();
            }

            var htmlContent = await RenderViewToStringAsync("UsageReport", part);
            var pdfBytes = _pdfService.GeneratePdf(htmlContent);

            return File(pdfBytes, "application/pdf", "UsageReport.pdf");
        }

        private async Task<string> RenderViewToStringAsync(string viewName, object model, object routeValues = null)
        {
            ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"View {viewName} not found");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                if (routeValues != null)
                {
                    foreach (var routeValue in new RouteValueDictionary(routeValues))
                    {
                        viewContext.RouteData.Values[routeValue.Key] = routeValue.Value;
                    }
                }

                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}