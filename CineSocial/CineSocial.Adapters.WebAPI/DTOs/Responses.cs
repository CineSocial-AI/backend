namespace CineSocial.Adapters.WebAPI.DTOs.Responses;

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Constructor - boş constructor
    public ApiResponse() { }

    // Parametreli constructor
    public ApiResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public static ApiResponse CreateSuccess(string message = "İşlem başarılı")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static ApiResponse CreateFailure(string error)
    {
        return new ApiResponse
        {
            Success = false,
            Message = error,
            Errors = new List<string> { error }
        };
    }

    public static ApiResponse CreateFailure(List<string> errors)
    {
        return new ApiResponse
        {
            Success = false,
            Message = string.Join(", ", errors),
            Errors = errors
        };
    }
}

/// <summary>
/// Generic API response with data
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    // Constructor - boş constructor
    public ApiResponse() : base() { }

    // Parametreli constructor
    public ApiResponse(bool success, string message, T? data = default) : base(success, message)
    {
        Data = data;
    }

    public static ApiResponse<T> CreateSuccess(T data, string message = "İşlem başarılı")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static new ApiResponse<T> CreateFailure(string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = error,
            Errors = new List<string> { error }
        };
    }

    public static new ApiResponse<T> CreateFailure(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = string.Join(", ", errors),
            Errors = errors
        };
    }
}

/// <summary>
/// Authentication token response
/// </summary>
public class AuthTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = null!;
}

/// <summary>
/// User information response
/// </summary>
public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Bio { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paginated response wrapper
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}