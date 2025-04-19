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

namespace Auto.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly PdfService _pdfService;
        private readonly ICompositeViewEngine _viewEngine;

        public InventoriesController(ApplicationDbContext context, UserManager<CustomUser> userManager, PdfService pdfService, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _userManager = userManager;
            _pdfService = pdfService;
            _viewEngine = viewEngine;
        }

        // GET: Inventories
        [Authorize(Roles = "IT, Warehouse, Administration, Procurement")]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, DateTime? writeOffStartDate, DateTime? writeOffEndDate)
        {
            var inventoriesQuery = _context.Inventories.Include(i => i.Part).AsQueryable();

            if (startDate.HasValue)
            {
                inventoriesQuery = inventoriesQuery.Where(i => i.поступления >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                inventoriesQuery = inventoriesQuery.Where(i => i.поступления < endDate.Value.Date.AddDays(1));
            }

            if (writeOffStartDate.HasValue)
            {
                inventoriesQuery = inventoriesQuery.Where(i => i.списания >= writeOffStartDate.Value.Date);
            }

            if (writeOffEndDate.HasValue)
            {
                inventoriesQuery = inventoriesQuery.Where(i => i.списания < writeOffEndDate.Value.Date.AddDays(1));
            }

            var inventories = await inventoriesQuery.ToListAsync();

            return View(inventories);
        }

        [HttpGet]
        [Authorize(Roles = "IT, Warehouse, Administration")]
        public async Task<IActionResult> DownloadPdf(DateTime? startDate, DateTime? endDate, DateTime? writeOffStartDate, DateTime? writeOffEndDate)
        {
            var inventoriesQuery = _context.Inventories.Include(i => i.Part).AsQueryable();

            if (startDate.HasValue)
            {
                inventoriesQuery = inventoriesQuery.Where(i => i.поступления >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                inventoriesQuery = inventoriesQuery.Where(i => i.поступления < endDate.Value.Date.AddDays(1));
            }

            if (writeOffStartDate.HasValue)
            {
                inventoriesQuery = inventoriesQuery.Where(i => i.списания >= writeOffStartDate.Value.Date);
            }

            if (writeOffEndDate.HasValue)
            {
                inventoriesQuery = inventoriesQuery.Where(i => i.списания < writeOffEndDate.Value.Date.AddDays(1));
            }

            var inventories = await inventoriesQuery.ToListAsync();

            var htmlContent = await RenderViewToStringAsync("Index", inventories, new { isPdf = true });
            var pdfBytes = _pdfService.GeneratePdf(htmlContent);

            return File(pdfBytes, "application/pdf", "HistoryReport.pdf");
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