using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, AccountDetail>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString()))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Product, ProductDetail>()
                .ForMember(dest => dest.CommodityId, opt => opt.MapFrom(src => src.Commodity.Id))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Area.Id))
                .ForMember(dest => dest.SellerId, opt => opt.MapFrom(src => src.Seller.Id))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
