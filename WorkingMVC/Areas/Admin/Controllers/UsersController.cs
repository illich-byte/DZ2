using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using WorkingMVC.Models.Users;
using WorkingMVC.Data.Entities.Idenity; // Для UserEntity
using WorkingMVC.Data.Entities;         // Перевір, можливо RoleEntity тут

namespace WorkingMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<UserEntity> _userManager;

        // ЗМІНИЛИ ТУТ: IdentityRole -> RoleEntity
        private readonly RoleManager<RoleEntity> _roleManager;

        // І В КОНСТРУКТОРІ ЗМІНИЛИ
        public UsersController(UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

        [HttpGet]
        public async Task<IActionResult> EditRoles(string userId)
        {
            UserEntity user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            ChangeRoleViewModel model = new ChangeRoleViewModel
            {
                UserId = user.Id.ToString(),
                UserEmail = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles // Тепер тут список RoleEntity
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(string userId, List<string> roles)
        {
            UserEntity user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            // Тут логіка залишається такою ж, бо ми працюємо з назвами (string)
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return RedirectToAction("Index");
        }
    }
}