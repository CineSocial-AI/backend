
MIGRATION INSTRUCTIONS:
=======================

1. Update ApplicationDbContext.cs:
   - Add the new DbSet properties listed in database_context_updates.txt
   - Add the entity configurations to OnModelCreating method

2. Update User.cs entity:
   - Replace the existing User entity with the updated version created by this script

3. Run EF Core migration:
   dotnet ef migrations add AddRedditPlatformEntities --project CineSocial.Adapters.Infrastructure --startup-project CineSocial.Adapters.WebAPI
   dotnet ef database update --project CineSocial.Adapters.Infrastructure --startup-project CineSocial.Adapters.WebAPI

4. Update Program.cs:
   - Add the service registrations listed in program_cs_updates.txt

5. Test the endpoints:
   - GET /api/groups - List groups
   - POST /api/groups - Create group
   - GET /api/groups/{id} - Get group details
   - POST /api/groups/{id}/join - Join group
   - GET /api/posts - List posts
   - POST /api/posts - Create post
   - GET /api/posts/{id} - Get post details
   - GET /api/posts/{id}/comments - Get post comments

FILES CREATED:
==============
- Domain Entities: Group, GroupMember, Post, PostMedia, PostComment, PostReaction, CommentReaction, GroupBan, UserBlock, Report, PostTag, Following
- DTOs: GroupDtos.cs, PostDtos.cs
- Service Interfaces: IGroupService.cs, IPostService.cs
- Service Implementations: GroupService.cs, PostService.cs
- Mapping Profiles: GroupMappingProfile.cs, PostMappingProfile.cs
- Controllers: GroupsController.cs, PostsController.cs
- Updated User.cs entity

NEXT STEPS:
===========
1. Apply the database context changes
2. Run migrations
3. Update Program.cs with service registrations
4. Test the API endpoints
5. Add authentication/authorization where needed
6. Implement file upload for post media
7. Add notification system
8. Add search functionality
9. Add moderation tools

