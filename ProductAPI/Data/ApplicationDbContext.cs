using Microsoft.EntityFrameworkCore;
using ProductAPI.Models.Entities;
using ProductAPI.Models.Enums;

namespace ProductAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionProduct> PromotionProducts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Shipping> Shippings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.StockPrice)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)")
                    .HasColumnName("stock_price");

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)")
                    .HasColumnName("price");

                entity.Property(e => e.StockQuantity)
                    .IsRequired()
                    .HasColumnName("stock_quantity");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                // Configure relationship
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.SoDienThoai)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("sodienthoai");

                entity.HasIndex(e => e.SoDienThoai).IsUnique();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Permission entity
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure UserRole entity (Many-to-Many)
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            // Configure RolePermission entity (Many-to-Many)
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId });

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId);
            });

            // Configure DiscountCode entity
            modelBuilder.Entity<DiscountCode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.Code).IsUnique();

                entity.Property(e => e.DiscountPercentage)
                    .IsRequired()
                    .HasColumnName("discount_percentage")
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.MaxUses)
                    .IsRequired()
                    .HasColumnName("max_uses");

                entity.Property(e => e.UsesCount)
                    .HasColumnName("uses_count")
                    .HasDefaultValue(0);

                entity.Property(e => e.MinOrderValue)
                    .HasColumnName("min_order_value")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.StartDate)
                    .IsRequired()
                    .HasColumnName("start_date");

                entity.Property(e => e.EndDate)
                    .IsRequired()
                    .HasColumnName("end_date");
            });

            // Configure PaymentMethod entity
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasIndex(e => e.Code).IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Provider)
                    .HasMaxLength(50);

                entity.Property(e => e.FeePercent)
                    .HasColumnName("fee_percent")
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Promotion entity
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.DiscountPercentage)
                    .IsRequired()
                    .HasColumnName("discount_percentage")
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.StartDate)
                    .IsRequired()
                    .HasColumnName("start_date");

                entity.Property(e => e.EndDate)
                    .IsRequired()
                    .HasColumnName("end_date");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Address entity
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("postal_code");

                entity.Property(e => e.IsDefault)
                    .HasColumnName("is_default")
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(a => a.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Wishlist entity
            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(w => w.User)
                    .WithMany(u => u.Wishlists)
                    .HasForeignKey(w => w.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Cart entity
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                entity.HasIndex(e => e.UserId).IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(c => c.User)
                    .WithOne(u => u.Cart)
                    .HasForeignKey<Cart>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Review entity
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnName("product_id");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                entity.Property(e => e.Rating)
                    .IsRequired();

                entity.Property(e => e.Comment)
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(r => r.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(e => e.TotalAmount)
                    .IsRequired()
                    .HasColumnName("total_amount")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.DiscountCodeId)
                    .HasColumnName("discount_code_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.DiscountCode)
                    .WithMany(dc => dc.Orders)
                    .HasForeignKey(o => o.DiscountCodeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnName("order_id");

                entity.Property(e => e.PaymentMethodId)
                    .IsRequired()
                    .HasColumnName("payment_method_id");

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasDefaultValue(PaymentStatus.Pending);

                entity.Property(e => e.ProviderTxnId)
                    .HasMaxLength(255)
                    .HasColumnName("provider_txn_id");

                entity.Property(e => e.ProviderFee)
                    .HasColumnName("provider_fee")
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.Metadata)
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.PaidAt)
                    .HasColumnName("paid_at");

                entity.Property(e => e.IdempotencyKey)
                    .HasColumnName("idempotency_key");

                entity.HasIndex(e => e.IdempotencyKey).IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(p => p.Order)
                    .WithMany(o => o.Payments)
                    .HasForeignKey(p => p.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.PaymentMethod)
                    .WithMany(pm => pm.Payments)
                    .HasForeignKey(p => p.PaymentMethodId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure Shipping entity
            modelBuilder.Entity<Shipping>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnName("order_id");

                entity.Property(e => e.AddressId)
                    .IsRequired()
                    .HasColumnName("address_id");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasDefaultValue(ShippingStatus.Pending);

                entity.Property(e => e.ShipperId)
                    .HasColumnName("shipper_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(s => s.Order)
                    .WithOne(o => o.Shipping)
                    .HasForeignKey<Shipping>(s => s.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Address)
                    .WithMany(a => a.Shippings)
                    .HasForeignKey(s => s.AddressId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(s => s.Shipper)
                    .WithMany(u => u.ShippingsAsShipper)
                    .HasForeignKey(s => s.ShipperId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure OrderDetail entity (Many-to-Many)
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id");

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.PriceAtPurchase)
                    .IsRequired()
                    .HasColumnName("price_at_purchase")
                    .HasColumnType("decimal(10,2)");

                entity.HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure CartItem entity (Many-to-Many)
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => new { e.CartId, e.ProductId });

                entity.Property(e => e.CartId)
                    .HasColumnName("cart_id");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id");

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.HasOne(ci => ci.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PromotionProduct entity (Many-to-Many)
            modelBuilder.Entity<PromotionProduct>(entity =>
            {
                entity.HasKey(e => new { e.PromotionId, e.ProductId });

                entity.Property(e => e.PromotionId)
                    .HasColumnName("promotion_id");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id");

                entity.HasOne(pp => pp.Promotion)
                    .WithMany(p => p.PromotionProducts)
                    .HasForeignKey(pp => pp.PromotionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pp => pp.Product)
                    .WithMany(p => p.PromotionProducts)
                    .HasForeignKey(pp => pp.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data - commented out for production use
            // Uncomment the line below if you want to seed initial data
            // SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            var categories = new[]
            {
                new Category
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Giày thể thao",
                    Description = "Giày dành cho hoạt động thể thao và tập luyện",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Giày công sở",
                    Description = "Giày lịch sự dành cho môi trường công sở",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Giày casual",
                    Description = "Giày thường ngày, phong cách thoải mái",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<Category>().HasData(categories);

            // Seed Products
            var products = new[]
            {
                new Product
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Nike Air Max 270",
                    Description = "Giày thể thao Nike Air Max 270 với công nghệ đệm khí tiên tiến",
                    StockPrice = 1500000m,
                    Price = 2200000m,
                    StockQuantity = 50,
                    CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "Adidas Ultraboost 22",
                    Description = "Giày chạy bộ Adidas Ultraboost 22 với công nghệ Boost",
                    StockPrice = 1800000m,
                    Price = 2500000m,
                    StockQuantity = 30,
                    CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    Name = "Clarks Desert Boot",
                    Description = "Giày boot da lộn Clarks Desert Boot phong cách cổ điển",
                    StockPrice = 1200000m,
                    Price = 1800000m,
                    StockQuantity = 25,
                    CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<Product>().HasData(products);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Product || e.Entity is Category || e.Entity is User ||
                           e.Entity is Role || e.Entity is Permission || e.Entity is Promotion ||
                           e.Entity is Address || e.Entity is Wishlist || e.Entity is Cart ||
                           e.Entity is Order || e.Entity is Payment || e.Entity is Shipping)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    switch (entry.Entity)
                    {
                        case Product product:
                            product.CreatedAt = DateTime.UtcNow;
                            product.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Category category:
                            category.CreatedAt = DateTime.UtcNow;
                            category.UpdatedAt = DateTime.UtcNow;
                            break;
                        case User user:
                            user.CreatedAt = DateTime.UtcNow;
                            user.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Role role:
                            role.CreatedAt = DateTime.UtcNow;
                            role.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Permission permission:
                            permission.CreatedAt = DateTime.UtcNow;
                            permission.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Promotion promotion:
                            promotion.CreatedAt = DateTime.UtcNow;
                            promotion.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Address address:
                            address.CreatedAt = DateTime.UtcNow;
                            address.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Wishlist wishlist:
                            wishlist.CreatedAt = DateTime.UtcNow;
                            wishlist.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Cart cart:
                            cart.CreatedAt = DateTime.UtcNow;
                            cart.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Order order:
                            order.CreatedAt = DateTime.UtcNow;
                            order.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Payment payment:
                            payment.CreatedAt = DateTime.UtcNow;
                            payment.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Shipping shipping:
                            shipping.CreatedAt = DateTime.UtcNow;
                            shipping.UpdatedAt = DateTime.UtcNow;
                            break;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    switch (entry.Entity)
                    {
                        case Product product:
                            product.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Category category:
                            category.UpdatedAt = DateTime.UtcNow;
                            break;
                        case User user:
                            user.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Role role:
                            role.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Permission permission:
                            permission.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Promotion promotion:
                            promotion.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Address address:
                            address.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Wishlist wishlist:
                            wishlist.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Cart cart:
                            cart.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Order order:
                            order.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Payment payment:
                            payment.UpdatedAt = DateTime.UtcNow;
                            break;
                        case Shipping shipping:
                            shipping.UpdatedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }
        }
    }
}
