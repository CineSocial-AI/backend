## Group API Documentation

This document outlines the API endpoints for managing groups within the CineSocial platform. User-specific actions like updating or deleting a group, or joining/leaving, require authentication. The system internally checks if the authenticated user has the necessary permissions (e.g., is a group admin/owner) for modification or deletion operations.

---

### Common DTO Structures

For clarity, here are assumed structures for commonly used Data Transfer Objects (DTOs) related to Groups.

*   **`GroupSummaryDto`**: Represents a group in a list or summary view.
    ```json
    {
      "Id": "group_guid",
      "Name": "Movie Lovers Club",
      "Description": "A club for people who love movies.",
      "ProfileImageUrl": "https://example.com/groupprofile.jpg",
      "MemberCount": 150,
      "IsPrivate": false,
      "CreatedAt": "2023-01-15T10:00:00Z"
    }
    ```

*   **`GroupDto`**: Represents a detailed view of a group.
    ```json
    {
      "Id": "group_guid",
      "Name": "Movie Lovers Club",
      "Description": "A detailed description of the Movie Lovers Club, its rules, and activities.",
      "ProfileImageUrl": "https://example.com/groupprofile.jpg",
      "BannerImageUrl": "https://example.com/groupbanner.jpg",
      "MemberCount": 150,
      "IsPrivate": false,
      "CreatedAt": "2023-01-15T10:00:00Z",
      "CreatedBy": { // UserSummaryDto or similar
        "Id": "user_guid_owner",
        "UserName": "group_owner",
        "ProfileImageUrl": "https://example.com/ownerprofile.jpg"
      },
      "Admins": [ // List of UserSummaryDto for group admins
        {
          "Id": "user_guid_admin1",
          "UserName": "group_admin1",
          "ProfileImageUrl": "https://example.com/admin1profile.jpg"
        }
      ]
      // May also include recent activity, rules, etc.
    }
    ```

*   **`CreateGroupDto`**: Used for creating a new group.
    ```json
    {
      "Name": "Sci-Fi Fans United",
      "Description": "A group for discussing all things science fiction cinema.",
      "IsPrivate": false, // Optional, defaults to false if not provided
      "ProfileImageUrl": "https://example.com/scifigroup.jpg", // Optional
      "BannerImageUrl": "https://example.com/scifibanner.jpg" // Optional
    }
    ```

*   **`UpdateGroupDto`**: Used for updating an existing group. All fields are optional.
    ```json
    {
      "Name": "Sci-Fi & Fantasy Fans United", // Only include fields to be updated
      "Description": "An updated description to include fantasy discussions.",
      "IsPrivate": true
      // ... other fields like ProfileImageUrl, BannerImageUrl
    }
    ```

*   **`GroupMemberDto`**: Represents a member of a group.
    ```json
    {
      "UserId": "user_guid_member",
      "UserName": "member_username",
      "FullName": "Member Full Name",
      "ProfileImageUrl": "https://example.com/memberprofile.jpg",
      "JoinedAt": "2023-02-01T12:00:00Z",
      "Role": "Member" // e.g., "Member", "Admin", "Owner"
    }
    ```

*   **`PaginatedResponse<T>`**: A generic wrapper for paginated results.
    ```json
    {
      "Items": [ /* Array of T, e.g., GroupSummaryDto or GroupMemberDto */ ],
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

### 1. Get Groups (Paginated List)

*   **Endpoint:** `GET /api/Groups`
*   **Description:** Retrieves a paginated list of groups, with options for searching and filtering by privacy.
*   **Authorization:** `[AllowAnonymous]`
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - The page number to retrieve.
    *   `pageSize`: `int` (Default: `20`) - The number of groups per page.
    *   `search`: `string` (Optional) - Search term to filter groups by name or description.
    *   `isPrivate`: `bool?` (Optional) - Filter groups by their privacy setting (e.g., `true` for private, `false` for public).
*   **Success Response (200 OK):** `ApiResponse<PaginatedResponse<GroupSummaryDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        "Items": [
          // Array of GroupSummaryDto objects
          {
            "Id": "group_guid_1",
            "Name": "Action Movie Fans",
            "Description": "Discussing the best action movies.",
            "ProfileImageUrl": "https://example.com/actiongroup.jpg",
            "MemberCount": 75,
            "IsPrivate": false,
            "CreatedAt": "2023-03-10T14:30:00Z"
          },
          // ... more groups
        ],
        "TotalCount": 30,
        "Page": 1,
        "PageSize": 20
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Invalid pagination parameters)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 2. Get Group by ID

*   **Endpoint:** `GET /api/Groups/{id}`
*   **Description:** Retrieves detailed information for a specific group by its ID.
*   **Authorization:** `[AllowAnonymous]` (The service layer might restrict access to some details of private groups if the user is not a member).
*   **Path Parameter:**
    *   `id`: `Guid` - The unique identifier of the group.
*   **Success Response (200 OK):** `ApiResponse<GroupDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        // Full GroupDto object
        "Id": "group_guid_1",
        "Name": "Action Movie Fans",
        "Description": "A detailed description...",
        // ... other GroupDto fields
      },
      "Errors": null
    }
    ```
