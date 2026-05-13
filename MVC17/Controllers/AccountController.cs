using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.DTOs.Accounts.Create;
using MVC17.DTOs.Accounts.Update;
using MVC17.Helpers.Constants.Auths.Accounts;
using MVC17.Helpers.Constants.Sessions;
using MVC17.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using MVC17.ViewModels;
using MVC17.Services.Interfaces;

namespace MVC17.Controllers
{
    public class AccountController : Controller
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AccountController(Dbmvc05Context context, IMapper mapper, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
            _emailService = emailService;
        }

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Index()
        {
            var accounts = await _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.UserId)
                .ToListAsync();
            return View(accounts);
        }

        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                if (!user.IsActive)
                {
                    TempData["Error"] = "Tài khoản chưa được xác nhận email. Vui lòng kiểm tra hộp thư của bạn.";
                    return View();
                }

                var tokenString = GenerateJwtToken(user);
                Response.Cookies.Append("jwt", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddMinutes(120)
                });


                await MergeCartAsync(user.UserId);
                return RedirectToAction("Index", "Home");
            }
            ViewData["Error"] = "Sai email hoặc mật khẩu.";
            return View();
        }

        public ActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Register()
        {
            ViewBag.Cities = new SelectList(UserProfileConstants.Cities);
            ViewBag.Countries = new SelectList(UserProfileConstants.Countries);

            var dto = new RegisterDTO();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = new SelectList(UserProfileConstants.Cities);
                ViewBag.Countries = new SelectList(UserProfileConstants.Countries);
                return View(dto);
            }

            var query = _context.Customers.Include(c => c.User);

            var isExistedEmail = await query.AnyAsync(u => u.User.Email == dto.Email);
            var count = await query.CountAsync() + 1;

            if (isExistedEmail)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                ViewBag.Cities = new SelectList(UserProfileConstants.Cities);
                ViewBag.Countries = new SelectList(UserProfileConstants.Countries);
                return View(dto);
            }

            var customer = new Customer()
            {
                CustomerCode = "CST" + count.ToString("X6"),
                User = _mapper.Map<User>(dto),
                Pi = _mapper.Map<PersonalInformation>(dto.Profile)
            };

            customer.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            customer.User.IsActive = false;
            customer.User.UserUuid = Guid.NewGuid();

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = customer.User.UserId, token = customer.User.UserUuid }, Request.Scheme);
            await _emailService.SendEmailAsync(customer.User.Email, "Xác nhận đăng ký tài khoản",
                $"Vui lòng click vào link sau để xác nhận đăng ký: <a href='{confirmationLink}'>Xác nhận</a>");

            TempData["Success"] = "Đăng ký thành công. Vui lòng kiểm tra email để xác nhận.";

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, Guid token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.UserUuid == token);
            if (user == null)
            {
                TempData["Error"] = "Link xác nhận không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Login");

            }

            user.IsActive = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xác nhận email thành công. Bạn có thể đăng nhập.";
            return RedirectToAction("Login");
        }

        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Profile()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }


            var customer = await _context.VwCustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Edit()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.VwCustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return NotFound();
            }

            var dto = new UpdateAccountDTO()
            {
                Email = customer.AccountEmail,
                Username = customer.Username,
                Profile = _mapper.Map<UpdateUserProfileDTO>(customer)
            };

            ViewBag.Cities = new SelectList(UserProfileConstants.Cities);
            ViewBag.Countries = new SelectList(UserProfileConstants.Countries);
            ViewBag.Genders = new SelectList(UserProfileConstants.Genders, "Key", "Value");
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Edit(UpdateAccountDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = new SelectList(UserProfileConstants.Cities);
                ViewBag.Countries = new SelectList(UserProfileConstants.Countries);
                ViewBag.Genders = new SelectList(UserProfileConstants.Genders, "Key", "Value");
                return View(dto);
            }

            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return NotFound();
            }

            customer.User.Email = dto.Email;
            customer.User.Username = dto.Username;
            _mapper.Map(dto.Profile, customer.Pi);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile");
        }

        [Authorize(Policy = "Customer")]
        public IActionResult ChangePassword()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ChangePasswordDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(dto.OldPassword), "Mật khẩu cũ không đúng");
                return View(dto);
            }

            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                ModelState.AddModelError(nameof(dto.ConfirmNewPassword), "Xác nhận mật khẩu không khớp");
                return View(dto);
            }

            if (dto.OldPassword == dto.NewPassword)
            {
                ModelState.AddModelError(nameof(dto.NewPassword), "Mật khẩu mới không được trùng mật khẩu cũ");
                return View(dto);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            Response.Cookies.Delete("jwt");
            HttpContext.Session.Clear();
            TempData["Success"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";

            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> OrderHistory()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await GetCustomerAsync(userId);
            if (customer == null)
            {
                return BadRequest("Không tìm thấy thông tin khách hàng.");
            }

            var invoice = await GetInvoiceAsync(customer.CustomerId);
            var orderHistory = _mapper.Map<List<OrderHistoryVM>>(invoice);

            return View(orderHistory);
        }

        private async Task MergeCartAsync(int userId)
        {
            var sessionId = HttpContext.Session.GetString(SessionConstants.sessionId);

            if (string.IsNullOrWhiteSpace(sessionId))
                return;

            var guestCart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.UserId == null);

            if (guestCart == null)
                return;

            var userCart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart == null)
            {
                guestCart.UserId = userId;
                guestCart.SessionId = null;

                await _context.SaveChangesAsync();
                return;
            }

            foreach (var guestItem in guestCart.CartItems)
            {
                var existingItem = userCart.CartItems
                    .FirstOrDefault(x => x.ProductId == guestItem.ProductId);

                if (existingItem == null)
                {
                    userCart.CartItems.Add(new CartItem
                    {
                        ProductId = guestItem.ProductId,
                        Quantity = guestItem.Quantity,
                        UnitPrice = guestItem.UnitPrice,
                        AddedDate = DateTime.Now
                    });
                }
                else
                {
                    existingItem.Quantity = existingItem.Quantity + guestItem.Quantity;
                }
            }

            _context.CartItems.RemoveRange(guestCart.CartItems);
            _context.ShoppingCarts.Remove(guestCart);

            await _context.SaveChangesAsync();
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out userId);
        }

        private Task<Customer?> GetCustomerAsync(int userId)
        {
            return _context.Customers
                .Include(c => c.User)
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);
        }

        private Task<List<Invoice>> GetInvoiceAsync(int customerId)
        {
            return _context.Invoices
                .Where(iv => iv.CustomerId == customerId)
                .ToListAsync();
        }
    }
}
