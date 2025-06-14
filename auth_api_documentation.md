## Authentication API Documentation

This document outlines the API endpoints for user authentication and authorization. JWT (JSON Web Tokens) are used for authorization. The frontend is responsible for securely storing these tokens and sending them with authenticated requests.

---

### 1. User Registration

*   **Endpoint:** `POST /api/Auth/register`
*   **Description:** Registers a new user in the system.
*   **Authorization:** `[AllowAnonymous]` (No authentication required)
*   **Request Body:** `RegisterRequest`
    ```json
    {
      "Email": "user@example.com",
      "Password": "Password123!",
      "ConfirmPassword": "Password123!",
      "FirstName": "John",
      "LastName": "Doe",
      "UserName": "johndoe"
    }
    ```
*   **Success Response (200 OK):** `ApiResponse<AuthTokenResponse>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Kayıt başarılı",
      "Data": {
        "AccessToken": "your_access_token",
        "RefreshToken": "your_refresh_token",
        "ExpiresAt": "2024-01-01T12:00:00Z",
        "User": {
          "Id": "user_guid",
          "Email": "user@example.com",
          "UserName": "johndoe",
          "FirstName": "John",
          "LastName": "Doe",
          "FullName": "John Doe",
          "ProfileImageUrl": null,
          "EmailConfirmed": false
        }
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse`
    ```json
    {
      "IsSuccess": false,
      "Message": "Error message from server (e.g., Passwords do not match, Email already exists)",
      "Data": null,
      "Errors": ["Error message details"]
    }
    ```
*   **Error Response (500 Internal Server Error):** `ApiResponse`
    ```json
    {
      "IsSuccess": false,
      "Message": "Bir hata oluştu",
      "Data": null,
      "Errors": ["Bir hata oluştu"]
    }
    ```

---

### 2. User Login

*   **Endpoint:** `POST /api/Auth/login`
*   **Description:** Logs in an existing user.
*   **Authorization:** `[AllowAnonymous]` (No authentication required)
*   **Request Body:** `LoginRequest`
    ```json
    {
      "Email": "user@example.com",
      "Password": "Password123!",
      "RememberMe": false
    }
    ```
*   **Success Response (200 OK):** `ApiResponse<AuthTokenResponse>` (Same structure as registration success)
    ```json
    {
      "IsSuccess": true,
      "Message": "Giriş başarılı",
      "Data": {
        "AccessToken": "your_access_token",
        "RefreshToken": "your_refresh_token",
        "ExpiresAt": "2024-01-01T12:00:00Z",
        "User": {
          "Id": "user_guid",
          "Email": "user@example.com",
          "UserName": "johndoe",
          "FirstName": "John",
          "LastName": "Doe",
          "FullName": "John Doe",
          "ProfileImageUrl": null,
          "EmailConfirmed": true
        }
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Invalid credentials)
    ```json
    {
      "IsSuccess": false,
      "Message": "Invalid credentials",
      "Data": null,
      "Errors": ["Invalid credentials"]
    }
    ```
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 3. Token Refresh

*   **Endpoint:** `POST /api/Auth/refresh`
*   **Description:** Refreshes an expired access token using a valid refresh token.
*   **Authorization:** `[AllowAnonymous]` (Technically no auth, but requires a valid refresh token)
*   **Request Body:** `RefreshTokenRequest`
    ```json
    {
      "RefreshToken": "your_valid_refresh_token"
    }
    ```
*   **Success Response (200 OK):** `ApiResponse<AuthTokenResponse>` (Same structure as registration/login success)
    ```json
    {
      "IsSuccess": true,
      "Message": "Token yenilendi",
      "Data": {
        "AccessToken": "new_access_token",
        "RefreshToken": "new_refresh_token", // Can be a new or the same refresh token
        "ExpiresAt": "2024-01-01T13:00:00Z", // New expiry for access token
        "User": { ... } // User details
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Invalid refresh token)
    ```json
    {
      "IsSuccess": false,
      "Message": "Invalid refresh token",
      "Data": null,
      "Errors": ["Invalid refresh token"]
    }
    ```
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 4. User Logout

*   **Endpoint:** `POST /api/Auth/logout`
*   **Description:** Logs out the currently authenticated user by invalidating their session/token if applicable on the server-side.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Request Body:** None
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Çıkış başarılı",
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 5. Email Confirmation

*   **Endpoint:** `GET /api/Auth/confirm-email`
*   **Description:** Confirms a user's email address using a token sent to their email.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `email`: `string` (The user's email address)
    *   `token`: `string` (The confirmation token)
*   **Example URL:** `/api/Auth/confirm-email?email=user@example.com&token=confirmation_token_value`
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Email doğrulandı",
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Invalid token or email)
    ```json
    {
      "IsSuccess": false,
      "Message": "Invalid token or email",
      "Data": null,
      "Errors": ["Invalid token or email"]
    }
    ```
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 6. Forgot Password

*   **Endpoint:** `POST /api/Auth/forgot-password`
*   **Description:** Initiates the password reset process. Sends a password reset link/token to the user's email.
*   **Authorization:** `[AllowAnonymous]`
*   **Request Body:** `ForgotPasswordRequest`
    ```json
    {
      "Email": "user@example.com"
    }
    ```
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Şifre sıfırlama linki email adresinize gönderildi",
      "Data": null,
      "Errors": null
    }
    ```
    *(Note: The response is the same regardless of whether the email exists in the system to prevent email enumeration.)*
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 7. Reset Password

*   **Endpoint:** `POST /api/Auth/reset-password`
*   **Description:** Resets the user's password using the token from the forgot password email.
*   **Authorization:** `[AllowAnonymous]`
*   **Request Body:** `ResetPasswordRequest`
    ```json
    {
      "Email": "user@example.com",
      "Token": "password_reset_token",
      "NewPassword": "NewPassword123!",
      "ConfirmPassword": "NewPassword123!"
    }
    ```
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Şifre başarıyla sıfırlandı",
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Invalid token, passwords do not match)
    ```json
    {
      "IsSuccess": false,
      "Message": "Error message (e.g., Invalid token, Passwords do not match)",
      "Data": null,
      "Errors": ["Error details"]
    }
    ```
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 8. Change Password

*   **Endpoint:** `POST /api/Auth/change-password`
*   **Description:** Allows an authenticated user to change their current password.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Request Body:** `ChangePasswordRequest`
    ```json
    {
      "CurrentPassword": "OldPassword123!",
      "NewPassword": "NewPassword123!",
      "ConfirmPassword": "NewPassword123!"
    }
    ```
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Şifre başarıyla değiştirildi",
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Incorrect current password, new passwords do not match)
    ```json
    {
      "IsSuccess": false,
      "Message": "Error message (e.g., Incorrect current password)",
      "Data": null,
      "Errors": ["Error details"]
    }
    ```
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

**Note on JWT Handling:**

The `AccessToken` returned upon successful login, registration, or token refresh is a JSON Web Token (JWT). This token must be:
1.  **Stored securely** by the client application (e.g., in HttpOnly cookies, secure storage).
2.  **Sent with subsequent requests** to protected API endpoints in the `Authorization` header using the `Bearer` scheme.
    Example: `Authorization: Bearer your_access_token`

The `RefreshToken` is used to obtain a new `AccessToken` when the current one expires. It should also be stored securely and is typically longer-lived than the `AccessToken`.
The specific details for `ApiResponse.CreateSuccess` and `ApiResponse.CreateFailure` structure would depend on their implementation, but generally, `CreateSuccess` wraps the data with a success message, and `CreateFailure` provides an error message and possibly detailed error codes/strings. The examples above follow a common pattern for such responses.
