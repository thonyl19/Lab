# GraphQL Configuration Guide

This document provides detailed information about configuring and customizing the GraphQL API.

## Table of Contents

1. [Basic Configuration](#basic-configuration)
2. [Advanced Settings](#advanced-settings)
3. [Security Configuration](#security-configuration)
4. [Performance Tuning](#performance-tuning)
5. [Troubleshooting](#troubleshooting)

## Basic Configuration

### Program.cs Setup

```csharp
// Add GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<TodoType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();

// Add GraphQL endpoint
app.MapGraphQL();
```

### Environment Variables

```json
{
  "GraphQL": {
    "Path": "/graphql",
    "EnablePlayground": true,
    "EnableSchema": true
  }
}
```

## Advanced Settings

### Custom Middleware

```csharp
app.UseGraphQL()
   .UseGraphQLPlayground()
   .UseGraphQLVoyager();
```

### Schema Configuration

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
    .AddGraphQLUpload()
    .AddInMemorySubscriptions();
```

## Security Configuration

### Authentication

```csharp
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();
```

### CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("GraphQLPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
```

## Performance Tuning

### DataLoader Configuration

```csharp
builder.Services
    .AddDataLoader<ITodoDataLoader>()
    .AddInMemoryCache();
```

### Query Complexity

```csharp
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<TodoType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddMaxExecutionDepth(10)
    .AddMaxComplexity(100);
```

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

## Best Practices

1. **Configuration Management**
   - Use environment-specific settings
   - Keep sensitive data in secure storage
   - Document all configuration changes

2. **Performance Optimization**
   - Implement proper caching
   - Use DataLoader for batch operations
   - Monitor query complexity

3. **Security Measures**
   - Implement proper authentication
   - Use HTTPS in production
   - Validate all inputs

4. **Monitoring and Logging**
   - Implement proper logging
   - Monitor performance metrics
   - Track error rates 