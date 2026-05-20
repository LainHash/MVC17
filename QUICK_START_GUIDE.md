# 🔧 QUICK START GUIDE - MVC17

## Hướng Dẫn Nhanh Cho Developers

---

## 1️⃣ SETUP BAN ĐẦU

### Prerequisites

```
✅ .NET 8.0 SDK installed
✅ SQL Server 2019+ running
✅ Visual Studio 2022 or VS Code
✅ Git installed
```

### Clone & Setup

```bash
# Clone repository
git clone <repo-url>
cd MVC17

# Restore packages
dotnet restore

# Configure connection string (appsettings.json)
# Update DefaultConnection with your SQL Server

# Run migrations
dotnet ef database update

# Start application
dotnet run
```

### Default URLs

```
🔗 Development: https://localhost:5001
🔗 HTTP: http://localhost:5000
```

---

## 2️⃣ CẤUTRÚC DỰ ÁN

### Folder Organization

```
MVC17/
│
├── Controllers/          # HTTP request handlers
│   ├── HomeController.cs
│   ├── ProductController.cs
│   ├── CartController.cs
│   ├── OrderController.cs
│   ├── AccountController.cs
│   └── StatisticController.cs
│
├── Models/              # Database entities
│   ├── Product.cs
│   ├── Laptop.cs
│   ├── Invoice.cs
│   └── ... (40+ files)
│
├── Views/               # Razor templates
│   ├── Product/
│   ├── Cart/
│   ├── Order/
│   ├── Account/
│   └── Shared/
│
├── Services/            # Business logic
│   ├── Interfaces/
│   └── Implementations/
│
├── DTOs/               # Data transfer objects
│   ├── Products/
│   ├── Orders/
│   └── Accounts/
│
├── ViewModels/         # View data models
│   ├── ProductVM.cs
│   ├── OrderHistoryVM.cs
│   └── ...
│
├── ViewComponents/     # Reusable UI components
│   ├── CategoryMenu.cs
│   ├── ProductSpec.cs
│   └── Pagination.cs
│
├── Mappings/          # AutoMapper profiles
│   ├── ProductMP.cs
│   ├── OrderMP.cs
│   └── AccountMP.cs
│
├── Helpers/           # Utilities & constants
│   ├── Constants/
│   └── Utils/
│
├── Data/              # Database context
│   └── Dbmvc05Context.cs
│
├── wwwroot/           # Static files (CSS, JS, images)
│   ├── css/
│   ├── js/
│   └── images/
│
├── Program.cs         # Application startup
├── appsettings.json   # Configuration
└── MVC17.csproj       # Project file
```

---

## 3️⃣ KEY COMPONENTS

### Program.cs - Startup Configuration

```csharp
// Services configuration
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
builder.Services.AddSession();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// Middleware configuration
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(...);
```

### DbContext - Database

```csharp
// Models are mapped to database tables
// Relationships are configured via Fluent API
// Supports LINQ queries

var products = await _context.Products
    .Include(p => p.Category)
    .Where(p => p.IsActive)
    .ToListAsync();
```

### Controllers - Request Handling

```csharp
public class ProductController : Controller
{
    private readonly Dbmvc05Context _context;
    private readonly IMapper _mapper;
    private readonly IProductService _service;

    public async Task<IActionResult> Index()
    {
        // Get data from service/repository
        var products = await _service.GetProductsAsync();

        // Map to ViewModel
        var vm = _mapper.Map<List<ProductVM>>(products);

        // Return view
        return View(vm);
    }
}
```

### Services - Business Logic

```csharp
public class ProductService : IProductService
{
    private readonly Dbmvc05Context _context;

    public async Task<Product> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            // ... map other properties
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }
}
```

### ViewModels - Data for Views

```csharp
public class ProductVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; }
    // Only properties needed in View
}
```

---

## 4️⃣ COMMON TASKS

### Add New Feature

#### Step 1: Create Model

```csharp
public class NewEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    // Add properties
}
```

#### Step 2: Add to DbContext

