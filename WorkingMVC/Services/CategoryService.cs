using AutoMapper;
using WorkingMVC.Data;
using WorkingMVC.Data.Entities;
using WorkingMVC.Interfaces;
using WorkingMVC.Models.Category;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkingMVC.Services;

public class CategoryService(ICategoryRepository categoryRepository,
    IImageService imageService,
    IMapper mapper) : ICategoryService
{
    private const string CategoryFolderName = "categories"; // Папка для зображень категорій

    public async Task CreateAsync(CategoryCreateModel model)
    {
        var entity = await categoryRepository.FindByNameAsync(model.Name);

        if (entity != null)
        {
            throw new Exception("У нас проблеми Хюстон" +
                $"Така категорія уже є {model.Name}");
        }

        // Використовуємо мапер, якщо це можливо, для уникнення ручного присвоєння
        // entity = mapper.Map<CategoryEntity>(model);

        entity = new CategoryEntity
        {
            Name = model.Name
        };

        if (model.Image != null)
        {
            // ВИПРАВЛЕНО: Змінено UploadImageAsync на SaveImageAsync та додано папку CategoryFolderName
            entity.Image = await imageService.SaveImageAsync(model.Image, CategoryFolderName);
        }

        await categoryRepository.AddAsync(entity);
    }

    public async Task<List<CategoryItemModel>> GetAllAsync()
    {
        var listTest = await categoryRepository.GetAllAsync();
        var model = mapper.Map<List<CategoryItemModel>>(listTest);
        return model;
    }
}