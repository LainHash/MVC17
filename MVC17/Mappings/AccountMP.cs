using AutoMapper;
using MVC17.DTOs.Accounts.Create;
using MVC17.DTOs.Accounts.Update;
using MVC17.Models;

namespace MVC17.Mappings
{
    public class AccountMP : Profile
    {
        public AccountMP()
        {
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
            CreateMap<CreateUserProfileDTO, PersonalInformation>();

            CreateMap<VwCustomerProfile, UpdateUserProfileDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.PersonalEmail));

            CreateMap<UpdateUserProfileDTO, PersonalInformation>();
        }
    }
}
