namespace EnglishSchool.Migrations
{
    using EnglishSchool.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EnglishSchool.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EnglishSchool.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
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
            if (result.Succeeded)
            {
                //добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
            }
            base.Seed(context);
        }
    }
}
