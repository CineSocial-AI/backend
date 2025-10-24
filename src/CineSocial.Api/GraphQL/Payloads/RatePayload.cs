using CineSocial.Domain.Entities.Social;

namespace CineSocial.Api.GraphQL.Payloads;

public class RatePayload
{
    public Rate? Rate { get; set; }
    public UserError? Error { get; set; }
}