```csharp
public DbSet<NewEntity> NewEntities { get; set; }
```

#### Step 3: Create Migration

```bash
dotnet ef migrations add AddNewEntity
dotnet ef database update
```

#### Step 4: Create DTO & ViewModel

```csharp
public class NewEntityDto { }
public class NewEntityVM { }
```

#### Step 5: Create Service

```csharp
public interface INewEntityService { }
public class NewEntityService : INewEntityService { }
```

#### Step 6: Register in Program.cs

```csharp
builder.Services.AddScoped<INewEntityService, NewEntityService>();
```

#### Step 7: Create Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class NewEntityController : ControllerBase
{
    private readonly INewEntityService _service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllAsync();
        return Ok(data);
    }
}
```

#### Step 8: Create Views

```html
@model List<NewEntityVM>
  @foreach(var item in Model) {
  <div>@item.Name</div>
  }</NewEntityVM
>
```

### Add Authentication to Action

```csharp
[Authorize(Roles = "Admin")]
public IActionResult AdminOnly()
{
    return View();
}

[Authorize(Roles = "Admin,Manager")]
public IActionResult AdminOrManager()
{
    return View();
}
```

### Query Database

```csharp
// Get all
var all = await _context.Products.ToListAsync();

// Get with condition
var active = await _context.Products
    .Where(p => p.IsActive)
    .ToListAsync();

// Get single
var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

// Get with related data
var products = await _context.Products
    .Include(p => p.Category)
    .Include(p => p.Supplier)
    .ToListAsync();

// Count
var count = await _context.Products.CountAsync();

// Order & paginate
var page = await _context.Products
    .OrderByDescending(p => p.CreatedDate)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### Map Objects with AutoMapper

```csharp
// Configure in MappingProfile
cfg.CreateMap<Product, ProductVM>();

// Use in code
var vm = _mapper.Map<ProductVM>(product);
var vms = _mapper.Map<List<ProductVM>>(products);
```

### Send Email

```csharp
// Using MailKit
using MailKit.Net.Smtp;
using MimeKit;

var email = new MimeMessage();
email.From.Add(new MailboxAddress("sender", "sender@example.com"));
email.To.Add(new MailboxAddress("recipient", "recipient@example.com"));
email.Subject = "Subject";
email.Body = new TextPart("html") { Text = "<h1>Hello</h1>" };

using var smtp = new SmtpClient();
await smtp.ConnectAsync("smtp.server.com", 587, SecureSocketOptions.StartTls);
await smtp.AuthenticateAsync("username", "password");
await smtp.SendAsync(email);
await smtp.DisconnectAsync(true);
```

### Handle JWT Authentication

```csharp
// Generate token
var tokenHandler = new JwtSecurityTokenHandler();
var key = Encoding.ASCII.GetBytes(jwtSecret);
var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Role, userRole)
    }),
    Expires = DateTime.UtcNow.AddHours(1),
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature)
});

var tokenString = tokenHandler.WriteToken(token);
Response.Cookies.Append("jwt", tokenString,
    new CookieOptions { HttpOnly = true });
```

---

## 5️⃣ DEBUGGING

### Enable Logging

```csharp
// In appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Debug",
      "MVC17": "Debug"
    }
  }
}
```

### View SQL Queries

```csharp
// In DbContext OnConfiguring
optionsBuilder.LogTo(Console.WriteLine);

// Or with filters
optionsBuilder.LogTo(Console.WriteLine,
    new[] { DbLoggerCategory.Database.Command.Name });
```

### Breakpoints & Stepping

```csharp
// Set breakpoint by clicking line number
// F5 = Start debugging
// F10 = Step over
// F11 = Step into
// Shift+F11 = Step out
// Ctrl+Shift+F5 = Restart
```

### Check Database

```bash
# Using Entity Framework CLI
dotnet ef dbcontext info
dotnet ef migrations list
dotnet ef database info
```

---

## 6️⃣ PERFORMANCE TIPS

### 1. Use Async/Await

```csharp
// ❌ Bad
var products = _context.Products.ToList();

// ✅ Good
var products = await _context.Products.ToListAsync();
```

