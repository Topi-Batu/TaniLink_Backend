using AutoMapper;
using Google.Protobuf.Collections;
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
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
            CreateMap<EditProfileReq, User>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
            CreateMap<Product, ProductDetail>()
                .ForMember(dest => dest.CommodityId, opt => opt.MapFrom(src => src.Commodity.Id))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Area.Id))
                .ForMember(dest => dest.SellerId, opt => opt.MapFrom(src => src.Seller.Id))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Images.Select(i => i.Image).ToList()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => Timestamp.FromDateTimeOffset(src.CreatedAt)))
                .ForMember(dest => dest.UpdatedAt, opt => {
                    opt.PreCondition(src => src.UpdatedAt != null);
                    opt.MapFrom(src => Timestamp.FromDateTimeOffset(src.UpdatedAt.Value)); 
                })
                .ForMember(dest => dest.DeletedAt, opt => {
                    opt.PreCondition(src => src.DeletedAt != null);
                    opt.MapFrom(src => Timestamp.FromDateTimeOffset(src.DeletedAt.Value));
                })
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember.ToString())));
        }
    }
}
