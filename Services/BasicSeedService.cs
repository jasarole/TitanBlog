using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBlog.Data;
using TitanBlog.Models;

namespace TitanBlog.Services
{
    public class BasicSeedService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public BasicSeedService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            await _context.Database.MigrateAsync();
            await SeedDataAsync();
        }

        //This is a wrapper method
        public async Task SeedDataAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            //Task 1: Ask the DB if any Roles already exist, if they do then return.
            if (_context.Roles.Any())
            {
                return;
            }

            //Task 2: Create the necessary Roles if they don't exist
            var modRole = new IdentityRole("Administrator");
            await _roleManager.CreateAsync(modRole);

            modRole = new IdentityRole("Moderator");
            await _roleManager.CreateAsync(modRole);
        }

        private async Task SeedUsersAsync()
        {
            //Task 1: Ask the DB if any Users already exist
            if (_context.Users.Any()) return;

            //Task 2: Create the User intended to occupy the Administrator role
            var adminUser = new BlogUser()
            {
                Email = "jmlynn@icloud.com",
                UserName = "jmlynn@icloud.com",
                FirstName = "Jason",
                LastName = "Lynn",
                DisplayName = "Jasa",
                PhoneNumber = "(336) 413-5560",
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(adminUser, "Abc&123!");
            await _userManager.AddToRoleAsync(adminUser, "Administrator");


            //Task 3: Create the User intended to occupy the Moderator role
            var modUser = new BlogUser()
            {
                Email = "jasontwichell@mailinator.com",
                UserName = "jasontwichell@mailinator.com",
                FirstName = "Jason",
                LastName = "Twichell",
                DisplayName = "Twich-man",
                PhoneNumber = "(336) 555-1212",
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(modUser, "Abc&123!");
            await _userManager.AddToRoleAsync(modUser, "Moderator");

        }

    }
}
