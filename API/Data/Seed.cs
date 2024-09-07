using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
            
            List<AppUser> users;
            try
            {
                users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
                if (users == null)
                {
                    Console.WriteLine("No users found in the JSON file.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return;
            }
            var roles = new List<AppRole> {
                new() {Name = "Member"},
                new() {Name = "Admin"},
                new() {Name = "Moderator"},
            };
            foreach (var role in roles){
                await roleManager.CreateAsync(role);
            }
            foreach (var user in users)
            {
                if (user.UserName != null)
                {
                    // using var hmac = new HMACSHA512();
                    user.UserName = user.UserName.ToLower();
                    // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    // user.PasswordSalt = hmac.Key;

                    await userManager.CreateAsync(user,"Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, "Member");
                }
                else
                {
                    Console.WriteLine(user);
                }
            }
            var admin = new AppUser{
                UserName = "admin",
                KnownAs = "Admin",
                Gender = "",
                City = "",
                Country = ""
            };
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
        }
    }
}
