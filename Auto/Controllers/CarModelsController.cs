﻿using System;
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
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "IT, Warehouse, Administration")]
        public async Task<IActionResult> Index(int? brandId)
        {
            if (brandId == null)
            {
                var allModels = await _context.CarModels.Include(c => c.Brand).ToListAsync();
                return View(allModels);
            }
            else
            {
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
        [Authorize(Roles = "IT, Warehouse")]
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
        [Authorize(Roles = "IT, Warehouse")]
        public IActionResult Create(int? brandId)
        {
            ViewBag.BrandId = brandId;
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name", brandId);
            return View();
        }

        // POST: CarModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Create([Bind("CarModelId,Name,Year,BrandId,LogoPath")] CarModel carModel, IFormFile logoFile)
        {
            if (ModelState.IsValid)
            {
                // Проверка на существование модели с таким же названием и годом выпуска
                var existingCarModel = await _context.CarModels
                    .FirstOrDefaultAsync(cm => cm.Name == carModel.Name && cm.Year == carModel.Year && cm.BrandId == carModel.BrandId);
                if (existingCarModel != null)
                {
                    ModelState.AddModelError("Name", "Модель с таким названием и годом выпуска уже существует.");
                    ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name", carModel.BrandId);
                    return View(carModel);
                }

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

                _context.Add(carModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { brandId = carModel.BrandId });
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name", carModel.BrandId);
            return View(carModel);
        }

        // GET: CarModels/Edit/5
        [Authorize(Roles = "IT, Warehouse")]
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
        [Authorize(Roles = "IT, Warehouse")]
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
                    // Проверка на существование модели с таким же названием и годом выпуска
                    var existingCarModel = await _context.CarModels
                        .FirstOrDefaultAsync(cm => cm.Name == carModel.Name && cm.Year == carModel.Year && cm.BrandId == carModel.BrandId && cm.CarModelId != carModel.CarModelId);
                    if (existingCarModel != null)
                    {
                        ModelState.AddModelError("Name", "Модель с таким названием и годом выпуска уже существует.");
                        ViewData["BrandList"] = new SelectList(_context.Brands, "BrandId", "Name", carModel.BrandId);
                        return View(carModel);
                    }

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
            ViewData["BrandList"] = new SelectList(_context.Brands, "BrandId", "Name", carModel.BrandId);
            return View(carModel);
        }

        // GET: CarModels/Delete/5
        [Authorize(Roles = "IT, Warehouse")]
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
        [Authorize(Roles = "IT, Warehouse")]
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