using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using WorkingMVC.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration; // Для DI

namespace WorkingMVC.Services;

public class ImageService(IConfiguration configuration) : IImageService
{
    // ВИПРАВЛЕНО: Змінена назва та доданий аргумент folderName
    public async Task<string> SaveImageAsync(IFormFile file, string folderName)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            // Використовуємо тут тимчасове ім'я файлу
            var fileName = Path.GetRandomFileName() + ".webp";

            var bytes = memoryStream.ToArray();
            using var image = Image.Load(bytes);

            image.Mutate(imgc =>
            {
                imgc.Resize(new ResizeOptions
                {
                    Size = new Size(600, 600),
                    Mode = ResizeMode.Max
                });
            });

            // Беремо базову папку (наприклад, 'images') з конфігурації
            var dirImageName = configuration["DirImageName"] ?? "images";

            // Шлях: CurentDir / images / products / ім'я_файлу
            // Тут використовуємо folderName, переданий з ProductService
            var path = Path.Combine(Directory.GetCurrentDirectory(), dirImageName, folderName, fileName);

            // Створюємо папку, якщо вона не існує
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), dirImageName, folderName));

            await image.SaveAsync(path, new WebpEncoder());

            // Повертаємо лише ім'я файлу (що вимагає ProductService)
            return fileName;
        }
        catch
        {
            return string.Empty;
        }
    }

    // ДОДАНО: Метод для видалення зображення
    public void DeleteImage(string fileName, string folderName)
    {
        try
        {
            var dirImageName = configuration["DirImageName"] ?? "images";

            // Шлях: CurentDir / images / products / ім'я_файлу
            var path = Path.Combine(Directory.GetCurrentDirectory(), dirImageName, folderName, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            // У реальному проєкті тут має бути логування помилки
            Console.WriteLine($"Помилка при видаленні файлу: {ex.Message}");
        }
    }
}