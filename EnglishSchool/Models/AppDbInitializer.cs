using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EnglishSchool.Models
{
    public class AppDbInitializer: DropCreateDatabaseAlways<ApplicationDbContext>
    {
        public override void InitializeDatabase(ApplicationDbContext context)
        {
            context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction
            , string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.Database.Connection.Database));
            base.InitializeDatabase(context);
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //создаем две роли:
            var role1 = new IdentityRole { Name = "Admin" };
            var role2 = new IdentityRole { Name = "User" };

            //добавляем роли в бд:
            roleManager.Create(role1);
            roleManager.Create(role2);

            var admin = new ApplicationUser { Email = "admin@english.school", UserName = "admin@english.school" };
            string password = "123456";
            var result = userManager.Create(admin, password);

            //если создание пользователя прошло успешно
            if(result.Succeeded)
            {
                //добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
            }
            base.Seed(context);
        }
    }
}