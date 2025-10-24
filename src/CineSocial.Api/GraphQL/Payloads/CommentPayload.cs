using CineSocial.Domain.Entities.Social;

namespace CineSocial.Api.GraphQL.Payloads;

public class CommentPayload
{
    public Comment? Comment { get; set; }
    public UserError? Error { get; set; }
}
