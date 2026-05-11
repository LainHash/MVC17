using AutoMapper;
using MVC17.Models;
using MVC17.ViewModels;

namespace MVC17.Mappings
{
    public class OrderMP : Profile
    {
        public OrderMP() {
            CreateMap<Invoice, OrderHistoryVM>();
        }
    }
}
