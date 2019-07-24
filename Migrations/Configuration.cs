namespace KerryDPeay_Blog.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using KerryDPeay_Blog.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<KerryDPeay_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string DisplayName { get; private set; }

        protected override void Seed(KerryDPeay_Blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.


            #region roleManager

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole
                {
                    Name = "Admin"
                });
            }

            //if there is not a role existing yet, add a moderator role
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole
                {
                    Name = "Moderator"
                });
            }

            #endregion

            //I need to create a few users that will eventually occupy the roles of either Admin or Moderator
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "kerrydp8@outlook.com"))
            {
                userManager.Create(new ApplicationUser
                {

                    UserName = "kerrydp8@outlook.com",
                    Email = "kerrydp8@outlook.com",
                    FirstName = "Kerry",
                    LastName = "Peay",
                    DisplayName = "kerrydp8"
                },
                "Wiiugamer12");
            }

            if (!context.Users.Any(u => u.Email == "JTwichell@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "JTwichell@Mailinator.com",
                    Email = "JTwichell@Mailinator.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "Twich"
                },"Abc&123!");

            var userId = userManager.FindByEmail("JTwichell@Mailinator.com").Id;
            userManager.AddToRole(userId, "Moderator");

            userId = userManager.FindByEmail("kerrydp8@outlook.com").Id;
            userManager.AddToRole(userId, "Admin");
            }
        }

    }
}
