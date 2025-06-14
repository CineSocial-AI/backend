## Movie API Documentation

This document outlines the API endpoints for managing movies within the CineSocial platform.

---

### Common DTO Structures

For clarity, here are assumed structures for commonly used Data Transfer Objects (DTOs). The exact fields might vary based on the application's domain models.

*   **`MovieSummaryDto`**: Represents a movie in a list or summary view.
    ```json
    {
      "Id": "movie_guid",
      "Title": "Inception",
      "ReleaseYear": 2010,
      "PosterImageUrl": "https://example.com/poster.jpg",
      "AverageRating": 8.8,
      "Director": "Christopher Nolan", // Or a simplified Director DTO
      "Genres": ["Action", "Sci-Fi", "Thriller"] // Or list of Genre DTOs
    }
    ```

*   **`MovieDto`**: Represents a detailed view of a movie.
    ```json
    {
      "Id": "movie_guid",
      "Title": "Inception",
      "Overview": "A thief who steals information by entering people's dreams...",
      "ReleaseDate": "2010-07-16T00:00:00Z",
      "DurationMinutes": 148,
      "PosterImageUrl": "https://example.com/poster.jpg",
      "BannerImageUrl": "https://example.com/banner.jpg",
      "AverageRating": 8.8,
      "MpaRating": "PG-13", // e.g., G, PG, PG-13, R, NC-17
      "Language": "English",
      "TrailerUrl": "https://youtube.com/watch?v=...",
      "Director": { // Detailed Director DTO
        "Id": "director_guid",
        "Name": "Christopher Nolan",
        "ProfileImageUrl": "https://example.com/nolan.jpg"
      },
      "Cast": [ // List of CastMemberDto
        {
          "Id": "actor_guid",
          "Name": "Leonardo DiCaprio",
          "CharacterName": "Cobb",
          "ProfileImageUrl": "https://example.com/dicaprio.jpg"
        }
      ],
      "Crew": [ // List of CrewMemberDto
        {
          "Id": "crew_guid",
          "Name": "Hans Zimmer",
          "Role": "Composer",
          "ProfileImageUrl": "https://example.com/zimmer.jpg"
        }
      ],
      "Genres": [ // List of GenreDto
        { "Id": "genre_guid_1", "Name": "Action" },
        { "Id": "genre_guid_2", "Name": "Sci-Fi" }
      ],
      "ProductionCompanies": [ // List of CompanyDto
        { "Id": "company_guid", "Name": "Warner Bros." }
      ],
      "CountryOfOrigin": "USA"
    }
    ```

*   **`CreateMovieDto`**: Used for creating a new movie.
    ```json
    {
      "Title": "New Movie Title",
      "Overview": "Brief description of the new movie.",
      "ReleaseDate": "2025-01-01T00:00:00Z",
      "DurationMinutes": 120,
      "PosterImageUrl": "https://example.com/new_poster.jpg",
      "BannerImageUrl": "https://example.com/new_banner.jpg",
      "MpaRating": "PG",
      "Language": "English",
      "TrailerUrl": "https://youtube.com/watch?v=new_trailer",
      "DirectorId": "director_guid", // Assuming director is an existing entity
      "GenreIds": ["genre_guid_1", "genre_guid_2"], // List of existing Genre IDs
      "CastIds": [{ "PersonId": "actor_guid_1", "CharacterName": "Main Character" }], // Simplified for example
      "CrewIds": [{ "PersonId": "crew_guid_1", "Role": "Lead Editor" }], // Simplified for example
      "ProductionCompanyIds": ["company_guid_1"],
      "CountryOfOrigin": "Canada"
    }
    ```
    *(Note: The actual structure for `CastIds` and `CrewIds` might involve more complex objects if creating new persons or roles simultaneously.)*

*   **`UpdateMovieDto`**: Used for updating an existing movie. Similar to `CreateMovieDto`, but all fields are optional.
    ```json
    {
      "Title": "Updated Movie Title", // Only include fields to be updated
      "Overview": "Updated description.",
      "ReleaseDate": "2025-01-02T00:00:00Z"
      // ... other fields as needed
    }
    ```

*   **`PaginatedResponse<T>`**: A generic wrapper for paginated results.
    ```json
    {
      "Items": [ /* Array of T, e.g., MovieSummaryDto */ ],
      "TotalCount": 100,
      "Page": 1,
      "PageSize": 20
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

### 1. Get Movies (Paginated List)

*   **Endpoint:** `GET /api/Movies`
*   **Description:** Retrieves a paginated list of movies, with options for searching, filtering by genre, and sorting.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - The page number to retrieve.
    *   `pageSize`: `int` (Default: `20`) - The number of movies per page.
    *   `search`: `string` (Optional) - Search term to filter movies by title or other relevant fields.
    *   `genreIds`: `List<Guid>` (Optional) - A list of genre IDs to filter movies. Example: `genreIds=guid1&genreIds=guid2`
    *   `sortBy`: `string` (Optional) - Field to sort by (e.g., `title_asc`, `releaseDate_desc`, `rating_desc`).
*   **Success Response (200 OK):** `ApiResponse<PaginatedResponse<MovieSummaryDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        "Items": [
          // Array of MovieSummaryDto objects
          {
            "Id": "movie_guid_1",
            "Title": "Inception",
            "ReleaseYear": 2010,
            "PosterImageUrl": "https://example.com/poster1.jpg",
            "AverageRating": 8.8,
            "Director": "Christopher Nolan",
            "Genres": ["Action", "Sci-Fi"]
          },
          // ... more movies
        ],
        "TotalCount": 50,
        "Page": 1,
        "PageSize": 20
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Invalid pagination parameters)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 2. Get Movie by ID

