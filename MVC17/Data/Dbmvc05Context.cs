using System;
using System.Collections.Generic;
using MVC17.Models;
using Microsoft.EntityFrameworkCore;

namespace MVC17.Data;

public partial class Dbmvc05Context : DbContext
{
    public Dbmvc05Context()
    {
    }

    public Dbmvc05Context(DbContextOptions<Dbmvc05Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Access> Accesses { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Cpu> Cpus { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Gpu> Gpus { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<InvoiceStatus> InvoiceStatuses { get; set; }

    public virtual DbSet<Laptop> Laptops { get; set; }

    public virtual DbSet<LaptopComponent> LaptopComponents { get; set; }

    public virtual DbSet<PersonalInformation> PersonalInformations { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSku> ProductSkus { get; set; }

    public virtual DbSet<Ram> Rams { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RouteEntity> RouteEntities { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwCpuSpec> VwCpuSpecs { get; set; }

    public virtual DbSet<VwCustomerProfile> VwCustomerProfiles { get; set; }

    public virtual DbSet<VwGpuSpec> VwGpuSpecs { get; set; }

    public virtual DbSet<VwInvoice> VwInvoices { get; set; }

    public virtual DbSet<VwInvoiceDetail> VwInvoiceDetails { get; set; }

    public virtual DbSet<VwLaptopSpec> VwLaptopSpecs { get; set; }

    public virtual DbSet<VwOrderedItem> VwOrderedItems { get; set; }

    public virtual DbSet<VwProduct> VwProducts { get; set; }

    public virtual DbSet<VwRamSpec> VwRamSpecs { get; set; }

    public virtual DbSet<VwShoppingCart> VwShoppingCarts { get; set; }

    public virtual DbSet<VwStorageSpec> VwStorageSpecs { get; set; }

    public virtual DbSet<VwTrendingProduct> VwTrendingProducts { get; set; }

    public virtual DbSet<VwsCancelledOrder> VwsCancelledOrders { get; set; }

    public virtual DbSet<VwsCompletedOrder> VwsCompletedOrders { get; set; }

    public virtual DbSet<VwsLowStockProduct> VwsLowStockProducts { get; set; }

    public virtual DbSet<VwsPendingOrder> VwsPendingOrders { get; set; }

    public virtual DbSet<VwsRefundedOrder> VwsRefundedOrders { get; set; }

    public virtual DbSet<VwsRevenueByCategory> VwsRevenueByCategories { get; set; }

    public virtual DbSet<VwsRevenueByCustomer> VwsRevenueByCustomers { get; set; }

    public virtual DbSet<VwsRevenueByEmployee> VwsRevenueByEmployees { get; set; }

    public virtual DbSet<VwsRevenueByProduct> VwsRevenueByProducts { get; set; }

    public virtual DbSet<VwsRevenueBySupplier> VwsRevenueBySuppliers { get; set; }

    public virtual DbSet<VwsShippingOrder> VwsShippingOrders { get; set; }

    public virtual DbSet<VwsTotalCustomer> VwsTotalCustomers { get; set; }

    public virtual DbSet<VwsTotalProduct> VwsTotalProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection string is supplied via Dependency Injection (Program.cs).
        // This fallback guard is intentionally left without a hardcoded string so
        // that missing configuration is caught early with a clear error message.
        if (!optionsBuilder.IsConfigured)
        {
            throw new InvalidOperationException(
                "DbContext is not configured. Ensure the connection string is provided " +
                "via the SQL_CONNECTION_STRING environment variable or appsettings.json.");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Access>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.RouteId }).HasName("PK__Accesses__42F3B7AE4F24265C");

            entity.Property(e => e.RoleId).ValueGeneratedOnAdd();
            entity.Property(e => e.IsAllowed).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Accesses)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accesses__RoleId__498EEC8D");

            entity.HasOne(d => d.Route).WithMany(p => p.Accesses)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accesses__RouteI__40F9A68C");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.Property(e => e.AddedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_Products");

            entity.HasOne(d => d.ShoppingCart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ShoppingCartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_ShoppingCarts");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B55594EC2");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E0E5E1557C").IsUnique();

            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cpu>(entity =>
        {
            entity.HasKey(e => e.CpuId).HasName("PK__Cpus__CAD9B6B3CB92DB0A");

            entity.HasIndex(e => e.ProductSkuId, "IX_Cpus").IsUnique();

            entity.Property(e => e.Socket)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.ProductSku).WithOne(p => p.Cpu)
                .HasForeignKey<Cpu>(d => d.ProductSkuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cpus__ProductSku__162F4418");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D808DD8B5C");

            entity.HasIndex(e => e.CustomerCode, "IX_Customers").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__Customer__1788CC4DE0F45D46").IsUnique();

            entity.HasIndex(e => e.Piid, "UQ__Customer__5F86BE41AEB4A8B6").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CustomerCode)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Piid).HasColumnName("PIId");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Pi).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.Piid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Customers__PIId__42E1EEFE");

            entity.HasOne(d => d.User).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Customers__UserI__43D61337");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BED41A740EB");

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ParentDepartment).WithMany(p => p.InverseParentDepartment)
                .HasForeignKey(d => d.ParentDepartmentId)
                .HasConstraintName("FK__Departmen__Paren__45BE5BA9");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasIndex(e => e.DiscountCode, "IX_Discounts").IsUnique();

