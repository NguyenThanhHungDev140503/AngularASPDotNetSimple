using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models.Entities;
using ProductAPI.Models.Enums;
using Xunit;

namespace ProductAPI.Tests
{
    public class DatabaseSchemaTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public DatabaseSchemaTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=MYLAPTOP\\SQLEXPRESS02;Database=TheShoe_Test;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public void CanCreateUser()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                SoDienThoai = "0123456789",
                Password = "hashedpassword"
            };

            // Act
            _context.Users.Add(user);
            _context.SaveChanges();

            // Assert
            var savedUser = _context.Users.FirstOrDefault(u => u.Email == "test@example.com");
            Assert.NotNull(savedUser);
            Assert.Equal("Test User", savedUser.Name);
        }

        [Fact]
        public void CanCreateOrderWithDetails()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "order@example.com",
                SoDienThoai = "0987654321",
                Password = "hashedpassword"
            };

            var category = new Category
            {
                Name = "Test Category",
                Description = "Test Description"
            };

            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Product Description",
                StockPrice = 100000m,
                Price = 150000m,
                StockQuantity = 10,
                Category = category
            };

            var order = new Order
            {
                User = user,
                Status = OrderStatus.Pending,
                TotalAmount = 150000m
            };

            var orderDetail = new OrderDetail
            {
                Order = order,
                Product = product,
                Quantity = 1,
                PriceAtPurchase = 150000m
            };

            // Act
            _context.Users.Add(user);
            _context.Categories.Add(category);
            _context.Products.Add(product);
            _context.Orders.Add(order);
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();

            // Assert
            var savedOrder = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.User.Email == "order@example.com");

            Assert.NotNull(savedOrder);
            Assert.Equal(OrderStatus.Pending, savedOrder.Status);
            Assert.Single(savedOrder.OrderDetails);
            Assert.Equal("Test Product", savedOrder.OrderDetails.First().Product.Name);
        }

        [Fact]
        public void CanCreateUserWithRoles()
        {
            // Arrange
            var user = new User
            {
                Name = "Admin User",
                Email = "admin@example.com",
                SoDienThoai = "0111222333",
                Password = "hashedpassword"
            };

            var role = new Role
            {
                Name = "Administrator",
                Description = "System Administrator"
            };

            var userRole = new UserRole
            {
                User = user,
                Role = role
            };

            // Act
            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.UserRoles.Add(userRole);
            _context.SaveChanges();

            // Assert
            var savedUser = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Email == "admin@example.com");

            Assert.NotNull(savedUser);
            Assert.Single(savedUser.UserRoles);
            Assert.Equal("Administrator", savedUser.UserRoles.First().Role.Name);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
