## Post API Documentation

This document outlines the API endpoints for managing posts, comments, and reactions within the CineSocial platform. Most actions like creating posts, commenting, or reacting require user authentication.

---

### Common DTO Structures & Enums

For clarity, here are assumed structures for commonly used Data Transfer Objects (DTOs) and enumerations related to Posts.

*   **`UserSummaryDto`** (Assumed, used within Post DTOs):
    ```json
    {
      "Id": "user_guid",
      "UserName": "post_author",
      "ProfileImageUrl": "https://example.com/userprofile.jpg"
    }
    ```

*   **`PostSummaryDto`**: Represents a post in a list or feed.
    ```json
    {
      "Id": "post_guid",
      "Title": "My Thoughts on Recent Movies", // Optional, depending on post type
      "ContentPreview": "This is a short preview of the post content...", // Or full content if not too long
      "Author": { // UserSummaryDto
        "Id": "user_guid_author",
        "UserName": "author_username",
        "ProfileImageUrl": "https://example.com/author.jpg"
      },
      "Group": { // Optional, GroupSummaryDto or similar
        "Id": "group_guid",
        "Name": "Movie Buffs Community"
      },
      "CreatedAt": "2023-05-01T10:00:00Z",
      "UpdatedAt": "2023-05-01T11:00:00Z",
      "CommentCount": 15,
      "ReactionCounts": { // Example reaction counts
        "Like": 50,
        "Love": 10
      },
      "CurrentUserReaction": "Like" // String or null, represents logged-in user's reaction
      // May include media like images/videos if applicable
    }
    ```

*   **`PostDto`**: Represents a detailed view of a post.
    ```json
    {
      "Id": "post_guid",
      "Title": "My In-Depth Review of 'The Grand Cinema'", // Optional
      "Content": "Full content of the post, which could be lengthy text, HTML, or Markdown.",
      "Author": { // UserSummaryDto
        "Id": "user_guid_author",
        "UserName": "author_username",
        "ProfileImageUrl": "https://example.com/author.jpg"
      },
      "Group": { // Optional, GroupSummaryDto or similar
        "Id": "group_guid",
        "Name": "Movie Buffs Community",
        "ProfileImageUrl": "https://example.com/group_profile.jpg"
      },
      "CreatedAt": "2023-05-01T10:00:00Z",
      "UpdatedAt": "2023-05-01T11:00:00Z",
      "CommentCount": 15,
      "ReactionCounts": {
        "Like": 50,
        "Love": 10,
        "Haha": 5
      },
      "CurrentUserReaction": "Love", // String or null
      "MediaUrls": [ // Optional array of URLs for images, videos, etc.
        "https://example.com/image1.jpg",
        "https://example.com/video.mp4"
      ],
      "Tags": ["review", "cinema", "new_release"] // Optional
    }
    ```

*   **`CreatePostDto`**: Used for creating a new post.
    ```json
    {
      "GroupId": "group_guid", // Optional, if posting to a specific group
      "Title": "Exciting News!", // Optional
      "Content": "Check out this new trailer I found...",
      "MediaUrls": ["https://youtube.com/watch?v=trailer_id"] // Optional
      // Tags might also be included here
    }
    ```

*   **`PostCommentDto`**: Represents a comment on a post.
    ```json
    {
      "Id": "comment_guid",
      "PostId": "post_guid",
      "Author": { // UserSummaryDto
        "Id": "user_guid_commenter",
        "UserName": "commenter_username",
        "ProfileImageUrl": "https://example.com/commenter.jpg"
      },
      "Content": "Great post! I totally agree with your points.",
      "CreatedAt": "2023-05-01T12:00:00Z",
      "UpdatedAt": "2023-05-01T12:05:00Z",
      "ReactionCounts": {
        "Like": 5
      },
      "CurrentUserReaction": null // String or null
      // Support for nested comments/replies might add a ParentCommentId and a list of Replies
    }
    ```

*   **`CreatePostCommentDto`**: Used for creating a new comment on a post.
    ```json
    {
      "PostId": "post_guid_to_comment_on",
      "Content": "This is my reply to the post."
      // ParentCommentId could be here if replying to another comment
    }
    ```

*   **`ReactionType`** (Enum, assumed values):
    ```typescript // Using TypeScript syntax for enum representation
    enum ReactionType {
      Like,
      Love,
      Haha,
      Wow,
      Sad,
      Angry
    }
    ```

*   **`ReactionRequest`**: Used for reacting to a post.
    ```json
    {
      "Type": "Like" // String representation of ReactionType, e.g., "Like", "Love"
    }
    ```

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

