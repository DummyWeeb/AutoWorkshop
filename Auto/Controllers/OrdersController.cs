using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auto.Data;
using Auto.Models;

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
        public IActionResult Create()
        {
            ViewBag.Parts = _context.Parts.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,TotalAmount,PricePerUnit,SupplierId,SupplierName,OrderParts")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Status = OrderStatus.Заказано; // Устанавливаем статус по умолчанию
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Parts = _context.Parts.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderParts)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.Parts = _context.Parts.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            return View(order);
        }

        // GET: Orders/Delete/5
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