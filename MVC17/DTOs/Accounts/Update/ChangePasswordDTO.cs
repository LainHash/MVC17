using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Accounts.Update
{
    public class ChangePasswordDTO
    {
        [Display(Name = "Mật Khẩu Cũ")]
        [Required(ErrorMessage = "Mật khẩu cũ là bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 255 ký tự")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;

        [Display(Name = "Mật Khẩu Mới")]
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 255 ký tự")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Display(Name = "Xác Nhận Mật Khẩu Mới")]
        [Required(ErrorMessage = "Xác nhận mật khẩu mới là bắt buộc")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
