## Genre API Documentation

This document outlines the API endpoints for managing movie genres within the CineSocial platform.

---

### Common DTO Structures

For clarity, here are assumed structures for commonly used Data Transfer Objects (DTOs) related to Genres.

*   **`GenreDto`**: Represents a movie genre.
    ```json
    {
      "Id": "genre_guid",
      "Name": "Action"
    }
    ```

*   **`CreateGenreDto`**: Used for creating a new genre.
    ```json
    {
      "Name": "Sci-Fi"
    }
    ```

*   **`ApiResponse<T>`**: A generic wrapper for API responses.
    ```json
    {
      "IsSuccess": true,
      "Message": "Optional success message",
      "Data": { /* Payload of type T */ },
      "Errors": null // Or an array of error strings/objects if IsSuccess is false
    }
    ```

---

### 1. Get All Genres

*   **Endpoint:** `GET /api/Genres`
*   **Description:** Retrieves a list of all movie genres.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:** None
*   **Success Response (200 OK):** `ApiResponse<List<GenreDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": [
        {
          "Id": "genre_guid_1",
          "Name": "Action"
        },
        {
          "Id": "genre_guid_2",
          "Name": "Comedy"
        },
        {
          "Id": "genre_guid_3",
          "Name": "Drama"
        }
        // ... more genres
      ],
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (Though unlikely for this endpoint without parameters, could occur if underlying service fails validation)
*   **Error Response (500 Internal Server Error):** `ApiResponse`
    ```json
    {
      "IsSuccess": false,
      "Message": "Bir hata oluştu", // "An error occurred"
      "Data": null,
      "Errors": ["Bir hata oluştu"]
    }
    ```

---

### 2. Create Genre (Admin Only)

*   **Endpoint:** `POST /api/Genres`
*   **Description:** Creates a new movie genre. **Requires Admin privileges.**
*   **Authorization:** `[Authorize(Roles = "Admin")]` (Requires Bearer token with Admin role)
*   **Request Body:** `CreateGenreDto`
    ```json
    {
      "Name": "Science Fiction"
    }
    ```
*   **Success Response (200 OK):** `ApiResponse<GenreDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Tür başarıyla oluşturuldu", // "Genre created successfully"
      "Data": {
        "Id": "new_genre_guid",
        "Name": "Science Fiction"
      },
      "Errors": null
    }
    ```
    *(Note: The controller returns 200 OK, not 201 Created, which is a slight deviation from typical RESTful practice for creation, but documented as per the controller code.)*
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors for `CreateGenreDto`, such as duplicate name)
    ```json
    {
      "IsSuccess": false,
      "Message": "Genre with this name already exists.", // Example error message
      "Data": null,
      "Errors": ["Genre with this name already exists."]
    }
    ```
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (403 Forbidden):** If authenticated but not an Admin.
*   **Error Response (500 Internal Server Error):** `ApiResponse`
    ```json
    {
      "IsSuccess": false,
      "Message": "Bir hata oluştu", // "An error occurred"
      "Data": null,
      "Errors": ["Bir hata oluştu"]
    }
    ```

---
