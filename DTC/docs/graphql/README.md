# GraphQL API Documentation

This document provides comprehensive documentation for the GraphQL API implementation in the DTC project.

## Table of Contents

1. [Overview](#overview)
2. [Getting Started](#getting-started)
3. [API Structure](#api-structure)
4. [Queries](#queries)
5. [Mutations](#mutations)
6. [Types](#types)
7. [Best Practices](#best-practices)
8. [Troubleshooting](#troubleshooting)
9. [References](#references)

## Overview

The GraphQL API is implemented using HotChocolate framework and coexists with the REST API. It provides a flexible and efficient way to query and manipulate data.

### Key Features

- Full CRUD operations for Todo items
- Pagination support
- Filtering and sorting capabilities
- Real-time error handling
- Type-safe operations
- File upload support
- In-memory subscriptions

## Getting Started

### Prerequisites

- .NET 6 SDK
- HotChocolate.AspNetCore (v13.5.1)
- HotChocolate.Data.EntityFramework (v13.5.1)
- Microsoft.EntityFrameworkCore.Sqlite (v6.0.0)

### Installation

```bash
# Install required packages
dotnet add package HotChocolate.AspNetCore --version 13.5.1
dotnet add package HotChocolate.Data.EntityFramework --version 13.5.1
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 6.0.0
```

### Running the API

```bash
# Restore packages
dotnet restore

# Run the application
dotnet run
```

Access the GraphQL IDE at:
- https://localhost:5001/graphql
- http://localhost:5000/graphql

## API Structure

```
src/DTC.API/
├── GraphQL/
│   ├── Types/           # GraphQL type definitions
│   ├── Queries/         # Query definitions
│   ├── Mutations/       # Mutation definitions
│   └── Subscriptions/   # Subscription definitions
├── Models/              # Data models (shared with REST API)
└── Data/               # Data access layer (shared with REST API)
```

## Queries

### Get All Todos

```graphql
query {
  todos {
    id
    title
    isCompleted
  }
}
```

### Get Todo by ID

```graphql
query {
  todoById(id: 1) {
    id
    title
    isCompleted
  }
}
```

## Mutations

### Create Todo

```graphql
mutation {
  createTodo(input: { title: "New Todo" }) {
    id
    title
    isCompleted
  }
}
```

### Update Todo

```graphql
mutation {
  updateTodo(input: { id: 1, title: "Updated Todo", isCompleted: true }) {
    id
    title
    isCompleted
  }
}
```

### Delete Todo

```graphql
mutation {
  deleteTodo(id: 1)
}
```

## Types

### Todo Type

```graphql
type Todo {
  id: ID!
  title: String!
  isCompleted: Boolean!
}
```

### Input Types

```graphql
input CreateTodoInput {
  title: String!
}

input UpdateTodoInput {
  id: ID!
  title: String
  isCompleted: Boolean
}
```

## Best Practices

1. **Error Handling**
   - Use GraphQLException for business logic errors
   - Implement proper validation
   - Return meaningful error messages

2. **Performance**
   - Use DataLoader for batch loading
   - Implement proper pagination
   - Optimize database queries

3. **Security**
   - Implement proper authentication
   - Validate input data
   - Use proper authorization

4. **Code Organization**
   - Follow the established directory structure
   - Use proper naming conventions
   - Document all types and operations

## Troubleshooting

### Common Issues

1. **Schema Generation Errors**
   - Check type definitions
   - Verify resolver implementations
   - Ensure proper dependency injection

2. **Performance Issues**
   - Monitor query complexity
   - Check database queries
   - Use DataLoader for batch operations

3. **CORS Issues**
   - Verify CORS configuration
   - Check request headers
   - Ensure proper middleware order

### Debug Configuration

```csharp
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<TodoType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
    .AddErrorFilter<GraphQLErrorFilter>();
```

## References

- [HotChocolate Documentation](https://chillicream.com/docs/hotchocolate)
- [GraphQL Specification](https://graphql.org/learn/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) 