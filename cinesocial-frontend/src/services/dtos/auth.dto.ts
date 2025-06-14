// Based on backend AuthController and DTOs from the provided documentation

export interface RegisterRequestDto {
  email: string;
  password_DEPRECATED?: string; // Will be replaced by `password`
  confirmPassword_DEPRECATED?: string; // Will be replaced by `confirmPassword`
  password?: string; // Correct field for password
  confirmPassword?: string; // Correct field for confirm password
  firstName: string;
  lastName: string;
  userName: string;
}

export interface LoginRequestDto {
  email: string;
  password_DEPRECATED?: string; // Will be replaced by `password`
  password?: string; // Correct field for password
  rememberMe?: boolean; // As per AuthController.cs Login method
}

export interface UserResponseDto {
  id: string;
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  fullName?: string;
  profileImageUrl?: string;
  emailConfirmed: boolean;
}

export interface AuthTokenResponseDto {
  accessToken: string;
  refreshToken: string;
  expiresAt: string; // Or Date
  user: UserResponseDto;
}

// General API Response Wrapper (consistent with backend's ApiResponse<T>)
// This is what the backend returns.
// The `value` field in the DTO below maps to `Data` in the C# ApiResponse<T>.
// The `errorMessage` field maps to `Message` or `Errors[0]` when IsSuccess is false.
export interface BackendApiResponse<T> {
  isSuccess: boolean;
  message?: string; // General message, often for success
  data?: T; // The actual payload, maps to 'Data' in C#
  errors?: string[]; // Array of error messages, maps to 'Errors' in C#
  errorMessage?: string; // Often used as a single error message from `CreateFailure`
}

// This will be the structure the service layer functions return,
// aiming to simplify it slightly for frontend consumption.
// We'll map the backend's `data` to `value` and `errors[0]` or `message` (on failure) to `errorMessage`.
export interface FrontendApiResponse<T> {
  isSuccess: boolean;
  value?: T; // The actual data payload
  message?: string; // Success message from backend
  errorMessage?: string; // Unified error message
  errors?: string[]; // For multiple validation errors
}
// Note: The service functions will transform BackendApiResponse to FrontendApiResponse.
// For the current task, I will use the BackendApiResponse directly in services
// as specified by `Promise<ApiResponse<AuthTokenResponseDto>>` in the plan,
// where `ApiResponse` here means the backend structure.
// The DTO field `errorMessage?: string;` in `RegisterRequestDto` and `LoginRequestDto`
// was a misinterpretation of the plan; it should be in the response, not request.
// I am ensuring `RegisterRequestDto` and `LoginRequestDto` only contain request fields.
// The `ApiResponse` in the plan refers to the backend's `ApiResponse<T>`.
// I will use `BackendApiResponse` as the type name for the service functions for clarity.
// The DTO `ApiResponse<T>` in the prompt was slightly ambiguous. I will use the structure from the backend more directly.

// Corrected DTOs for request:
export interface RegisterRequest {
  email: string;
  password?: string;
  confirmPassword?: string;
  firstName: string;
  lastName:string;
  userName: string;
}

export interface LoginRequest {
  email: string;
  password?: string;
  rememberMe?: boolean;
}

// The `AuthTokenResponseDto` and `UserResponseDto` are fine.
// `BackendApiResponse<T>` will be used by services.
// I'll stick to `ApiResponse<T>` as the generic name in services as per prompt,
// but it will map to `BackendApiResponse<T>`.

// Final structure for DTOs to be used in authService.ts
export interface RegisterUserDto {
  email: string;
  password?: string;
  confirmPassword?: string;
  firstName: string;
  lastName: string;
  userName: string;
}

export interface LoginUserDto {
  email: string;
  password?: string;
  rememberMe?: boolean;
}
// UserResponseDto and AuthTokenResponseDto are as above.
// The generic ApiResponse for service return type:
export interface GenericApiResponse<T> {
  isSuccess: boolean;
  message?: string;       // Message from backend (often success message)
  data?: T;               // Payload from backend (maps to Data property)
  errors?: string[];      // Error list from backend
  errorMessage?: string;  // Potentially a primary error message (if backend sends it in Message on failure)
}
// Sticking to the DTOs as defined in the initial problem description's AuthController.cs mapping for requests:
// RegisterRequest (Email, Password, ConfirmPassword, FirstName, LastName, UserName)
// LoginRequest (Email, Password, RememberMe)
// AuthTokenResponse (AccessToken, RefreshToken, ExpiresAt, User) where User is UserResponse (Id, Email, UserName, FirstName, LastName, FullName, ProfileImageUrl, EmailConfirmed).
// Common response ApiResponse.CreateSuccess or ApiResponse.CreateFailure.
// Let's redefine based on this for clarity and correctness.

export interface RegisterRequestData {
  email: string;
  password?: string; // Made optional to match DTO style, but usually required
  confirmPassword?: string; // Made optional, but usually required
  firstName: string;
  lastName: string;
  userName: string;
}

export interface LoginRequestData {
  email: string;
  password?: string; // Made optional, but usually required
  rememberMe?: boolean;
}

// UserResponse and AuthTokenResponse are fine as UserResponseDto and AuthTokenResponseDto.

// This is the generic wrapper that axiosInstance.post<HERE> will expect.
// This matches the C# ApiResponse<T> structure.
export interface ServiceApiResponse<T> {
  isSuccess: boolean;
  message?: string; // This is the 'Message' property in C# ApiResponse
  data?: T;         // This is the 'Data' property in C# ApiResponse
  errors?: string[];// This is the 'Errors' property in C# ApiResponse
}

// The prompt's example uses `ApiResponse<T>` as the return type for service functions.
// I will define this to match the `ServiceApiResponse<T>` which reflects the backend.
export type { ServiceApiResponse as ApiResponse };