### 1. Get Posts (Paginated List)

*   **Endpoint:** `GET /api/Posts`
*   **Description:** Retrieves a paginated list of posts, with options for filtering by group, user, search term, and sorting.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - Page number.
    *   `pageSize`: `int` (Default: `20`) - Number of posts per page.
    *   `groupId`: `Guid?` (Optional) - Filter posts belonging to a specific group.
    *   `userId`: `Guid?` (Optional) - Filter posts made by a specific user.
    *   `search`: `string?` (Optional) - Search term for post content or title.
    *   `sortBy`: `string?` (Optional) - Sorting criteria (e.g., `createdAt_desc`, `popularity_desc`).
*   **Success Response (200 OK):** `ApiResponse<PaginatedResponse<PostSummaryDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        "Items": [ /* Array of PostSummaryDto objects */ ],
        "TotalCount": 120,
        "Page": 1,
        "PageSize": 20
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse`
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 2. Get Post by ID

*   **Endpoint:** `GET /api/Posts/{id}`
*   **Description:** Retrieves detailed information for a specific post by its ID.
*   **Authorization:** `[AllowAnonymous]`
*   **Path Parameter:**
    *   `id`: `Guid` - The unique identifier of the post.
*   **Success Response (200 OK):** `ApiResponse<PostDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": { /* Full PostDto object */ },
      "Errors": null
    }
    ```
*   **Error Response (404 Not Found):** `ApiResponse`
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 3. Create Post

*   **Endpoint:** `POST /api/Posts`
*   **Description:** Creates a new post. The authenticated user is set as the author.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Request Body:** `CreatePostDto`
*   **Success Response (201 Created):** `ApiResponse<PostDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Post başarıyla oluşturuldu",
      "Data": { /* Full PostDto of the created post */ },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 4. Get Post Comments

*   **Endpoint:** `GET /api/Posts/{id}/comments`
*   **Description:** Retrieves paginated comments for a specific post.
*   **Authorization:** `[AllowAnonymous]`
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the post for which to retrieve comments.
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - Page number.
    *   `pageSize`: `int` (Default: `20`) - Number of comments per page.
*   **Success Response (200 OK):** `ApiResponse<PaginatedResponse<PostCommentDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        "Items": [ /* Array of PostCommentDto objects */ ],
        "TotalCount": 35,
        "Page": 1,
        "PageSize": 20
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Post not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 5. Create Post Comment

*   **Endpoint:** `POST /api/Posts/comments`
*   **Description:** Creates a new comment on a post. The authenticated user is set as the author.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Request Body:** `CreatePostCommentDto`
*   **Success Response (200 OK):** `ApiResponse<PostCommentDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Yorum başarıyla oluşturuldu",
      "Data": { /* Full PostCommentDto of the created comment */ },
      "Errors": null
    }
    ```
    *(Note: The controller returns 200 OK, not 201 Created for comments.)*
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors, Post not found, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 6. React to Post

*   **Endpoint:** `POST /api/Posts/{id}/react`
*   **Description:** Allows an authenticated user to add or remove a reaction to a post. Sending the same reaction type might remove it (toggle behavior), or the service might handle updating an existing reaction.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the post to react to.
*   **Request Body:** `ReactionRequest`
    ```json
    {
      "Type": "Like" // Or "Love", "Haha", etc. from ReactionType enum
    }
    ```
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Reaksiyon başarıyla eklendi", // Or "Reaksiyon güncellendi/kaldırıldı"
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Invalid reaction type, Post not found, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (404 Not Found):** `ApiResponse` (If the post with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 7. Get Trending Posts

*   **Endpoint:** `GET /api/Posts/trending`
*   **Description:** Retrieves a list of trending posts based on engagement or other metrics.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `count`: `int` (Default: `10`) - The number of trending posts to retrieve.
*   **Success Response (200 OK):** `ApiResponse<List<PostSummaryDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": [ /* Array of PostSummaryDto objects */ ],
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse`
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 8. Get User Feed

*   **Endpoint:** `GET /api/Posts/feed`
*   **Description:** Retrieves a personalized feed of posts for the authenticated user. This typically includes posts from followed users, joined groups, etc.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - Page number for the feed.
    *   `pageSize`: `int` (Default: `20`) - Number of posts per page in the feed.
*   **Success Response (200 OK):** `ApiResponse<List<PostSummaryDto>>`
    *(Note: The controller shows `List<PostSummaryDto>` but for a feed, `PaginatedResponse<PostSummaryDto>` might be more appropriate for consistency. Documenting as per controller.)*
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": [ /* Array of PostSummaryDto objects for the user's feed */ ],
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---
