## Watchlist API Documentation

This document outlines the API endpoints for managing a user's movie watchlist within the CineSocial platform. All watchlist operations require user authentication.

---

### Common DTO Structures

For clarity, here are assumed structures for commonly used Data Transfer Objects (DTOs) related to Watchlists.

*   **`MovieSummaryDto`** (Assumed, used within WatchlistDto):
    ```json
    {
      "Id": "movie_guid",
      "Title": "Pulp Fiction",
      "ReleaseYear": 1994,
      "PosterImageUrl": "https://example.com/pulp_fiction_poster.jpg",
      "Director": "Quentin Tarantino"
    }
    ```

*   **`WatchlistDto`**: Represents an item in the user's watchlist.
    ```json
    {
      "Id": "watchlist_item_guid", // Unique ID for the watchlist entry itself
      "UserId": "user_guid",
      "Movie": { // MovieSummaryDto
        "Id": "movie_guid_1",
        "Title": "The Shawshank Redemption",
        "ReleaseYear": 1994,
        "PosterImageUrl": "https://example.com/shawshank.jpg",
        "Director": "Frank Darabont"
      },
      "AddedAt": "2023-07-01T10:00:00Z",
      "IsWatched": false, // Indicates if the user has marked this movie as watched
      "WatchedAt": null // Timestamp when marked as watched, null if not watched
    }
    ```

*   **`AddToWatchlistDto`**: Used for adding a movie to the watchlist.
    ```json
    {
      "MovieId": "movie_guid_to_add",
      "IsWatched": false // Optional, defaults to false. Can be set to true if adding a movie already watched.
    }
    ```

*   **`PaginatedResponse<T>`**: Generic wrapper for paginated results.
    ```json
    {
      "Items": [ /* Array of T, e.g., WatchlistDto */ ],
      "TotalCount": 100,
      "Page": 1,
      "PageSize": 20
    }
    ```

*   **`ApiResponse<T>`**: Generic wrapper for API responses.
    ```json
    {
      "IsSuccess": true,
      "Message": "Optional success message",
      "Data": { /* Payload of type T */ },
      "Errors": null // Or an array of error strings/objects if IsSuccess is false
    }
    ```

---

### 1. Get User Watchlist

*   **Endpoint:** `GET /api/Watchlist`
*   **Description:** Retrieves the authenticated user's watchlist, paginated, with an option to filter by watched status.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - Page number.
    *   `pageSize`: `int` (Default: `20`) - Number of watchlist items per page.
    *   `isWatched`: `bool?` (Optional) - Filter watchlist items by their watched status (e.g., `true` for watched, `false` for unwatched). If null, returns all items.
*   **Success Response (200 OK):** `ApiResponse<PaginatedResponse<WatchlistDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        "Items": [
          {
            "Id": "watchlist_item_guid_1",
            "UserId": "current_user_guid",
            "Movie": {
              "Id": "movie_guid_abc",
              "Title": "Fight Club",
              "ReleaseYear": 1999,
              "PosterImageUrl": "https://example.com/fightclub.jpg",
              "Director": "David Fincher"
            },
            "AddedAt": "2023-07-15T11:00:00Z",
            "IsWatched": false,
            "WatchedAt": null
          }
          // ... more watchlist items
        ],
        "TotalCount": 25,
        "Page": 1,
        "PageSize": 20
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., "Kullanıcı kimliği bulunamadı" if token is invalid)
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 2. Add to Watchlist

*   **Endpoint:** `POST /api/Watchlist`
*   **Description:** Adds a movie to the authenticated user's watchlist or updates its `IsWatched` status if already present.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Request Body:** `AddToWatchlistDto`
    ```json
    {
      "MovieId": "movie_guid_to_add_or_update",
      "IsWatched": true // Set to true to mark as watched, false to mark as unwatched
    }
    ```
*   **Success Response (200 OK):** `ApiResponse<WatchlistDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Film izleme listesine eklendi", // "Movie added to watchlist" or a similar message indicating update
      "Data": {
        // Full WatchlistDto of the added or updated item
        "Id": "watchlist_item_guid_xyz",
        "UserId": "current_user_guid",
        "Movie": {
          "Id": "movie_guid_to_add_or_update",
          "Title": "The Matrix",
          "ReleaseYear": 1999,
          "PosterImageUrl": "https://example.com/matrix.jpg",
          "Director": "The Wachowskis"
        },
        "AddedAt": "2023-07-20T09:30:00Z", // Could be the original add time or update time
        "IsWatched": true,
        "WatchedAt": "2023-07-20T09:30:00Z" // Timestamp if IsWatched is true
      },
      "Errors": null
    }
    ```
    *(Note: The controller returns 200 OK, not 201 Created, as this can also be an update operation if the movie is already in the list but its `IsWatched` status is changed.)*
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Movie not found, validation errors for DTO, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 3. Check if Movie is in Watchlist

*   **Endpoint:** `GET /api/Watchlist/check/{movieId}`
*   **Description:** Checks if a specific movie is in the authenticated user's watchlist.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Path Parameter:**
    *   `movieId`: `Guid` - The ID of the movie to check.
*   **Success Response (200 OK):** `ApiResponse<bool>`
    ```json
    // If movie is in watchlist
    {
      "IsSuccess": true,
      "Message": null,
      "Data": true,
      "Errors": null
    }
    // If movie is NOT in watchlist
    {
      "IsSuccess": true,
      "Message": null,
      "Data": false,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Invalid movie ID format, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (404 Not Found):** `ApiResponse` (If the movie ID itself is valid but does not correspond to an existing movie - this might be handled by the service and return `false` or an error depending on implementation)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---
