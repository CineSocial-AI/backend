using AutoMapper;
using CineSocial.Core.Domain.Entities;
using CineSocial.Core.Application.DTOs.Posts;

namespace CineSocial.Core.Application.Mapping;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.GetScore()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorProfileImage, opt => opt.MapFrom(src => src.Author.ProfileImageUrl))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.GroupIcon, opt => opt.MapFrom(src => src.Group.IconUrl))
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag)))
            .ForMember(dest => dest.CurrentUserReaction, opt => opt.Ignore());

        CreateMap<Post, PostSummaryDto>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.GetScore()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Media.FirstOrDefault() != null ? src.Media.FirstOrDefault()!.ThumbnailUrl : null))
            .ForMember(dest => dest.CurrentUserReaction, opt => opt.Ignore());

        CreateMap<CreatePostDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.UpvoteCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.DownvoteCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Group, opt => opt.Ignore())
            .ForMember(dest => dest.Media, opt => opt.Ignore())
            .ForMember(dest => dest.Reactions, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());

        CreateMap<UpdatePostDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Url, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.GroupId, opt => opt.Ignore())
            .ForMember(dest => dest.UpvoteCount, opt => opt.Ignore())
            .ForMember(dest => dest.DownvoteCount, opt => opt.Ignore())
            .ForMember(dest => dest.CommentCount, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsLocked, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Group, opt => opt.Ignore())
            .ForMember(dest => dest.Media, opt => opt.Ignore())
            .ForMember(dest => dest.Reactions, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());

        CreateMap<PostMedia, PostMediaDto>();

        CreateMap<PostComment, PostCommentDto>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.GetScore()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorProfileImage, opt => opt.MapFrom(src => src.Author.ProfileImageUrl))
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
            .ForMember(dest => dest.CurrentUserReaction, opt => opt.Ignore());

        CreateMap<CreatePostCommentDto, PostComment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.UpvoteCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.DownvoteCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
            .ForMember(dest => dest.Replies, opt => opt.Ignore())
            .ForMember(dest => dest.Reactions, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());

        CreateMap<UpdatePostCommentDto, PostComment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PostId, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.ParentCommentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.UpvoteCount, opt => opt.Ignore())
            .ForMember(dest => dest.DownvoteCount, opt => opt.Ignore())
            .ForMember(dest => dest.ReplyCount, opt => opt.Ignore())
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
            .ForMember(dest => dest.Replies, opt => opt.Ignore())
            .ForMember(dest => dest.Reactions, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());
    }
}