*   **Error Response (404 Not Found):** `ApiResponse` (If the group with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 3. Create Group

*   **Endpoint:** `POST /api/Groups`
*   **Description:** Creates a new group. The authenticated user becomes the owner/admin.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Request Body:** `CreateGroupDto` (See common DTO structures for an example)
*   **Success Response (201 Created):** `ApiResponse<GroupDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Grup başarıyla oluşturuldu",
      "Data": {
        // Full GroupDto of the created group
        "Id": "new_group_guid",
        "Name": "Sci-Fi Fans United",
        // ... other GroupDto fields
      },
      "Errors": null
    }
    ```
    *(The response includes a `Location` header pointing to the newly created resource: `/api/Groups/new_group_guid`)*
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors for `CreateGroupDto`, or "Kullanıcı kimliği bulunamadı" if token is invalid/missing user ID)
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 4. Update Group

*   **Endpoint:** `PUT /api/Groups/{id}`
*   **Description:** Updates an existing group. Requires the authenticated user to be an admin or owner of the group.
*   **Authorization:** `[Authorize]` (Requires Bearer token; service layer handles permission check)
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the group to update.
*   **Request Body:** `UpdateGroupDto` (See common DTO structures. Include only fields to be updated.)
*   **Success Response (200 OK):** `ApiResponse<GroupDto>`
    ```json
    {
      "IsSuccess": true,
      "Message": "Grup başarıyla güncellendi",
      "Data": {
        // Full GroupDto of the updated group
        "Id": "group_guid_1",
        "Name": "Updated Group Name",
        // ... other GroupDto fields reflecting updates
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Validation errors, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (403 Forbidden):** `ApiResponse` (If authenticated user is not an admin/owner of the group - handled by service layer, may result in a generic error message like "Operation not allowed" or a specific one from the service)
*   **Error Response (404 Not Found):** `ApiResponse` (If the group with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 5. Delete Group

*   **Endpoint:** `DELETE /api/Groups/{id}`
*   **Description:** Deletes a group. Requires the authenticated user to be an admin or owner of the group.
*   **Authorization:** `[Authorize]` (Requires Bearer token; service layer handles permission check)
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the group to delete.
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Grup başarıyla silindi",
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (403 Forbidden):** `ApiResponse` (If authenticated user is not an admin/owner - handled by service layer)
*   **Error Response (404 Not Found):** `ApiResponse` (If the group with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 6. Get Group Members

*   **Endpoint:** `GET /api/Groups/{id}/members`
*   **Description:** Retrieves a paginated list of members for a specific group.
*   **Authorization:** `[AllowAnonymous]` (Access to members of a private group might be restricted if the user is not a member - service layer logic).
*   **Path Parameter:**
    *   `id`: `Guid` - The unique identifier of the group.
*   **Query Parameters:**
    *   `page`: `int` (Default: `1`) - The page number to retrieve.
    *   `pageSize`: `int` (Default: `20`) - The number of members per page.
*   **Success Response (200 OK):** `ApiResponse<PaginatedResponse<GroupMemberDto>>`
    ```json
    {
      "IsSuccess": true,
      "Message": null,
      "Data": {
        "Items": [
          // Array of GroupMemberDto objects
          {
            "UserId": "user_guid_member1",
            "UserName": "member1_username",
            "FullName": "Member One",
            "ProfileImageUrl": "https://example.com/member1.jpg",
            "JoinedAt": "2023-02-05T11:00:00Z",
            "Role": "Admin"
          },
          // ... more members
        ],
        "TotalCount": 25,
        "Page": 1,
        "PageSize": 20
      },
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Group not found, or pagination errors)
*   **Error Response (404 Not Found):** `ApiResponse` (If the group itself is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 7. Join Group

*   **Endpoint:** `POST /api/Groups/{id}/join`
*   **Description:** Allows an authenticated user to join a public group or request to join a private group.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the group to join.
*   **Request Body:** None
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Gruba başarıyla katıldınız", // "Successfully joined the group" or "Request to join sent" for private groups
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Already a member, group is private and not accepting requests, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (404 Not Found):** `ApiResponse` (If the group with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---

### 8. Leave Group

*   **Endpoint:** `POST /api/Groups/{id}/leave`
*   **Description:** Allows an authenticated user to leave a group they are a member of.
*   **Authorization:** `[Authorize]` (Requires Bearer token)
*   **Path Parameter:**
    *   `id`: `Guid` - The ID of the group to leave.
*   **Request Body:** None
*   **Success Response (200 OK):** `ApiResponse`
    ```json
    {
      "IsSuccess": true,
      "Message": "Gruptan başarıyla ayrıldınız", // "Successfully left the group"
      "Data": null,
      "Errors": null
    }
    ```
*   **Error Response (400 Bad Request):** `ApiResponse` (e.g., Not a member of the group, "Kullanıcı kimliği bulunamadı")
*   **Error Response (401 Unauthorized):** If not authenticated.
*   **Error Response (404 Not Found):** `ApiResponse` (If the group with the given ID is not found)
*   **Error Response (500 Internal Server Error):** `ApiResponse`

---
