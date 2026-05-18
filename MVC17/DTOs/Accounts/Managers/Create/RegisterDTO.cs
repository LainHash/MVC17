using MVC17.DTOs.Accounts.Managers;
using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Accounts.Managers.Create
{
    public class RegisterDTO : AccountDTO
    {
        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 255 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Xác Nhận Mật Khẩu")]
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Ảnh Đại Diện (URL)")]
        public string? AvatarImage { get; set; }

        [Display(Name = "Hồ Sơ Người Dùng")]
        public CreateProfileDTO? Profile { get; set; }
    }
}
