using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkingMVC.Data.Entities;
using WorkingMVC.Interfaces;
using WorkingMVC.Constants; // Для доступу до Roles
using System.Threading.Tasks;
using System.Collections.Generic; // Потрібен для List<IFormFile>

namespace WorkingMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: /Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // ------------------ СТВОРЕННЯ (CREATE) ------------------

        // GET: /Admin/Products/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesAsync();
            return View();
        }

        // POST: /Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductEntity product, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(product, images);
                return RedirectToAction(nameof(Index));
            }

            await PopulateCategoriesAsync();
            return View(product);
        }

        // ------------------ РЕДАГУВАННЯ (EDIT) - ПОЧАТОК ------------------

        // GET: /Admin/Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();

            await PopulateCategoriesAsync();
            return View(product);
        }

        // POST: /Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEntity product, List<IFormFile> newImages)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(product, newImages);
                }
                catch (System.Exception)
                {
                    // Логіка обробки помилок, наприклад, якщо товар не знайдено
                    if (await _productService.GetProductByIdAsync(id) == null)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateCategoriesAsync();
            return View(product);
        }

        // ------------------ ВИДАЛЕННЯ (DELETE) - ПОЧАТОК ------------------

        // GET: /Admin/Products/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: /Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ------------------ ДОДАТКОВІ ДІЇ ------------------

        // POST: /Admin/Products/DeleteImage
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            await _productService.DeleteImageAsync(imageId);
            // Припускаємо, що після видалення повертаємося на форму редагування
            return RedirectToAction(nameof(Edit), new { id = TempData["ProductIdForRedirect"] });
        }

        // ------------------ ДОПОМІЖНІ МЕТОДИ ------------------

        private async Task PopulateCategoriesAsync()
        {
            // ВИПРАВЛЕНО: Змінено GetAll() на GetAllAsync()
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, nameof(CategoryEntity.Id), nameof(CategoryEntity.Name));
        }
    }
}