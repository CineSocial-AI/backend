using AutoMapper;
using CineSocial.Core.Domain.Entities;
using CineSocial.Core.Application.DTOs.Groups;

namespace CineSocial.Core.Application.Mapping;

public class GroupMappingProfile : Profile
{
    public GroupMappingProfile()
    {
        CreateMap<Group, GroupDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.GetFullName()))
            .ForMember(dest => dest.CurrentUserRole, opt => opt.Ignore())
            .ForMember(dest => dest.IsCurrentUserMember, opt => opt.Ignore())
            .ForMember(dest => dest.IsCurrentUserBanned, opt => opt.Ignore());

        CreateMap<Group, GroupSummaryDto>();

        CreateMap<CreateGroupDto, Group>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Members, opt => opt.Ignore())
            .ForMember(dest => dest.Posts, opt => opt.Ignore())
            .ForMember(dest => dest.Bans, opt => opt.Ignore());

        CreateMap<UpdateGroupDto, Group>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.MemberCount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Members, opt => opt.Ignore())
            .ForMember(dest => dest.Posts, opt => opt.Ignore())
            .ForMember(dest => dest.Bans, opt => opt.Ignore());

        CreateMap<GroupMember, GroupMemberDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.GetFullName()))
            .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.ProfileImageUrl));
    }
}
