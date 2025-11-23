using WorkingMVC.Data;
using WorkingMVC.Data.Entities;
using WorkingMVC.Interfaces;
using Microsoft.EntityFrameworkCore; // Для Include() та ToListAsync()
using Microsoft.AspNetCore.Http;     // Для IFormFile
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;                   // Для OrderBy() та Linq-запитів

namespace WorkingMVC.Services
{
    public class ProductService : IProductService
    {
        private readonly MyAppDbContext _context;
        private readonly IImageService _imageService;
        private const string ProductsFolderName = "products"; // Назва папки для зображень товарів

        public ProductService(MyAppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // ------------------ READ ------------------
        public async Task<List<ProductEntity>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages.OrderBy(img => img.IsMain)) // ВИПРАВЛЕНО: Використовуємо ProductImages
                .ToListAsync();
        }

        public async Task<ProductEntity> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages) // ВИПРАВЛЕНО: Використовуємо ProductImages
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // ------------------ CREATE ------------------
        public async Task CreateProductAsync(ProductEntity product, List<IFormFile> images)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (images != null && images.Any())
            {
                // Зберігаємо усі фотографії та прив'язуємо їх до товару
                await SaveImagesToProduct(product.Id, images);
            }
        }

        // ------------------ UPDATE ------------------
        public async Task UpdateProductAsync(ProductEntity product, List<IFormFile> newImages)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            // Якщо є нові фотографії, зберігаємо їх
            if (newImages != null && newImages.Any())
            {
                await SaveImagesToProduct(product.Id, newImages);
            }
        }

        // ------------------ DELETE ------------------
        public async Task DeleteProductAsync(int id)
        {
            // ВИПРАВЛЕНО: Include(p => p.ProductImages)
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                // Видаляємо фізичні файли з сервера
                foreach (var img in product.ProductImages) // ВИПРАВЛЕНО: ProductImages
                {
                    _imageService.DeleteImage(img.ImageUrl, ProductsFolderName);
                }

                // Видаляємо сам товар
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // ------------------ IMAGE LOGIC ------------------
        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                // 1. Видаляємо фізичний файл
                _imageService.DeleteImage(image.ImageUrl, ProductsFolderName);

                // 2. Видаляємо запис з БД
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SaveImagesToProduct(int productId, List<IFormFile> images)
        {
            var imageEntities = new List<ProductImageEntity>();
            bool isFirstImage = true;

            foreach (var file in images)
            {
                var imageUrl = await _imageService.SaveImageAsync(file, ProductsFolderName);

                imageEntities.Add(new ProductImageEntity
                {
                    ProductId = productId,
                    ImageUrl = imageUrl,
                    ImageDescription = file.FileName,
                    IsMain = isFirstImage // Призначаємо перше фото головним
                });

                isFirstImage = false;
            }

            await _context.ProductImages.AddRangeAsync(imageEntities);
            await _context.SaveChangesAsync();
        }
    }
}