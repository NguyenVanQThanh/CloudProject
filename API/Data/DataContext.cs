using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Entities.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
    {
        public DbSet<UserLike> Likes { get; private set; }
        public DbSet<Message> Messages { get; private set; }
        public DbSet<Group> Groups { get; private set; }
        public DbSet<Connection> Connections { get; private set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

            modelBuilder.Entity<AppRole>()
           .HasMany(ur => ur.UserRoles)
           .WithOne(r => r.Role)
           .HasForeignKey(ur => ur.RoleId)
           .IsRequired();


            modelBuilder.Entity<UserLike>()
                .HasKey(ul => new { ul.SourceUserId, ul.TargetUserId });
            modelBuilder.Entity<UserLike>()
                .HasOne(ul => ul.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLike>()
                .HasOne(ul => ul.TargetUser)
                .WithMany(l => l.LikeByUsers)
                .HasForeignKey(t => t.TargetUserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.MessagesReceived)
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
            modelBuilder.Entity<AppUser>()
                .Property(u=>u.Amount)
                .HasPrecision(18, 2);
            //order
            modelBuilder.Entity<Order>()
                .Property(o=>o.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v)
                );
            modelBuilder.Entity<Order>()
                .Property(o=>o.Payment)
                .HasConversion(
                    v => v.ToString(),
                    v => (OrderPayment)Enum.Parse(typeof(OrderPayment), v)
                );
            modelBuilder.Entity<Order>()
                .HasOne(o => o.UserClient)
                .WithMany(u => u.OrderClients)
                .HasForeignKey(o => o.IdClient)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.UserVendor)
                .WithMany(u => u.OrderVendors)
                .HasForeignKey(o => o.IdVendor)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .Property(o=>o.Amount)
                .HasPrecision(18, 2);
            //order details
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderDetail>()
                .Property(od=>od.Price)
                .HasPrecision(18, 2);
            //cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Client)
                .WithMany(u => u.CartClients)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Vendor)
                .WithMany(u => u.CartVendors)
                .HasForeignKey(c => c.VendorId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Cart>()
                .Property(c=>c.TotalPrice)
                .HasPrecision(18, 2);
            //cart items
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartItem>()
                .Property(ct=>ct.Price)
                .HasPrecision(18, 2);
            // product
            // modelBuilder.Entity<Product>()
            //     .HasIndex(p=>p.Name)
            //     .IsUnique();
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Vendor)
                .WithMany(c => c.VendorProducts)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Product>()
                .Property(p=>p.Price)
                .HasPrecision(18, 2);
            //category
            modelBuilder.Entity<Category>()
                .HasIndex(c=>c.Name)
                .IsUnique();
        }
    }
}