using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Accounts.Managers
{
    public class ProfileDTO
    {
        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên phải từ 2 đến 100 ký tự")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Họ là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ phải từ 2 đến 100 ký tự")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Giới Tính")]
        public bool Gender { get; set; }

        [Display(Name = "Ngày Sinh")]
        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date)]
        public DateOnly Dob { get; set; }

        [Display(Name = "Tỉnh/Thành Phố")]
        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        public string City { get; set; } = null!;

        [Display(Name = "Quốc Gia")]
        [Required(ErrorMessage = "Quốc gia là bắt buộc")]
        public string Country { get; set; } = null!;

        [Display(Name = "Địa Chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Địa chỉ phải từ 5 đến 500 ký tự")]
        public string Address { get; set; } = null!;

        [Display(Name = "Số Điện Thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\+?[\d\s\-\(\)]{10,}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Display(Name = "Số CMND/CCCD")]
        [Required(ErrorMessage = "Số CMND/CCCD là bắt buộc")]
        public string CitizenIdentityCard { get; set; } = null!;
    }
}