### 2. Use Include for Related Data

```csharp
// ❌ Bad - N+1 problem
var products = _context.Products.ToList();
foreach(var p in products)
{
    var category = _context.Categories.Find(p.CategoryId);
}

// ✅ Good - Single query
var products = await _context.Products
    .Include(p => p.Category)
    .ToListAsync();
```

### 3. Use AsNoTracking for Read-Only

```csharp
// ✅ Faster for read-only queries
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync();
```

### 4. Pagination for Large Data

```csharp
// ✅ Limit results
var pageSize = 20;
var page = 1;
var products = await _context.Products
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 5. Index Important Columns

```csharp
// In DbContext
modelBuilder.Entity<Product>()
    .HasIndex(p => p.CategoryId);

modelBuilder.Entity<Invoice>()
    .HasIndex(i => i.CustomerId);
```

---

## 7️⃣ DEPLOYMENT CHECKLIST

### Before Production

- [ ] Remove debug logging
- [ ] Set Release configuration
- [ ] Update appsettings.json for production
- [ ] Update database connection string
- [ ] Generate JWT keys
- [ ] Configure email settings
- [ ] Set up SSL/HTTPS
- [ ] Enable CORS if needed
- [ ] Run load tests
- [ ] Test all features
- [ ] Backup database
- [ ] Setup monitoring

### Deployment Steps

```bash
# 1. Build release
dotnet publish -c Release -o ./publish

# 2. Create web.config for IIS
# 3. Copy publish folder to web server
# 4. Configure IIS application
# 5. Setup SSL certificate
# 6. Run migrations on production DB
# 7. Verify application
# 8. Setup backups
# 9. Monitor performance
```

---

## 8️⃣ TROUBLESHOOTING

### Database Connection Issues

```
Error: Connection string invalid
Solution: Check appsettings.json connection string
         Verify SQL Server is running
         Check credentials and permissions
```

### Migration Errors

```
Error: Migration X is pending
Solution: dotnet ef database update

Error: Model snapshot mismatch
Solution: dotnet ef migrations remove (undo last)
         Make code changes
         dotnet ef migrations add NewMigration
```

### Authentication Issues

```
Error: Unauthorized 401
Solution: Check JWT token validity
         Verify token in cookie/header
         Check [Authorize] attributes

Error: Forbidden 403
Solution: Check user roles match [Authorize(Roles="...")]
         Verify claims are correct
```

### Performance Issues

```
Solution: Profile using SQL profiler
         Check for N+1 queries
         Add indexes to frequently queried columns
         Use caching for read-heavy data
         Enable compression
```

---

## 9️⃣ USEFUL COMMANDS

### Package Management

```bash
dotnet add package PackageName
dotnet remove package PackageName
dotnet list package
```

### Database

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
dotnet ef database rollback
dotnet ef dbcontext scaffold
```

### Build & Run

```bash
dotnet restore
dotnet clean
dotnet build
dotnet run
dotnet publish
dotnet test
```

### Git

```bash
git clone <url>
git add .
git commit -m "message"
git push
git pull
git branch -a
```

---

## 🔟 RESOURCES

### Documentation

- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [EF Core Docs](https://docs.microsoft.com/ef/core)
- [JWT Best Practices](https://tools.ietf.org/html/rfc7519)

### Useful Tools

- **Postman** - API Testing
- **SQL Server Management Studio** - Database Management
- **LINQPad** - LINQ/SQL Testing
- **Fiddler** - HTTP Debugging
- **Application Insights** - Monitoring

### Community

- Stack Overflow: Tag `asp.net-core`
- Microsoft Docs
- GitHub Issues
- Reddit: r/dotnet

---

## 📞 SUPPORT

**Need Help?**

1. Check documentation at `PRESENTATION.md`
2. Review code comments
3. Check existing issues in GitHub
4. Ask team members
5. Search Stack Overflow

---

**Last Updated:** 2026-05-15
**Version:** 1.0
✅ **Ready to Use**
