using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MVC17.Data;
using MVC17.DTOs.Accounts.Managers;
using MVC17.DTOs.Accounts.Managers.Create;
using MVC17.DTOs.Accounts.Managers.Update;
using MVC17.Helpers.Constants.Auths.Accounts;
using MVC17.Models;
using MVC17.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVC17.Controllers
{
    public class ManagerController : Controller
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public ManagerController(Dbmvc05Context context, IMapper mapper, IConfiguration config, IEmailService emailService)
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

        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
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
                return RedirectToAction("Index", "Home");
            }
            ViewData["Error"] = "Sai email hoặc mật khẩu.";
            return View();
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
                Pi = _mapper.Map<PersonalInformation>(dto.Profile),
                AvatarImage = dto.AvatarImage
            };

            customer.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            customer.User.IsActive = false;
            customer.User.UserUuid = Guid.NewGuid();

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var confirmationLink = Url.Action("ConfirmEmail", "Manager", new { userId = customer.User.UserId, token = customer.User.UserUuid }, Request.Scheme);
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

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Profile(int? userId)
        {
            int targetUserId;
            if (userId.HasValue)
            {
                targetUserId = userId.Value;
            }
            else
            {
                if (!TryGetCurrentUserId(out targetUserId))
                {
                    return RedirectToAction("Login", "Manager");
                }
            }

            var employee = await _context.VwEmployeeProfiles
                .FirstOrDefaultAsync(e => e.UserId == targetUserId);
            if (employee == null)
            {
                return NotFound("Không tìm thấy thông tin nhân viên.");
            }

            return View(employee);
        }

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> ProfileCustomer(int userId)
        {
            var customer = await _context.VwCustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return NotFound("Không tìm thấy thông tin khách hàng.");
            }

            var customerEntity = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);
            ViewBag.AvatarImage = customerEntity?.AvatarImage;

            return View(customer);
        }

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Edit()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Manager");
            }

            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Pi)
                .FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null)
            {
                return NotFound("Không tìm thấy thông tin nhân viên.");
            }

            var dto = new ManagerProfileEditDTO()
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                Username = employee.User.Username,
                Email = employee.User.Email,
                FirstName = employee.Pi?.FirstName ?? "",
                LastName = employee.Pi?.LastName ?? "",
                Gender = employee.Pi?.Gender ?? false,
                Dob = employee.Pi?.Dob ?? DateOnly.FromDateTime(DateTime.Now),
                City = employee.Pi?.City ?? "",
                Country = employee.Pi?.Country ?? "",
                Address = employee.Pi?.Address ?? "",
                Phone = employee.Pi?.Phone ?? "",
                CitizenIdentityCard = employee.Pi?.CitizenIdentityCard ?? "",
                AvatarImage = employee.AvatarImage
            };

            ViewBag.Cities = new SelectList(UserProfileConstants.Cities);
            ViewBag.Countries = new SelectList(UserProfileConstants.Countries);
            ViewBag.Genders = new SelectList(UserProfileConstants.Genders, "Key", "Value");
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Edit(ManagerProfileEditDTO dto)
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
                return RedirectToAction("Login", "Manager");
            }

            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Pi)
                .FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null)
            {
                return NotFound("Không tìm thấy thông tin nhân viên.");
            }

            employee.User.Username = dto.Username;
            employee.User.Email = dto.Email;

            employee.AvatarImage = dto.AvatarImage;

            if (employee.Pi != null)
            {
                employee.Pi.FirstName = dto.FirstName;
                employee.Pi.LastName = dto.LastName;
                employee.Pi.Gender = dto.Gender;
                employee.Pi.Dob = dto.Dob;
                employee.Pi.City = dto.City;
                employee.Pi.Country = dto.Country;
                employee.Pi.Address = dto.Address;
                employee.Pi.Phone = dto.Phone;
                employee.Pi.CitizenIdentityCard = dto.CitizenIdentityCard;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin thành công.";
            return RedirectToAction("Profile");
        }

        [Authorize(Policy = "Manager")]
        public IActionResult ChangePassword()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Manager");
            }

            return View(new ChangePasswordDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Manager");
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


        private bool TryGetCurrentUserId(out int userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out userId);
        }

    }
}
