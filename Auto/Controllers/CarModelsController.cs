using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auto.Data;
using Auto.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Auto.Controllers
{
    public class CarModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CarModelsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: CarModels
        public async Task<IActionResult> Index(int? brandId)
        {
            if (brandId == null)
            {
                // Если brandId не указан, показываем все модели
                var allModels = await _context.CarModels.Include(c => c.Brand).ToListAsync();
                return View(allModels);
            }
            else
            {
                // Если brandId указан, показываем модели только для этой марки
                var modelsForBrand = await _context.CarModels
                    .Where(m => m.BrandId == brandId)
                    .Include(c => c.Brand)
                    .ToListAsync();
                ViewBag.BrandId = brandId;
                ViewBag.BrandName = _context.Brands.FirstOrDefault(b => b.BrandId == brandId)?.Name;
                return View(modelsForBrand);
            }
        }

        // GET: CarModels/Details/5
        public async Task<IActionResult> Details(int? id, int? brandId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(m => m.CarModelId == id);
            if (carModel == null)
            {
                return NotFound();
            }

            ViewBag.BrandId = brandId;
            return View(carModel);
        }



        // GET: CarModels/Create
        public IActionResult Create(int carModelId)
        {
            ViewBag.CarModelId = carModelId;
            return View();
        }

        // POST: CarModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PartId,Name,Description,Quantity")] Part part, int carModelId)
        {
            if (ModelState.IsValid)
            {
                var carModel = await _context.CarModels.FindAsync(carModelId);
                if (carModel != null)
                {
                    part.CarModels = new List<CarModel> { carModel };
                }
                _context.Add(part);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { carModelId = carModelId });
            }
            ViewBag.CarModelId = carModelId;
            return View(part);
        }

        // GET: CarModels/Edit/5
        public async Task<IActionResult> Edit(int? id, int? brandId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels.FindAsync(id);
            if (carModel == null)
            {
                return NotFound();
            }
            ViewBag.BrandId = brandId;
            ViewData["BrandList"] = new SelectList(_context.Brands, "BrandId", "Name", carModel.BrandId);
            return View(carModel);
        }

        // POST: CarModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarModelId,Name,Year,BrandId,LogoPath")] CarModel carModel, IFormFile logoFile)
        {
            if (id != carModel.CarModelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (logoFile != null)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "carmodels");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + logoFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(fileStream);
                        }
                        carModel.LogoPath = "/images/carmodels/" + uniqueFileName;
                    }

                    _context.Update(carModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarModelExists(carModel.CarModelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { brandId = carModel.BrandId });
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name", carModel.BrandId);
            return View(carModel);
        }

        // GET: CarModels/Delete/5
        public async Task<IActionResult> Delete(int? id, int? brandId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(m => m.CarModelId == id);
            if (carModel == null)
            {
                return NotFound();
            }

            ViewBag.BrandId = brandId;
            return View(carModel);
        }

        // POST: CarModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? brandId)
        {
            var carModel = await _context.CarModels.FindAsync(id);
            if (carModel != null)
            {
                _context.CarModels.Remove(carModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { brandId = brandId });
        }

        private bool CarModelExists(int id)
        {
            return _context.CarModels.Any(e => e.CarModelId == id);
        }
    }
}