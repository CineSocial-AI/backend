## Review API Documentation

This document outlines the API endpoints for managing movie reviews, their likes, and comments within the CineSocial platform. Actions like creating reviews, liking, or commenting require user authentication.

---

### Common DTO Structures

For clarity, here are assumed structures for commonly used Data Transfer Objects (DTOs) related to Reviews.

*   **`UserSummaryDto`** (Assumed, used within Review and Comment DTOs):
    ```json
    {
      "Id": "user_guid",
      "UserName": "reviewer_username",
      "ProfileImageUrl": "https://example.com/userprofile.jpg"
    }
    ```

*   **`MovieSummaryDto`** (Assumed, used within Review DTO):
    ```json
    {
      "Id": "movie_guid",
      "Title": "Inception",
      "ReleaseYear": 2010,
      "PosterImageUrl": "https://example.com/movie_poster.jpg"
    }
    ```

*   **`ReviewDto`**: Represents a movie review.
    ```json
    {
      "Id": "review_guid",
      "User": { // UserSummaryDto
        "Id": "user_guid_reviewer",
        "UserName": "cinephile_joe",
        "ProfileImageUrl": "https://example.com/joe.jpg"
      },
      "Movie": { // MovieSummaryDto
        "Id": "movie_guid_reviewed",
        "Title": "The Cinematic Masterpiece",
        "ReleaseYear": 2023,
        "PosterImageUrl": "https://example.com/masterpiece.jpg"
      },
      "Rating": 5, // e.g., 1-5 or 1-10 scale
      "Title": "An Unforgettable Experience!", // Optional review title
      "Content": "This movie was absolutely stunning. The cinematography, acting, and score were all top-notch...",
      "CreatedAt": "2023-06-01T14:00:00Z",
      "UpdatedAt": "2023-06-01T15:30:00Z",
      "LikeCount": 75,
      "CurrentUserLiked": true, // Boolean indicating if the logged-in user liked this review
      "CommentCount": 12
    }
    ```

*   **`CreateReviewDto`**: Used for creating a new review.
    ```json
    {
      "MovieId": "movie_guid_to_review",
      "Rating": 4,
      "Title": "Great Film, Minor Flaws", // Optional
      "Content": "I really enjoyed this movie overall, but there were a few plot points that didn't quite land for me."
    }
    ```

*   **`CommentDto`**: Represents a comment on a review. (Could be similar or identical to `PostCommentDto` if a generic comment system is used).
    ```json
    {
      "Id": "comment_guid",
      "ReviewId": "review_guid", // ID of the review this comment belongs to
      "Author": { // UserSummaryDto
        "Id": "user_guid_commenter",
        "UserName": "film_fan_jane",
        "ProfileImageUrl": "https://example.com/jane.jpg"
      },
      "Content": "Well said! I had similar thoughts on the third act.",
      "CreatedAt": "2023-06-02T10:00:00Z",
      "UpdatedAt": "2023-06-02T10:05:00Z"
      // May include like counts for comments, replies, etc.
    }
    ```

*   **`CreateCommentDto`**: Used for creating a new comment on a review.
    ```json
    {
      "ReviewId": "review_guid_to_comment_on", // ID of the review
      "Content": "I agree with your assessment!"
      // ParentCommentId could be here if replying to another comment
    }
    ```

*   **`LikeReviewRequest`**: Used for liking/unliking a review.
    ```json
    true
    ```
    *(Note: The controller action `LikeReview` takes `[FromBody] bool isLike = true`. This means the raw boolean value `true` or `false` is expected in the request body, not a JSON object like `{"isLike": true}`).*

*   **`PaginatedResponse<T>`**: Generic wrapper for paginated results.
    ```json
    {
      "Items": [ /* Array of T */ ],
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

### 1. Get Reviews (Paginated List)

*   **Endpoint:** `GET /api/Reviews`
*   **Description:** Retrieves a paginated list of reviews, with options for filtering by movie or user.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - Page number.
    *   `pageSize`: `int` (Default: `20`) - Number of reviews per page.
    *   `movieId`: `Guid?` (Optional) - Filter reviews for a specific movie.
    *   `userId`: `Guid?` (Optional) - Filter reviews made by a specific user.
*   **Success Response (200 OK):** `ApiResponse<PaginatedResponse<ReviewDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        "Items": [ /* Array of ReviewDto objects */ ],
        "TotalCount": 45,
        "Page": 1,
        "PageSize": 20
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse`
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 2. Get Review by ID

*   **Endpoint:** `GET /api/Reviews/{id}`
*   **Description:** Retrieves detailed information for a specific review by its ID.
*   **Authorization:** `[AllowAnonymous]`
*   **Path Parameter:**
    *   `id`: `Guid` - The unique identifier of the review.
*   **Success Response (200 OK):** `ApiResponse<ReviewDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": { /* Full ReviewDto object */ },
      "Errors": null
    }
    ```
*   **Error Response (404 Not Found):** `ApiResponse`
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 3. Create Review

*   **Endpoint:** `POST /api/Reviews`
*   **Description:** Creates a new movie review. The authenticated user is set as the author. A user typically cannot review the same movie multiple times (handled by service layer).
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Request Body:** `CreateReviewDto`
*   **Success Response (201 Created):** `ApiResponse<ReviewDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "İnceleme başarıyla oluşturuldu",
      "Data": { /* Full ReviewDto of the created review */ },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors, user already reviewed movie, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 4. Like/Unlike Review

*   **Endpoint:** `POST /api/Reviews/{id}/like`
*   **Description:** Allows an authenticated user to like or unlike a review.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the review to like/unlike.
*   **Request Body:** `boolean`
    *   Send `true` to like the review.
    *   Send `false` to unlike the review.
    *   Example: `true` (sent as raw boolean in the request body)
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    // When isLike was true
    {
      "IsSuccess": true,
      "Message": "İnceleme beğenildi", // "Review liked"
      "Data": null,
      "Errors": null
    }
    // When isLike was false
    {
      "IsSuccess": true,
      "Message": "İnceleme beğenilmedi", // "Review unliked"
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Review not found, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (404 Not Found):** `ApiResponse` (If the review with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 5. Get Review Comments

*   **Endpoint:** `GET /api/Reviews/{id}/comments`
*   **Description:** Retrieves paginated comments for a specific review.
*   **Authorization:** `[AllowAnonymous]`
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the review for which to retrieve comments.
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - Page number.
    *   `pageSize`: `int` (Default: `20`) - Number of comments per page.
*   **Success Response (200 OK):** `ApiResponse<PaginatedResponse<CommentDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        "Items": [ /* Array of CommentDto objects */ ],
        "TotalCount": 8,
        "Page": 1,
        "PageSize": 20
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Review not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 6. Create Review Comment

*   **Endpoint:** `POST /api/Reviews/comments`
*   **Description:** Creates a new comment on a review. The authenticated user is set as the author.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Request Body:** `CreateCommentDto`
*   **Success Response (200 OK):** `ApiResponse<CommentDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Yorum başarıyla oluşturuldu",
      "Data": { /* Full CommentDto of the created comment */ },
      "Errors": null
    }
    ```
    *(Note: The controller returns 200 OK, not 201 Created for comments.)*
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors, Review not found, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---
