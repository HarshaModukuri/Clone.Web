using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OktaClone.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OktaClone.Web.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            // Seed Roles
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));

            // Seed Admin User
            var adminUser = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true, FullName = "Admin User" };
            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");

            // Seed Regular User
            var regularUser = new ApplicationUser { UserName = "user@example.com", Email = "user@example.com", EmailConfirmed = true, FullName = "Regular User" };
            await userManager.CreateAsync(regularUser, "User@123");
            await userManager.AddToRoleAsync(regularUser, "User");

            // Seed Dummy Users for Chat Testing
            for (int i = 1; i <= 15; i++)
            {
                var dummyUser = new ApplicationUser { UserName = $"testuser{i}@example.com", Email = $"testuser{i}@example.com", EmailConfirmed = true, FullName = $"Test User {i}" };
                await userManager.CreateAsync(dummyUser, "Test@123");
                await userManager.AddToRoleAsync(dummyUser, "User");
            }

            // Seed Applications
            var applications = new Application[]
            {
                new Application{Name="Salesforce", Description="Salesforce CRM", IconUrl="/icons/salesforce.png"},
                new Application{Name="Zoom", Description="Zoom Video Conferencing", IconUrl="/icons/zoom.png"},
                new Application{Name="Office 365", Description="Microsoft Office 365", IconUrl="/icons/office365.png"},
                new Application{Name="Gainsight", Description="Gainsight Customer Success", IconUrl="/icons/gainsight.png"},
                new Application{Name="Udemy for Business", Description="Udemy Online Learning", IconUrl="/icons/udemy.png"}
            };
            foreach (Application app in applications)
            {
                context.Applications.Add(app);
            }
            await context.SaveChangesAsync();

            // Seed UserApplications
            var userApplications = new UserApplication[]
            {
                new UserApplication{UserId = regularUser.Id, ApplicationId = applications[0].Id, Status = "Assigned"},
                new UserApplication{UserId = regularUser.Id, ApplicationId = applications[1].Id, Status = "Assigned"},
                new UserApplication{UserId = regularUser.Id, ApplicationId = applications[2].Id, Status = "Requested"},
            };

            foreach (UserApplication ua in userApplications)
            {
                context.UserApplications.Add(ua);
            }
            await context.SaveChangesAsync();
        }
    }
}
