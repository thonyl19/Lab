# GraphQL Development Guide

This guide provides comprehensive information for developers working with the GraphQL API.

## Table of Contents

1. [Development Setup](#development-setup)
2. [Code Structure](#code-structure)
3. [Type Definitions](#type-definitions)
4. [Resolvers](#resolvers)
5. [Testing](#testing)
6. [Debugging](#debugging)
7. [Contributing](#contributing)

## Development Setup

### Prerequisites

- .NET 6 SDK
- Visual Studio 2022 or VS Code
- Git
- SQLite

### Local Development

1. Clone the repository
2. Install dependencies
3. Configure environment
4. Run the application

```bash
# Clone repository
git clone [repository-url]

# Install dependencies
dotnet restore

# Run the application
dotnet run
```

## Code Structure

### Directory Organization

```
src/DTC.API/
├── GraphQL/
│   ├── Types/           # GraphQL type definitions
│   │   ├── TodoType.cs
│   │   └── InputTypes.cs
│   ├── Queries/         # Query definitions
│   │   └── TodoQuery.cs
│   ├── Mutations/       # Mutation definitions
│   │   └── TodoMutation.cs
│   └── Subscriptions/   # Subscription definitions
├── Models/              # Data models
└── Data/               # Data access layer
```

### Naming Conventions

- Types: `[Name]Type.cs`
- Queries: `[Name]Query.cs`
- Mutations: `[Name]Mutation.cs`
- Input Types: `[Name]Input.cs`

## Type Definitions

### Basic Type

```csharp
[GraphQLName("Todo")]
[GraphQLDescription("A todo item")]
public class TodoType
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}
```

### Input Type

```csharp
[GraphQLName("CreateTodoInput")]
[GraphQLDescription("Input for creating a new todo")]
public class CreateTodoInput
{
    [GraphQLDescription("The title of the todo")]
    public string Title { get; set; }
}
```

## Resolvers

### Query Resolver

```csharp
[UseDbContext(typeof(ApplicationDbContext))]
[UsePaging]
[UseFiltering]
[UseSorting]
public IQueryable<Todo> GetTodos([Service] ApplicationDbContext context)
{
    return context.Todos;
}
```

### Mutation Resolver

```csharp
[UseDbContext(typeof(ApplicationDbContext))]
public async Task<Todo> CreateTodo(
    [Service] ApplicationDbContext context,
    CreateTodoInput input)
{
    var todo = new Todo
    {
        Title = input.Title,
        IsCompleted = false
    };
    context.Todos.Add(todo);
    await context.SaveChangesAsync();
    return todo;
}
```

## Testing

### Unit Tests

```csharp
public class TodoQueryTests
{
    [Fact]
    public async Task GetTodos_ReturnsAllTodos()
    {
        // Arrange
        var context = CreateTestContext();
        var query = new TodoQuery();

        // Act
        var result = await query.GetTodos(context);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
```

### Integration Tests

```csharp
public class GraphQLIntegrationTests
{
    [Fact]
    public async Task CreateTodo_ReturnsNewTodo()
    {
        // Arrange
        var client = CreateTestClient();
        var mutation = @"
            mutation {
                createTodo(input: { title: ""Test Todo"" }) {
                    id
                    title
                    isCompleted
                }
            }
        ";

        // Act
        var response = await client.PostAsync("/graphql", 
            new StringContent(mutation, Encoding.UTF8, "application/json"));

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
}
```

## Debugging

### GraphQL IDE

Use the built-in GraphQL IDE (Banana) for testing queries and mutations:

```
https://localhost:5001/graphql
```

### Logging

```csharp
public class GraphQLErrorFilter : IErrorFilter
{
    private readonly ILogger<GraphQLErrorFilter> _logger;

    public GraphQLErrorFilter(ILogger<GraphQLErrorFilter> logger)
    {
        _logger = logger;
    }

    public IError OnError(IError error)
    {
        _logger.LogError(error.Exception, "GraphQL Error: {Message}", error.Message);
        return error;
    }
}
```

## Contributing

### Code Style

- Follow C# coding conventions
- Use meaningful names
- Add XML documentation
- Write unit tests

### Pull Request Process

1. Create a feature branch
2. Make your changes
3. Write/update tests
4. Update documentation
5. Submit PR

### Review Process

- Code review
- Test coverage
- Documentation
- Performance impact 