*   **Endpoint:** `GET /api/Movies/{id}`
*   **Description:** Retrieves detailed information for a specific movie by its ID.
*   **Authorization:** `[AllowAnonymous]`
*   **Path Parameter:**
    *   `id`: `Guid` - The unique identifier of the movie.
*   **Success Response (200 OK):** `ApiResponse<MovieDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        // Full MovieDto object
        "Id": "movie_guid_1",
        "Title": "Inception",
        "Overview": "A thief who steals information by entering people's dreams...",
        // ... other MovieDto fields
      },
      "Errors": null
    }
    ```
*   **Error Response (404 Not Found):** `ApiResponse` (If the movie with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 3. Create Movie (Admin Only)

*   **Endpoint:** `POST /api/Movies`
*   **Description:** Creates a new movie. **Requires Admin privileges.**
*   **Authorization:** `[Authorize(Roles = "Admin")]` (Requires Bearer token with Admin role)
*   **Request Body:** `CreateMovieDto` (See common DTO structures for an example)
*   **Success Response (201 Created):** `ApiResponse<MovieDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Film başarıyla oluşturuldu",
      "Data": {
        // Full MovieDto of the created movie
        "Id": "new_movie_guid",
        "Title": "New Movie Title",
        // ... other MovieDto fields
      },
      "Errors": null
    }
    ```
    *(The response includes a `Location` header pointing to the newly created resource: `/api/Movies/new_movie_guid`)*
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors for `CreateMovieDto`)
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (403 Forbidden):** If authenticated but not an Admin.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 4. Update Movie (Admin Only)

*   **Endpoint:** `PUT /api/Movies/{id}`
*   **Description:** Updates an existing movie. **Requires Admin privileges.**
*   **Authorization:** `[Authorize(Roles = "Admin")]` (Requires Bearer token with Admin role)
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the movie to update.
*   **Request Body:** `UpdateMovieDto` (See common DTO structures for an example. Include only fields to be updated.)
*   **Success Response (200 OK):** `ApiResponse<MovieDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Film başarıyla güncellendi",
      "Data": {
        // Full MovieDto of the updated movie
        "Id": "movie_guid_1",
        "Title": "Updated Movie Title",
        // ... other MovieDto fields reflecting updates
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors for `UpdateMovieDto`, or if ID in path and body mismatch if applicable)
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (403 Forbidden):** If authenticated but not an Admin.
*   **Error Response (404 Not Found):** `ApiResponse` (If the movie with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 5. Delete Movie (Admin Only)

*   **Endpoint:** `DELETE /api/Movies/{id}`
*   **Description:** Deletes a movie. **Requires Admin privileges.**
*   **Authorization:** `[Authorize(Roles = "Admin")]` (Requires Bearer token with Admin role)
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the movie to delete.
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Film başarıyla silindi",
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., If the movie cannot be deleted due to business rules)
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (403 Forbidden):** If authenticated but not an Admin.
*   **Error Response (404 Not Found):** `ApiResponse` (If the movie with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 6. Get Popular Movies

*   **Endpoint:** `GET /api/Movies/popular`
*   **Description:** Retrieves a list of popular movies, typically based on ratings, views, or other engagement metrics.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `count`: `int` (Default: `10`) - The number of popular movies to retrieve.
*   **Success Response (200 OK):** `ApiResponse<List<MovieSummaryDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": [
        // Array of MovieSummaryDto objects
        {
          "Id": "movie_guid_popular_1",
          "Title": "Popular Movie 1",
          // ... other MovieSummaryDto fields
        },
        // ... up to 'count' movies
      ],
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse`
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 7. Get Top-Rated Movies

*   **Endpoint:** `GET /api/Movies/top-rated`
*   **Description:** Retrieves a list of the highest-rated movies.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `count`: `int` (Default: `10`) - The number of top-rated movies to retrieve.
*   **Success Response (200 OK):** `ApiResponse<List<MovieSummaryDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": [
        // Array of MovieSummaryDto objects, ordered by rating
        {
          "Id": "movie_guid_top_1",
          "Title": "Top Rated Movie 1",
          "AverageRating": 9.5,
          // ... other MovieSummaryDto fields
        },
        // ... up to 'count' movies
      ],
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse`
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 8. Get Recent Movies

*   **Endpoint:** `GET /api/Movies/recent`
*   **Description:** Retrieves a list of the most recently added or released movies.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `count`: `int` (Default: `10`) - The number of recent movies to retrieve.
*   **Success Response (200 OK):** `ApiResponse<List<MovieSummaryDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": [
        // Array of MovieSummaryDto objects, ordered by recency
        {
          "Id": "movie_guid_recent_1",
          "Title": "Recent Movie 1",
          "ReleaseYear": 2024, // Or full ReleaseDate
          // ... other MovieSummaryDto fields
        },
        // ... up to 'count' movies
      ],
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse`
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---
