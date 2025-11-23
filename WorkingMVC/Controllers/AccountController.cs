using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkingMVC.Constants;
using WorkingMVC.Data.Entities.Idenity;
using WorkingMVC.Interfaces;
using WorkingMVC.Models.Account;
using Microsoft.AspNetCore.Http; // Для IFormFile (хоча тут неявно, краще мати)

namespace WorkingMVC.Controllers;

public class AccountController(
    UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager,
    IImageService imageService,
    IMapper mapper) : Controller
{
    // Назва папки для зображень користувачів
    private const string UserFolderName = "avatars";

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            var res = await signInManager
                .PasswordSignInAsync(user, model.Password, false, false);

            // Запобігаємо подвійній авторизації, оскільки PasswordSignInAsync вже авторизує
            if (res.Succeeded)
            {
                // await signInManager.SignInAsync(user, isPersistent: false); 
                return Redirect("/");
            }
        }
        ModelState.AddModelError("", "Дані вазано не вірно!");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = mapper.Map<UserEntity>(model);

        // ВИПРАВЛЕНО: Змінено UploadImageAsync на SaveImageAsync та додано папку UserFolderName
        var imageStr = model.Image is not null
            ? await imageService.SaveImageAsync(model.Image, UserFolderName) : null;

        user.Image = imageStr;
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            //Даю користувачеві роль User
            result = await userManager.AddToRoleAsync(user, Roles.User);
            //після реєстрації авторизовуємо
            await signInManager.SignInAsync(user, isPersistent: false);
            //Перехід на головну
            return RedirectToAction("Index", "Main");
        }
        else
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Redirect("/");
    }

}