using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Data.SeedData
{
    public class DataContextSeed
    {
        public static async Task SeedUserData(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            try
            {
                // seed the AppUser, UserSeedData.json file
                if (await userManager.Users.AnyAsync()) // if any data just return
                {
                    return;
                }
                // create a list of AppRoles
                var roles = new List<AppRole>
                {
                    new AppRole { Name = "Member"},
                    new AppRole { Name = "Admin"},
                    new AppRole { Name = "Moderator"},
                                    
                };
                foreach(var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
                // here I went with Async 
                var userData = await File.ReadAllTextAsync("Data/SeedData/UserSeedData.json"); // path is relative to the API project
                //var userData = await File.ReadAllTextAsync("Data/SeedData/UserPhotosCommentsData.json"); // path is relative to the API project
                var users = JsonSerializer.Deserialize<List<AppUser>>(userData); // we dont use Newtonsoft Deserializer in this app
                foreach (var user in users)
                {
                    user.UserName = user.UserName.ToLower();
                    // creates and saved the user along with its password whoch is hardcoded to be "password"
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    // add user to a specific role
                    await userManager.AddToRoleAsync(user, "Member");
                };

                var admin = new AppUser
                {
                    UserName = "admin",
                    Gender = "Male"
                };

                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