            entity.Property(e => e.Amount).HasDefaultValue(1);
            entity.Property(e => e.DiscountCode)
                .HasMaxLength(12)
                .IsFixedLength();
            entity.Property(e => e.ExpiredDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasDefaultValue("Product");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F1102C23FF0");

            entity.HasIndex(e => e.EmployeeCode, "IX_Employees").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__Employee__1788CC4D7315326A").IsUnique();

            entity.HasIndex(e => e.Piid, "UQ__Employee__5F86BE412CC3F7B0").IsUnique();

            entity.Property(e => e.CompanyEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.Piid).HasColumnName("PIId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("IsActive");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Employees__Depar__46B27FE2");

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__Employees__Manag__47A6A41B");

            entity.HasOne(d => d.Pi).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.Piid)
                .HasConstraintName("FK__Employees__PIId__489AC854");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Employees__Posit__498EEC8D");

            entity.HasOne(d => d.User).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__UserI__4A8310C6");
        });

        modelBuilder.Entity<Gpu>(entity =>
        {
            entity.HasKey(e => e.GpuId).HasName("PK__Gpus__E542DFA4BA587785");

            entity.HasIndex(e => e.ProductSkuId, "IX_Gpus").IsUnique();

            entity.Property(e => e.Bus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Igpu).HasDefaultValue(false);
            entity.Property(e => e.MemoryType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.ProductSku).WithOne(p => p.Gpu)
                .HasForeignKey<Gpu>(d => d.ProductSkuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Gpus__ProductSku__17236851");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F70CC653BBB3");

            entity.Property(e => e.ImageUrl).HasDefaultValue("~img/NotFoundImage.png");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB556CB24B1");

            entity.HasIndex(e => e.InvoiceUuid, "IX_Invoices").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.InvoiceUuid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ShippedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.ShippingFee).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoices__Custom__2C1E8537");

            entity.HasOne(d => d.Employee).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Invoices__Employ__2D12A970");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceDetailId).HasName("PK__InvoiceD__1F157811E977A21A");

            entity.Property(e => e.LineTotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InvoiceDe__Invoi__2E06CDA9");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InvoiceDe__Produ__2EFAF1E2");
        });

        modelBuilder.Entity<InvoiceStatus>(entity =>
        {
            entity.HasKey(e => e.Status);

            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.NameVi)
                .HasMaxLength(50)
                .HasColumnName("NameVI");
        });

        modelBuilder.Entity<Laptop>(entity =>
        {
            entity.HasKey(e => e.LaptopId).HasName("PK__Laptops__19F0268441A25D26");

            entity.HasIndex(e => e.LaptopId, "IX_Laptops");

            entity.HasIndex(e => e.ProductSkuId, "IX_Laptops_1").IsUnique();

            entity.Property(e => e.LaptopType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Os)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OS");
            entity.Property(e => e.ScreenResolution)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.LaptopComponent).WithMany(p => p.Laptops)
                .HasForeignKey(d => d.LaptopComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Laptops_LaptopComponents");

            entity.HasOne(d => d.ProductSku).WithOne(p => p.Laptop)
                .HasForeignKey<Laptop>(d => d.ProductSkuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Laptops__Product__19FFD4FC");
        });

        modelBuilder.Entity<LaptopComponent>(entity =>
        {
            entity.HasOne(d => d.Cpu).WithMany(p => p.LaptopComponents)
                .HasForeignKey(d => d.CpuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LComponents__CpuId__1BE81D6E");

            entity.HasOne(d => d.Gpu).WithMany(p => p.LaptopComponents)
                .HasForeignKey(d => d.GpuId)
                .HasConstraintName("FK_LaptopComponents_Gpus");

            entity.HasOne(d => d.Ram).WithMany(p => p.LaptopComponents)
                .HasForeignKey(d => d.RamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LComponents__RamId__1CDC41A7");

            entity.HasOne(d => d.Storage).WithMany(p => p.LaptopComponents)
                .HasForeignKey(d => d.StorageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LComponents__Memor__1DD065E0");
        });

        modelBuilder.Entity<PersonalInformation>(entity =>
        {
            entity.HasKey(e => e.Piid).HasName("PK__Personal__5F86BE40C72AFA32");

            entity.Property(e => e.Piid).HasColumnName("PIId");
            entity.Property(e => e.CitizenIdentityCard)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A794D490ED4");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Level).HasDefaultValue(1);
            entity.Property(e => e.PositionName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD1FA14FEA");

            entity.HasIndex(e => e.ProductUuid, "UQ__Products__B19E2D340F087441").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProductUuid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__125EB334");

            entity.HasOne(d => d.Image).WithMany(p => p.Products)
                .HasForeignKey(d => d.ImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__ImageI__1446FBA6");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Suppli__1352D76D");
        });

        modelBuilder.Entity<ProductSku>(entity =>
        {
            entity.HasKey(e => e.ProductSkuId).HasName("PK__ProductS__3A8042B4135B4ECB");

            entity.HasIndex(e => e.ProductId, "IX_ProductSkus").IsUnique();

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 2)");

            entity.HasOne(d => d.Product).WithOne(p => p.ProductSku)
                .HasForeignKey<ProductSku>(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductSkus_Products");
        });

        modelBuilder.Entity<Ram>(entity =>
        {
            entity.HasKey(e => e.RamId).HasName("PK__Rams__8840B277DA01005D");

            entity.HasIndex(e => e.ProductSkuId, "IX_Rams").IsUnique();

            entity.Property(e => e.Gen)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Kit)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ProductSku).WithOne(p => p.Ram)
                .HasForeignKey<Ram>(d => d.ProductSkuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rams__ProductSku__18178C8A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AE34DC21C");

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RouteEntity>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__RouteEnt__80979B4D6210C13B");

            entity.HasIndex(e => e.RouteCode, "UQ__RouteEnt__FDC34585989AEF86").IsUnique();

            entity.Property(e => e.Endpoint)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Method)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Module)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Path)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RouteCode)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.ShoppingCartId).HasName("PK_Carts");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.Subtotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ShoppingCarts_Users");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.StorageId).HasName("PK__Memories__9A4986D48902E106");

            entity.HasIndex(e => e.ProductSkuId, "IX_Memories").IsUnique();

            entity.Property(e => e.InterfaceType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MemoryType)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ProductSku).WithOne(p => p.Storage)
                .HasForeignKey<Storage>(d => d.ProductSkuId)
                .HasConstraintName("FK__Memories__Produc__190BB0C3");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B43C667970");

            entity.HasIndex(e => e.CompanyName, "UQ__Supplier__9BCE05DC27914076").IsUnique();

            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C4BBDA3EC");

            entity.HasIndex(e => e.UserUuid, "UQ__Users__2DCB0AEA2717A278").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053457A931B2").IsUnique();

            entity.Property(e => e.Balance).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasDefaultValue(1);
            entity.Property(e => e.UserUuid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__5D95E53A");
        });

        modelBuilder.Entity<VwCpuSpec>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CpuSpecs");

            entity.Property(e => e.CpuName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Socket)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwCustomerProfile>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CustomerProfiles");

            entity.Property(e => e.AccountEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Balance).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.CitizenIdentityCard)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CustomerCode)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PersonalEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwGpuSpec>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_GpuSpecs");

            entity.Property(e => e.Bus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.GpuName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MemoryType)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwInvoice>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Invoices");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.ShippingFee).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(15, 2)");
        });

        modelBuilder.Entity<VwInvoiceDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_InvoiceDetails");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LineTotal).HasColumnType("decimal(26, 2)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 2)");
        });

        modelBuilder.Entity<VwLaptopSpec>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_LaptopSpecs");

            entity.Property(e => e.Bus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CpuName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gen)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.GpuName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InterfaceType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Kit)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.LaptopName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LaptopType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MemoryType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Os)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OS");
            entity.Property(e => e.RamName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ScreenResolution)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Socket)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StorageName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StorageType)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwOrderedItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_OrderedItem");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LineTotal).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 2)");
        });

        modelBuilder.Entity<VwProduct>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Products");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 2)");
        });

        modelBuilder.Entity<VwRamSpec>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_RamSpecs");

            entity.Property(e => e.Gen)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Kit)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RamName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwShoppingCart>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ShoppingCarts");

            entity.Property(e => e.LineTotal).HasColumnType("decimal(26, 2)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 2)");
        });

        modelBuilder.Entity<VwStorageSpec>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StorageSpecs");

            entity.Property(e => e.InterfaceType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StorageType)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwTrendingProduct>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TrendingProducts");

            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 2)");
        });

        modelBuilder.Entity<VwsCancelledOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_CancelledOrders");

            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwsCompletedOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_CompletedOrders");

            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwsLowStockProduct>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_LowStockProducts");

            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwsPendingOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_PendingOrders");

            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwsRefundedOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_RefundedOrders");

            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwsRevenueByCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_RevenueByCategories");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwsRevenueByCustomer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_RevenueByCustomers");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwsRevenueByEmployee>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_RevenueByEmployees");

            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwsRevenueByProduct>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_RevenueByProducts");

            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwsRevenueBySupplier>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_RevenueBySuppliers");

            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwsShippingOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_ShippingOrders");

            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwsTotalCustomer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_TotalCustomers");
        });

        modelBuilder.Entity<VwsTotalProduct>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vws_TotalProducts");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
