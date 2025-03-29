using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auto.Data;
using Auto.Models;

namespace Auto.Controllers
{
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parts
        public async Task<IActionResult> Index(int? carModelId)
        {
            if (carModelId == null)
            {
                return View(await _context.Parts.ToListAsync());
            }
            else
            {
                var partsForCarModel = await _context.Parts
                    .Where(p => p.CarModels.Any(cm => cm.CarModelId == carModelId))
                    .ToListAsync();
                ViewBag.CarModelId = carModelId;
                return View(partsForCarModel);
            }
        }

        // GET: Parts/Details/5
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
        public IActionResult Create(int carModelId)
        {
            ViewBag.CarModelId = carModelId;
            ViewBag.CarModels = _context.CarModels.ToList();
            return View();
        }

        // POST: Parts/Create
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
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CarModelId = carModelId;
            return View(part);
        }

        // GET: Parts/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("PartId,Name,Description,Quantity,CarModelId")] Part part)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part != null)
            {
                _context.Parts.Remove(part);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PartExists(int id)
        {
            return _context.Parts.Any(e => e.PartId == id);
        }
    }
}