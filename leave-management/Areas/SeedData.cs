﻿using leave_management.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Areas
{
    public static class SeedData
    {
        //public static void Seed(UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager)
        public static void Seed(UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        //private static void SeedUsers(UserManager<Employee> userManager)
        private static void SeedUsers(UserManager<Employee> userManager)
        {
            if(userManager.FindByNameAsync("Admin").Result==null )
            {
                var user = new Employee
                {
                    UserName = "Admin",
                    Email = "admin@local.com"
                };
                var result = userManager.CreateAsync(user, "Ashish@100").Result;
                if(result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }
        //private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if(!roleManager.RoleExistsAsync("Administrator").Result)
            {
                var role = new IdentityRole { Name = "Administrator" };
                var result=roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Employee").Result)
            {
                var role = new IdentityRole { Name = "Employee" };
                var result = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
