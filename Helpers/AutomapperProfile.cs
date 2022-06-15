using API.DTO;
using API.Types;
using AutoMapper;

namespace API.Helpers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<UserForRegisterDto, User>();
            CreateMap<User, UserForDetailDto>().ForMember(dest => dest.UserId, o =>
            {
                o.MapFrom(src => src.Id);
            })
            .ForMember(dest => dest.HasAvatar, o =>
            {
                o.MapFrom(src => !String.IsNullOrWhiteSpace(src.AvatarUrl));
            });
            CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.UserId, o =>
            {
                o.MapFrom(src => src.Id);
            })
            .ForMember(dest => dest.HasAvatar, o =>
            {
                o.MapFrom(src => !String.IsNullOrWhiteSpace(src.AvatarUrl));
            });
            CreateMap<BlogPost, PostForListDto>()
                .ForMember(dest => dest.CreatedBy, o =>
                {
                    o.MapFrom(src => src.CreatedBy.FullName);
                })
                .ForMember(dest => dest.CreatedById, o =>
                {
                    o.MapFrom(src => src.UserId);
                })
                .ForMember(dest => dest.Content, o =>
                {
                    o.MapFrom(src => src.ShortContent);
                })
                .ForMember(dest => dest.WasUpdated, o =>
                {
                    o.MapFrom(src => src.UpdateDate.HasValue);
                });
            CreateMap<BlogPost, PostForDetailDto>()
                .ForMember(dest => dest.CreatedBy, o =>
                {
                    o.MapFrom(src => src.CreatedBy.FullName);
                })
                .ForMember(dest => dest.CreatedById, o =>
                {
                    o.MapFrom(src => src.UserId);
                })
                .ForMember(dest => dest.HasImage, o =>
                {
                    o.MapFrom(src => !String.IsNullOrWhiteSpace(src.ImageUrl));
                });
        }
    }
}