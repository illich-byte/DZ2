using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using WorkingMVC.Data.Entities;       // <--- Додай це (або .Idenity, залежить де лежить RoleEntity)
using WorkingMVC.Data.Entities.Idenity;

namespace WorkingMVC.Models.Users
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }

        // ТУТ БУЛО IdentityRole, А ТРЕБА RoleEntity
        public List<RoleEntity> AllRoles { get; set; }

        public IList<string> UserRoles { get; set; }

        public ChangeRoleViewModel()
        {
            AllRoles = new List<RoleEntity>();
            UserRoles = new List<string>();
        }
    }
}