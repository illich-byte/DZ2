using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WorkingMVC.Interfaces;

public interface IImageService
{
    // Змінено назву на SaveImageAsync та додано аргумент folderName
    Task<string> SaveImageAsync(IFormFile file, string folderName);

    // Додано метод для видалення, який приймає ім'я файлу та папку
    void DeleteImage(string fileName, string folderName);
}