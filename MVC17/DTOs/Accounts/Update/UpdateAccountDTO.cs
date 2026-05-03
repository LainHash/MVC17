using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Accounts.Update
{
    public class UpdateAccountDTO : AccountBaseDTO
    {
        [Display(Name = "Số Dư")]
        [Range(0, int.MaxValue, ErrorMessage = "Số dư phải là số dương")]
        public int Balance { get; set; }

        [Display(Name = "Đổi Mật Khẩu")]
        public ChangePasswordDTO? ChangePassword { get; set; }

        [Display(Name = "Hồ Sơ Người Dùng")]
        public UpdateUserProfileDTO? Profile { get; set; }
    }
}
