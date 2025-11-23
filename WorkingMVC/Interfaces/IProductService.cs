using WorkingMVC.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Потрібен для IFormFile

namespace WorkingMVC.Interfaces
{
    public interface IProductService
    {
        // CRUD для товарів
        Task<List<ProductEntity>> GetAllProductsAsync();
        Task<ProductEntity> GetProductByIdAsync(int id);

        // Метод для створення: приймає сам товар та список файлів
        Task CreateProductAsync(ProductEntity product, List<IFormFile> images);

        // Метод для оновлення: приймає сам товар та список нових файлів
        Task UpdateProductAsync(ProductEntity product, List<IFormFile> newImages);

        Task DeleteProductAsync(int id);

        // Додаткові методи для керування фотографіями
        Task DeleteImageAsync(int imageId);
    }
}