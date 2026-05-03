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

namespace MVC17.Controllers
{
    public class AccountController : Controller
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;

        public AccountController(Dbmvc05Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
            if (user != null)
            {
                HttpContext.Session.SetInt32(SessionConstants.userId, user.UserId);
                HttpContext.Session.SetString(SessionConstants.email, user.Email);
                HttpContext.Session.SetString(SessionConstants.username, user.Username);
                await MergeCartAsync(user.UserId);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Sai email hoặc mật khẩu";
            return View();
        }

        public ActionResult Logout()
        {
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
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            var query = _context.Customers.Include(c => c.User);

            var isExistedEmail = await query.AnyAsync(u => u.User.Email == dto.Email);
            var count = await query.CountAsync() + 1;

            if (isExistedEmail)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View();
            }

            var customer = new Customer()
            {
                CustomerCode = "CST" + count.ToString("X6"),
                User = _mapper.Map<User>(dto),
                Pi = _mapper.Map<PersonalInformation>(dto.Profile)
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");

        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
            if (userId == null)
            {
                return NotFound();
            }
            var customer = await _context.VwCustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
            if (userId == null)
            {
                return NotFound();
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
        public async Task<IActionResult> Edit(UpdateAccountDTO dto)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
            if (userId == null)
            {
                return NotFound();
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

        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            return View(new ChangePasswordDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.PasswordHash != dto.OldPassword)
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

            user.PasswordHash = dto.NewPassword;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công";
            return RedirectToAction("Profile");
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
                    .FirstOrDefault(x => x.ProductSkuId == guestItem.ProductSkuId);

                if (existingItem == null)
                {
                    userCart.CartItems.Add(new CartItem
                    {
                        ProductSkuId = guestItem.ProductSkuId,
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
    }
}
