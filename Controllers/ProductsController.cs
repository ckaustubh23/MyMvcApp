using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;
namespace MyMvcApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Stock")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Stock")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Purchase
        public async Task<IActionResult> Purchase(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than zero.");
                return View(product);
            }

            if (product.Stock < quantity)
            {
                ModelState.AddModelError("", "Insufficient stock available.");
                return View(product);
            }

            product.Stock -= quantity;

            var purchase = new Purchase
            {
                ProductId = product.Id,
                Quantity = quantity,
                PurchaseDate = DateTime.Now
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DownloadReport()
    {
        var products = _context.Products.ToList();
        var purchases = _context.Purchases.ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Report");

            worksheet.Cell(1, 1).Value = "Product Name";
            worksheet.Cell(1, 2).Value = "Price";
            worksheet.Cell(1, 3).Value = "Initial Stock";
            worksheet.Cell(1, 4).Value = "Sold";
            worksheet.Cell(1, 5).Value = "Remaining Stock";

            // Data Rows
            int row = 2;
            foreach (var product in products)
            {
                var totalSold = purchases.Where(p => p.ProductId == product.Id).Sum(p => p.Quantity);
                worksheet.Cell(row, 1).Value = product.Name;
                worksheet.Cell(row, 2).Value = product.Price;
                worksheet.Cell(row, 3).Value = product.Stock + totalSold; 
                worksheet.Cell(row, 4).Value = product.Stock;
                worksheet.Cell(row, 5).Value = totalSold;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DetailedReport.xlsx");
            }
        }
    }

        // method for existing products
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
