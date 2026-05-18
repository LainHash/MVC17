using AutoMapper;
using MVC17.Models;

namespace MVC17.Mappings
{
    public class AccountMP : Profile
    {
        public AccountMP()
        {
            // Customer maps
            CreateMap<DTOs.Accounts.Customers.Create.RegisterDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
            CreateMap<DTOs.Accounts.Customers.Create.CreateProfileDTO, PersonalInformation>();

            CreateMap<VwCustomerProfile, DTOs.Accounts.Customers.Update.UpdateProfileDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.PersonalEmail));

            CreateMap<DTOs.Accounts.Customers.Update.UpdateProfileDTO, PersonalInformation>();

            // Manager maps
            CreateMap<DTOs.Accounts.Managers.Create.RegisterDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
            CreateMap<DTOs.Accounts.Managers.Create.CreateProfileDTO, PersonalInformation>();
        }
    }
}
