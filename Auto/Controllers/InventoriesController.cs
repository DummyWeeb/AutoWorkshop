using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Auto.Data;
using Auto.Models;

namespace Auto.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, DateTime? writeOffStartDate, DateTime? writeOffEndDate)
        {
            var inventories = _context.Inventories.Include(i => i.Part).AsQueryable();

            if (startDate.HasValue)
            {
                inventories = inventories.Where(i => i.поступления.HasValue && i.поступления.Value.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                inventories = inventories.Where(i => i.поступления.HasValue && i.поступления.Value.Date <= endDate.Value.Date);
            }

            if (writeOffStartDate.HasValue)
            {
                inventories = inventories.Where(i => i.списания.HasValue && i.списания.Value.Date >= writeOffStartDate.Value.Date);
            }

            if (writeOffEndDate.HasValue)
            {
                inventories = inventories.Where(i => i.списания.HasValue && i.списания.Value.Date <= writeOffEndDate.Value.Date);
            }

            return View(await inventories.ToListAsync());
        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Part)
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }
    }
}