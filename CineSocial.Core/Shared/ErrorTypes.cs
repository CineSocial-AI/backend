namespace CineSocial.Core.Shared;

public static class ErrorTypes
{
    public static class Validation
    {
        public const string Required = "VALIDATION_REQUIRED";
        public const string InvalidFormat = "VALIDATION_INVALID_FORMAT";
        public const string MinLength = "VALIDATION_MIN_LENGTH";
        public const string MaxLength = "VALIDATION_MAX_LENGTH";
        public const string Range = "VALIDATION_RANGE";
        public const string Email = "VALIDATION_EMAIL";
        public const string UniqueConstraint = "VALIDATION_UNIQUE_CONSTRAINT";
    }

    public static class Authentication
    {
        public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
        public const string UserNotFound = "AUTH_USER_NOT_FOUND";
        public const string TokenExpired = "AUTH_TOKEN_EXPIRED";
        public const string TokenInvalid = "AUTH_TOKEN_INVALID";
        public const string Unauthorized = "AUTH_UNAUTHORIZED";
        public const string EmailNotVerified = "AUTH_EMAIL_NOT_VERIFIED";
        public const string AccountLocked = "AUTH_ACCOUNT_LOCKED";
    }

    public static class Authorization
    {
        public const string InsufficientPermissions = "AUTHZ_INSUFFICIENT_PERMISSIONS";
        public const string ResourceNotOwned = "AUTHZ_RESOURCE_NOT_OWNED";
        public const string AccessDenied = "AUTHZ_ACCESS_DENIED";
    }

    public static class NotFound
    {
        public const string User = "NOT_FOUND_USER";
        public const string Movie = "NOT_FOUND_MOVIE";
        public const string Review = "NOT_FOUND_REVIEW";
        public const string Comment = "NOT_FOUND_COMMENT";
        public const string MovieList = "NOT_FOUND_MOVIE_LIST";
        public const string Rating = "NOT_FOUND_RATING";
        public const string Person = "NOT_FOUND_PERSON";
        public const string Genre = "NOT_FOUND_GENRE";
    }

    public static class Conflict
    {
        public const string UserAlreadyExists = "CONFLICT_USER_ALREADY_EXISTS";
        public const string EmailAlreadyExists = "CONFLICT_EMAIL_ALREADY_EXISTS";
        public const string UsernameAlreadyExists = "CONFLICT_USERNAME_ALREADY_EXISTS";
        public const string RatingAlreadyExists = "CONFLICT_RATING_ALREADY_EXISTS";
        public const string ReviewAlreadyExists = "CONFLICT_REVIEW_ALREADY_EXISTS";
        public const string FavoriteAlreadyExists = "CONFLICT_FAVORITE_ALREADY_EXISTS";
    }

    public static class Business
    {
        public const string InvalidOperation = "BUSINESS_INVALID_OPERATION";
        public const string RatingOutOfRange = "BUSINESS_RATING_OUT_OF_RANGE";
        public const string SelfReaction = "BUSINESS_SELF_REACTION";
        public const string DuplicateReaction = "BUSINESS_DUPLICATE_REACTION";
    }

    public static class System
    {
        public const string DatabaseError = "SYSTEM_DATABASE_ERROR";
        public const string ExternalServiceError = "SYSTEM_EXTERNAL_SERVICE_ERROR";
        public const string InternalError = "SYSTEM_INTERNAL_ERROR";
        public const string ServiceUnavailable = "SYSTEM_SERVICE_UNAVAILABLE";
    }
}