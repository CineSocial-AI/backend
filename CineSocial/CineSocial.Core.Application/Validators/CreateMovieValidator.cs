using FluentValidation;
using CineSocial.Core.Application.DTOs.Movies;

namespace CineSocial.Core.Application.Validators;

/// <summary>
/// Validator for CreateMovieDto
/// </summary>
public class CreateMovieValidator : AbstractValidator<CreateMovieDto>
{
    public CreateMovieValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Film başlığı gereklidir")
            .MaximumLength(200)
            .WithMessage("Film başlığı 200 karakterden uzun olamaz");

        RuleFor(x => x.OriginalTitle)
            .MaximumLength(200)
            .WithMessage("Orijinal başlık 200 karakterden uzun olamaz")
            .When(x => !string.IsNullOrEmpty(x.OriginalTitle));

        RuleFor(x => x.Overview)
            .MaximumLength(2000)
            .WithMessage("Film açıklaması 2000 karakterden uzun olamaz")
            .When(x => !string.IsNullOrEmpty(x.Overview));

        RuleFor(x => x.Runtime)
            .GreaterThan(0)
            .WithMessage("Film süresi 0'dan büyük olmalıdır")
            .LessThan(1000)
            .WithMessage("Film süresi 1000 dakikadan az olmalıdır")
            .When(x => x.Runtime.HasValue);

        RuleFor(x => x.VoteAverage)
            .InclusiveBetween(0, 10)
            .WithMessage("Oy ortalaması 0-10 arasında olmalıdır")
            .When(x => x.VoteAverage.HasValue);

        RuleFor(x => x.VoteCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Oy sayısı 0'dan küçük olamaz")
            .When(x => x.VoteCount.HasValue);

        RuleFor(x => x.GenreIds)
            .NotEmpty()
            .WithMessage("En az bir tür seçilmelidir");

        RuleFor(x => x.TmdbId)
            .GreaterThan(0)
            .WithMessage("TMDB ID 0'dan büyük olmalıdır")
            .When(x => x.TmdbId.HasValue);
    }
}