using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
                new() {Name = "Client"},
                new() {Name = "Admin"},
                new() {Name = "Vendor"},
            };
            foreach (var role in roles){
                await roleManager.CreateAsync(role);
            }
            foreach (var user in users)
            {
                if (user.UserName != null)
                {
                    // using var hmac = new HMACSHA512();
                    user.Photos.First().IsApproved = true;
                    user.UserName = user.UserName.ToLower();
                    user.Email = $"{user.UserName}@example.com";
                    user.Amount = 0;
                    // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    // user.PasswordSalt = hmac.Key;

                    await userManager.CreateAsync(user,"Pa$$w0rd");
                    await userManager.AddToRolesAsync(user, ["Vendor","Client"]);
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
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        public static async Task SeedCategory(DataContext context){
            if (await context.Categories.AnyAsync()){
                return;
            }
            var categoriesData = await File.ReadAllTextAsync("Data/CategorySeedData.json");
            var options = new JsonSerializerOptions{
                PropertyNameCaseInsensitive = true
            };
            List<Category> categories;
            try{
                categories = JsonSerializer.Deserialize<List<Category>>(categoriesData,options);
                if (categories == null){
                    Console.WriteLine("No categories found in the JSON file.");
                    return;
                }
            }
            catch(Exception ex) {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return;
            }
            foreach(var category in categories){
                if (category!=null){
                    await context.Categories.AddAsync(category);
                }
            }
            await context.SaveChangesAsync();
        }
        public static async Task SeedProduct(DataContext context){
            if (await context.Products.AnyAsync()){
                return;
            }
            var productsData = await File.ReadAllTextAsync("Data/ProductSeedData.json");
            var options = new JsonSerializerOptions{
                PropertyNameCaseInsensitive = true
            };
            List<Product> products;
            try{
                products = JsonSerializer.Deserialize<List<Product>>(productsData,options);
                if (products == null){
                    Console.WriteLine("No products found in the JSON file.");
                    return;
                }
            }
            catch(Exception ex) {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return;
            }
            int count = 1;
            foreach(var product in products){
                if (product!=null){
                    product.ImageUrl = $"https://cdnp-clouddb-dev.azureedge.net/image/product{++count}.jpg";
                    await context.Products.AddAsync(product);
                }     
            }
            await context.SaveChangesAsync();
        }
    }

}
