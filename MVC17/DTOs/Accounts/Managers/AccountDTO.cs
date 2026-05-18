using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Accounts.Managers
{
    public class AccountDTO
    {
        [Display(Name = "Tên Đăng Nhập")]
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 100 ký tự")]
        public string Username { get; set; } = null!;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; } = null!;
    }
}
