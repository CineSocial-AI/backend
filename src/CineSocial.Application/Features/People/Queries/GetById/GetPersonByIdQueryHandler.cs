using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.People.Queries.GetById;

public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, Result<Person>>
{
    private readonly IRepository<Person> _personRepository;

    public GetPersonByIdQueryHandler(IRepository<Person> personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<Person>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _personRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (person == null)
                return Result<Person>.Failure("Person not found");

            return Result<Person>.Success(person);
        }
        catch (Exception ex)
        {
            return Result<Person>.Failure($"Failed to retrieve person: {ex.Message}");
        }
    }
}
