using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

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

            foreach (var user in users)
            {
                if (user.UserName != null)
                {
                    using var hmac = new HMACSHA512();
                    user.UserName = user.UserName.ToLower();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    user.PasswordSalt = hmac.Key;

                    context.Users.Add(user);
                }
                else
                {
                    Console.WriteLine(user);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
