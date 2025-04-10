using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auto.Data;
using Auto.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Auto.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        [Authorize(Roles = "IT, Procurement, Administration")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.OrderParts)
                    .ThenInclude(op => op.Part)
                .ToListAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "IT, Procurement")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.OrderParts)
                    .ThenInclude(op => op.Part)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = "IT, Procurement")]
        public IActionResult Create()
        {
            ViewBag.Parts = _context.Parts.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            ViewBag.PartsJson = JsonConvert.SerializeObject(_context.Parts.Select(p => p.Name).ToList());
            ViewBag.SuppliersJson = JsonConvert.SerializeObject(_context.Suppliers.Select(s => s.Name).ToList());
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT, Procurement")]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,TotalAmount,PricePerUnit,SupplierName,OrderParts")] Order order)
        {
            if (ModelState.IsValid)
            {
                var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Name == order.SupplierName);
                if (supplier != null)
                {
                    order.SupplierId = supplier.SupplierId;
                }
                else
                {
                    ModelState.AddModelError("SupplierName", "Поставщик не найден.");
                    ViewBag.Parts = _context.Parts.ToList();
                    ViewBag.Suppliers = _context.Suppliers.ToList();
                    ViewBag.PartsJson = JsonConvert.SerializeObject(_context.Parts.Select(p => p.Name).ToList());
                    ViewBag.SuppliersJson = JsonConvert.SerializeObject(_context.Suppliers.Select(s => s.Name).ToList());
                    return View(order);
                }

                foreach (var orderPart in order.OrderParts)
                {
                    var part = await _context.Parts.FirstOrDefaultAsync(p => p.Name == orderPart.PartName);
                    if (part != null)
                    {
                        orderPart.PartId = part.PartId;
                    }
                    else
                    {
                        ModelState.AddModelError("OrderParts", $"Запчасть '{orderPart.PartName}' не найдена.");
                        ViewBag.Parts = _context.Parts.ToList();
                        ViewBag.Suppliers = _context.Suppliers.ToList();
                        ViewBag.PartsJson = JsonConvert.SerializeObject(_context.Parts.Select(p => p.Name).ToList());
                        ViewBag.SuppliersJson = JsonConvert.SerializeObject(_context.Suppliers.Select(s => s.Name).ToList());
                        return View(order);
                    }
                }

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Parts = _context.Parts.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            ViewBag.PartsJson = JsonConvert.SerializeObject(_context.Parts.Select(p => p.Name).ToList());
            ViewBag.SuppliersJson = JsonConvert.SerializeObject(_context.Suppliers.Select(s => s.Name).ToList());
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT, Procurement")]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,TotalAmount,Status,PricePerUnit,SupplierId,SupplierName,OrderParts")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewBag.Parts = _context.Parts.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            ViewBag.PartsJson = JsonConvert.SerializeObject(_context.Parts.Select(p => p.Name).ToList());
            ViewBag.SuppliersJson = JsonConvert.SerializeObject(_context.Suppliers.Select(s => s.Name).ToList());
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "IT, Procurement")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.OrderParts)
                    .ThenInclude(op => op.Part)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT, Procurement")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderParts)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult MarkAsArrived(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderParts)
                .ThenInclude(op => op.Part)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = OrderStatus.ЗаказВыполнен;
            _context.SaveChanges();

            foreach (var orderPart in order.OrderParts)
            {
                var part = _context.Parts.Find(orderPart.PartId);
                if (part != null)
                {
                    part.Quantity += orderPart.Quantity;
                    _context.SaveChanges();

                    // Обновление инвентаря
                    var inventory = new Inventory
                    {
                        PartId = part.PartId,
                        Quantity = orderPart.Quantity,
                        поступления = DateTime.Now,
                        Part = part
                    };
                    _context.Inventories.Add(inventory);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}