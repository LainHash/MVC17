using AutoMapper;
using MVC17.Models;
using MVC17.DTOs.Products.Create;
using MVC17.DTOs.Products.Update;
using MVC17.ViewModels;

namespace MVC17.Mappings
{
    public class ProductMP : Profile
    {
        public ProductMP() {
            CreateMap<VwProduct, ProductVM>().ReverseMap();

            CreateMap<CreateProductDTO, Product>();
            CreateMap<CreateLaptopDTO, Laptop>();
            CreateMap<CreateCpuDTO, Cpu>();
            CreateMap<CreateGpuDTO, Gpu>();
            CreateMap<CreateRamDTO, Ram>();
            CreateMap<CreateStorageDTO, Storage>();

            CreateMap<UpdateProductDTO, Product>().ReverseMap();
            CreateMap<UpdateLaptopDTO, Laptop>()
                .ForMember(dest => dest.ProductSku, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<UpdateCpuDTO, Cpu>()
                .ForMember(dest => dest.ProductSku, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<UpdateGpuDTO, Gpu>()
                .ForMember(dest => dest.ProductSku, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<UpdateRamDTO, Ram>()
                .ForMember(dest => dest.ProductSku, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<UpdateStorageDTO, Storage>()
                .ForMember(dest => dest.ProductSku, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
