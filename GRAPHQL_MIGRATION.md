# GraphQL Migration Guide

## Overview
CineSocial API has been successfully migrated from REST to GraphQL using HotChocolate.

## Changes Made

### 1. Packages Added
- `HotChocolate.AspNetCore` (v15.1.11)
- `HotChocolate.Data.EntityFramework` (v15.1.11)

### 2. New GraphQL Structure

```
CineSocial.Api/
├── GraphQL/
│   ├── Queries/
│   │   ├── Query.cs
│   │   ├── UserQueries.cs
│   │   ├── MovieQueries.cs
│   │   ├── FollowQueries.cs
│   │   └── BlockQueries.cs
│   ├── Mutations/
│   │   ├── Mutation.cs
│   │   ├── AuthMutations.cs
│   │   ├── UserMutations.cs
│   │   ├── FollowMutations.cs
│   │   └── BlockMutations.cs
│   └── GraphQLUserContextAccessor.cs
└── Controllers_OLD/ (backed up REST controllers)
```

### 3. GraphQL Endpoint
- **Endpoint**: `https://localhost:PORT/graphql`
- **GraphQL IDE**: Banana Cake Pop (available in development mode)

### 4. Authentication
JWT authentication is fully integrated with GraphQL. Use the `Authorization` header:
```
Authorization: Bearer YOUR_JWT_TOKEN
```

## Available Queries

### User Queries
```graphql
query {
  getCurrentUser {
    id
    username
    email
    role
    bio
    createdAt
  }
}
```

### Movie Queries
```graphql
query {
  getMovies(page: 1, pageSize: 20, searchTerm: "matrix") {
    items {
      id
      title
      overview
      releaseDate
      voteAverage
    }
    totalCount
    pageNumber
    pageSize
  }
}

query {
  getMovieById(id: 1) {
    id
    title
    originalTitle
    overview
    releaseDate
    runtime
    genres
  }
}
```

### Follow Queries
```graphql
query {
  getFollowers(userId: 1) {
    id
    username
    followedAt
  }
}

query {
  getFollowing(userId: 1) {
    id
    username
    followedAt
  }
}
```

### Block Queries
```graphql
query {
  getBlockedUsers {
    id
    username
    blockedAt
  }
}
```

## Available Mutations

### Auth Mutations
```graphql
mutation {
  login(email: "user@example.com", password: "Password123!") {
    token
    userId
    username
    email
  }
}

mutation {
  register(
    username: "newuser"
    email: "newuser@example.com"
    password: "Password123!"
  ) {
    token
    userId
    username
    email
  }
}
```

### User Mutations
```graphql
mutation {
  updateProfile(
    userId: 1
    username: "newusername"
    bio: "My new bio"
    profileImageId: null
    backgroundImageId: null
  ) {
    userId
    message
  }
}
```

### Follow Mutations
```graphql
mutation {
  followUser(followingId: 2)
}

mutation {
  unfollowUser(followingId: 2)
}
```

### Block Mutations
```graphql
mutation {
  blockUser(blockedUserId: 3)
}

mutation {
  unblockUser(blockedUserId: 3)
}
```

## Running the Application

1. **Start the API**:
   ```bash
   cd D:\code\CineSocial\backend\src\CineSocial.Api
   dotnet run
   ```

2. **Access GraphQL IDE**:
   Open your browser and navigate to:
   ```
   https://localhost:<PORT>/graphql/
   ```

3. **Test Queries**:
   Use the built-in Banana Cake Pop IDE to test your queries and mutations.

## Migration Notes

- All existing REST controllers have been moved to `Controllers_OLD/` directory
- The REST API is no longer active
- All business logic remains unchanged (CQRS with MediatR)
- JWT authentication continues to work seamlessly
- Error handling is implemented using GraphQL exceptions

## CORS Configuration
CORS is configured to allow all origins for development. Update the CORS policy in production.

## Next Steps

1. Update frontend to use GraphQL queries
2. Remove `Controllers_OLD/` directory once frontend migration is complete
3. Consider adding GraphQL subscriptions for real-time features
4. Implement field-level authorization if needed
5. Add DataLoader for optimizing database queries (N+1 problem)
