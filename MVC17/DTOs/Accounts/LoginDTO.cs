using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Accounts
{
    public class LoginDTO
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 255 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
