# Check Mate — Copilot Instructions

## Solution Overview
Check Mate is a to-do style application. The solution file is `CheckMate.slnx`.

### Projects
| Project | Type | Purpose |
|---|---|---|
| `CM.Domain` | Class library, `net10.0` | All domain entities, CQRS interfaces, commands, events, queries, and result types |
| `CM.LiteDB` | Class library, `net10.0` | LiteDB persistence implementations of CQRS handlers |
| `CM.Bogus` | Class library, `net10.0` | Deterministic Bogus-generated seed data for development and testing |
| `CM.Domain.Tests` | xUnit test project, `net10.0` | Unit tests for CM.Domain handlers and CQRS infrastructure |
| `CM.Bogus.Tests` | xUnit test project, `net10.0` | Unit tests for CM.Bogus data services and seed data |
| `CM.LiteDB.Tests` | xUnit test project, `net10.0` | Unit tests for CM.LiteDB data services |

Project names follow the `CM.*` prefix convention. Projects are organised into sub-folders:
- `Domain/CM.Domain` — domain layer
- `Library/CM.Bogus` — Bogus seed data library
- `Library/CM.LiteDB` — LiteDB persistence library
- `Tests/CM.Domain.Tests` — domain unit tests
- `Tests/CM.Bogus.Tests` — Bogus data service unit tests
- `Tests/CM.LiteDB.Tests` — LiteDB data service unit tests

---

## Code Conventions

### Language and Framework
- C# 14, .NET 10
- `Nullable`, `ImplicitUsings`, and `LangVersion` are set in `Directory.Build.props` — never add them to `.csproj` files

### Naming
- Follow Microsoft naming conventions throughout
- Acronyms of 3+ letters use Pascal casing: `Cqrs` not `CQRS`, `Xml` not `XML`
- Two-letter acronyms remain uppercase: `Id` follows .NET convention
- No abbreviations except well-accepted ones (`Id`, `Dto`)
- Project names use the `CM.*` prefix (`CM.Domain`, etc.)

### Classes
- All classes must be `sealed`
- Member ordering within a class: private fields → constructors → properties → methods
- Within each group, members are ordered **alphabetically**
- All public types and members must have XML documentation (`///` comments)

### Records (Commands, Events, Queries, Results)
- Use **positional record syntax**: `public record Foo(Type Param) : IInterface;`
- Parameters are ordered **alphabetically**
- `///` `<param>` docs go on the record declaration line
- No `using` directives inside type files — use `_GlobalUsings.cs` only

### Global Usings
- Every project has a `_GlobalUsings.cs` containing all `global using` directives for the project
- No `using` directives in individual type files

---

## CM.Domain Structure

```
Domain/CM.Domain/
  _GlobalUsings.cs
  Extensions.cs        — static class; AddDefaultHandlers() extension on IServiceCollection
  Cqrs/
    CommandFailed.cs   — record; IEvent raised when a command fails; (Guid? CommandId, string Message)
    FailResult.cs      — sealed class; failure carrier with Message property
    ICommand.cs        — interface; Guid Id { get; }
    ICommandHandler.cs — interface; Task HandleAsync(TCommand, CancellationToken)
    IEvent.cs          — interface; Guid? CommandId { get; }
    IMessenger.cs      — interface; Task SendAsync(TMessage, CancellationToken)
    IQuery.cs          — interface; pure marker generic interface IQuery<TResult>, no properties
    IQueryHandler.cs   — interface; Task<Result<TResult>> HandleAsync(TQuery, CancellationToken)
    IValidatable.cs    — interface; FailResult? Validate()
    Result.cs          — sealed class; generic discriminated union of T | FailResult
  Checkables/
    Checkable.cs             — sealed entity class
    ICheckableDataService.cs — interface; DeleteAsync, GetByCheckListAsync, GetByIdAsync, UpsertAsync
    Commands/
      CheckCheckable.cs
      CheckCheckableHandler.cs
      CreateCheckable.cs       — implements IValidatable
      CreateCheckableHandler.cs
      DeleteCheckable.cs
      DeleteCheckableHandler.cs
      UncheckCheckable.cs
      UncheckCheckableHandler.cs
      UpdateCheckable.cs       — implements IValidatable
      UpdateCheckableHandler.cs
    Events/
      CheckableChecked.cs
      CheckableCreated.cs
      CheckableDeleted.cs
      CheckableUnchecked.cs
      CheckableUpdated.cs
    Queries/
      CheckableDto.cs                    — has secondary ctor: CheckableDto(Checkable)
      GetCheckablesByCheckList.cs
      GetCheckablesByCheckListHandler.cs
  CheckLists/
    CheckList.cs             — sealed entity class
    ICheckListDataService.cs — interface; DeleteAsync, GetByIdAsync, GetByUserAsync, UpsertAsync
    Commands/
      CreateCheckList.cs       — implements IValidatable
      CreateCheckListHandler.cs
      DeleteCheckList.cs
      DeleteCheckListHandler.cs
      UpdateCheckList.cs       — implements IValidatable
      UpdateCheckListHandler.cs
    Events/
      CheckListCreated.cs
      CheckListDeleted.cs
      CheckListUpdated.cs
    Queries/
      CheckListDto.cs              — has secondary ctor: CheckListDto(CheckList)
      GetCheckList.cs
      GetCheckListHandler.cs
      GetCheckListsByUser.cs
      GetCheckListsByUserHandler.cs
  Users/
    User.cs             — sealed entity class
    IUserDataService.cs — interface; ExistsByEmailAsync, GetByIdAsync, UpsertAsync
    Commands/
      CreateUser.cs       — implements IValidatable; validates Email (required + format) then Name (required)
      CreateUserHandler.cs
      UpdateUser.cs       — implements IValidatable
      UpdateUserHandler.cs
    Events/
      UserCreated.cs
      UserUpdated.cs
    Queries/
      GetUser.cs
      GetUserHandler.cs
      GetUserEmailExists.cs
      GetUserEmailExistsHandler.cs
      UserDto.cs          — has secondary ctor: UserDto(User)
```

---

## CM.Bogus Structure

```
Library/CM.Bogus/
  _GlobalUsings.cs
  DataSeed.cs          — static class; deterministic seed data via Bogus
  DataStore.cs         — internal static class; loads seed data into mutable lists; exposes Lock SyncRoot
  Extensions.cs        — static class; AddBogusDataServices() extension on IServiceCollection
  Checkables/
    CheckableDataService.cs  — internal sealed; implements ICheckableDataService
  CheckLists/
    CheckListDataService.cs  — internal sealed; implements ICheckListDataService
  Users/
    UserDataService.cs       — internal sealed; implements IUserDataService
```

### DataSeed
- `static class DataSeed` — all data generated once in the static constructor using seed `20250417`
- Three public static methods: `GetUsers()`, `GetCheckLists()`, `GetCheckables()` — each returns `IReadOnlyList<T>`
- Seeded user: Bilbo Baggins (`bilbo.baggins@shire.me`), fixed `Id` of `00000000-0000-0000-0000-000000000001`
- Generates 10 `CheckList` instances, each with 3–20 `Checkable` items
- `Checkable.CheckListId` is always correctly set to its parent `CheckList.Id`

### DataStore
- `internal static class DataStore` — loads seed data from `DataSeed` into private `List<T>` fields in its static constructor
- Exposes `Checkables`, `CheckLists`, `Users` as `internal static List<T>` properties
- Exposes `internal static Lock SyncRoot` for thread-safe access
- Data service implementations use `using var scope = DataStore.SyncRoot.EnterScope()` for locking

### Bogus Data Services
- Each `XxxDataService` is `internal sealed`, lives in `CM.Bogus/Xxx/`
- Implements the corresponding `IXxxDataService` from `CM.Domain` using `DataStore` as the backing store
- Uses `Task.FromResult` (synchronous operations wrapped as tasks)
- `AddBogusDataServices(this IServiceCollection)` in `Extensions.cs` registers all three as scoped

---

## CM.LiteDB Structure

```
Library/CM.LiteDB/
  _GlobalUsings.cs
  Extensions.cs        — static class; AddLiteDbDataServices() extension on IServiceCollection
  Checkables/
    CheckableDataService.cs  — internal sealed; implements ICheckableDataService
  CheckLists/
    CheckListDataService.cs  — internal sealed; implements ICheckListDataService
  Users/
    UserDataService.cs       — internal sealed; implements IUserDataService
```

### LiteDB Data Services
- Each `XxxDataService` is `internal sealed`, lives in `CM.LiteDB/Xxx/`
- Implements the corresponding `IXxxDataService` from `CM.Domain` using `ILiteDatabase`
- `ILiteDatabase` is injected via constructor, null-guarded
- Entities are mapped to/from `BsonDocument` explicitly — no auto-mapper
- `DateTimeOffset` stored as ISO 8601 string (`"O"` format)
- Collections and indexing:
  - `User` → collection `"users"`, keyed by `Id`, indexed on `email`
  - `CheckList` → collection `"checklists"`, keyed by `Id`, indexed on `userId`
  - `Checkable` → collection `"checkables"`, keyed by `Id`, indexed on `checkListId`
- `AddLiteDbDataServices(this IServiceCollection)` in `Extensions.cs` registers all three as scoped

---

## Tests Structure

```
Tests/CM.Domain.Tests/
  _GlobalUsings.cs
  CM.Domain.Tests.csproj
  Checkables/
    Commands/
      CheckCheckableHandlerTests.cs
      CreateCheckableHandlerTests.cs
      DeleteCheckableHandlerTests.cs
      UncheckCheckableHandlerTests.cs
      UpdateCheckableHandlerTests.cs
    Queries/
      GetCheckablesByCheckListHandlerTests.cs
  CheckLists/
    Commands/
      CreateCheckListHandlerTests.cs
      DeleteCheckListHandlerTests.cs
      UpdateCheckListHandlerTests.cs
    Queries/
      GetCheckListHandlerTests.cs
      GetCheckListsByUserHandlerTests.cs
  Cqrs/
    ResultTests.cs
  Users/
    Commands/
      CreateUserHandlerTests.cs
      UpdateUserHandlerTests.cs
    Queries/
      GetUserEmailExistsHandlerTests.cs
      GetUserHandlerTests.cs

Tests/CM.Bogus.Tests/
  _GlobalUsings.cs
  CM.Bogus.Tests.csproj
  DataSeedTests.cs
  ExtensionsTests.cs
  Checkables/
    CheckableDataServiceTests.cs
  CheckLists/
    CheckListDataServiceTests.cs
  Users/
    UserDataServiceTests.cs
    UserDataServiceExistsByEmailTests.cs

Tests/CM.LiteDB.Tests/
  _GlobalUsings.cs
  CM.LiteDB.Tests.csproj
  ExtensionsTests.cs
  Checkables/
    CheckableDataServiceTests.cs
  CheckLists/
    CheckListDataServiceTests.cs
  Users/
    UserDataServiceTests.cs
    UserDataServiceExistsByEmailTests.cs
```

### Test Conventions
- Test projects use **xUnit**, **NSubstitute**, and **FluentAssertions**
- Test class names: `XxxTests` (sealed)
- Test method naming: `MethodName_Returns_Xxx_When_Some_Condition` (snake_case segments, PascalCase words)
- Each test project has `_GlobalUsings.cs` declaring all necessary global usings including `FluentAssertions`, `NSubstitute`, and `Xunit`
- No `using` directives in individual test files
- Test projects reference `Microsoft.Extensions.DependencyInjection` (full package, not just Abstractions) where DI container is needed
- LiteDB test classes implement `IDisposable` to dispose `LiteDatabase` and `ServiceProvider`

### Test Patterns
- **Constructor null guard tests**: one test per nullable constructor parameter using `WithParameterName`
- **Validation tests**: `[Theory]` with `[InlineData]` for each invalid input variant
- **Handler error path tests**: mock data service returns `new FailResult(...)`, assert `CommandFailed` is sent
- **Handler success path tests**: assert correct event type is sent with correct properties
- **State mutation tests**: assert the entity is mutated correctly before being upserted (e.g. `Checked = true`)
- **Data service integration tests** (Bogus/LiteDB): use real service instances; Bogus uses `AddBogusDataServices()` + DI; LiteDB uses `new LiteDatabase(":memory:")` + DI
- NSubstitute substitutes are declared as fields: `IXxx _dep = Substitute.For<IXxx>()`
- The system under test (`sut`) is created inline in each test method, never in a constructor or fixture

---

## Domain Entities

### `User` (sealed)
| Property | Type | Mutability |
|---|---|---|
| `Created` | `DateTimeOffset` | `init` |
| `Email` | `string` | `init` |
| `Id` | `Guid` | `init` = `Guid.CreateVersion7()` |
| `Name` | `string` | `get; set;` |

### `CheckList` (sealed)
| Property | Type | Mutability |
|---|---|---|
| `Created` | `DateTimeOffset` | `init` |
| `Id` | `Guid` | `init` = `Guid.CreateVersion7()` |
| `Name` | `string` | `get; set;` |
| `UserId` | `Guid` | `init` |

### `Checkable` (sealed)
| Property | Type | Mutability |
|---|---|---|
| `Checked` | `bool` | `get; set;` |
| `CheckListId` | `Guid` | `init` |
| `Created` | `DateTimeOffset` | `init` |
| `Description` | `string` | `get; set;` |
| `Id` | `Guid` | `init` = `Guid.CreateVersion7()` |
| `UserId` | `Guid` | `init` |

---

## CQRS Patterns

### Commands
- Named as the action, no "Command" suffix: `CreateCheckable` not `CreateCheckableCommand`
- Positional records implementing `ICommand`
- All have `public Guid Id { get; init; } = Guid.CreateVersion7();` as an explicit body property (not in the positional params) — auto-generated, overridable for deserialization rehydration
- All other properties in positional params, alphabetically ordered
- Commands with string properties implement `IValidatable` and define `Validate()` inline on the record
- `Validate()` returns `null` when valid, or a `FailResult` describing the first failure
- Email fields are validated as required first, then format-checked using `System.Net.Mail.MailAddress.TryCreate`

### Events
- Named as past tense of the command: `CheckableCreated` for `CreateCheckable`
- Positional records implementing `IEvent`
- **Non-delete events**: `(XxxDto Xxx, Guid? CommandId)` — only the Dto and CommandId; all other data is accessible via the Dto
- **Delete events**: `(Guid XxxId, Guid? CommandId)` — no Dto (entity no longer exists)
- `CommandId` associates the event back to its originating command

### Queries
- Positional records implementing `IQuery<TResult>`
- Single-item queries implement `IQuery<TDto?>` (nullable) — caller handles not-found
- Collection queries implement `IQuery<IReadOnlyList<TDto>>`
- Scalar queries implement `IQuery<T>` directly (e.g. `IQuery<bool>`)
- `IQueryHandler.HandleAsync` wraps the result: returns `Task<Result<TResult>>`

### DTOs (Query Results)
- Named `XxxDto`, live in `Xxx/Queries/` folder
- Positional records, params alphabetically ordered
- Mirror the entity shape
- All have a secondary constructor `XxxDto(XxxEntity entity)` that chains to the primary via `this(...)`

### Result<T>
- Discriminated union: `T` (success) or `FailResult` (failure)
- Use implicit operators to construct: `return new FailResult("msg")` or `return someValue`
- When the implicit operator cannot apply (e.g. `T` is an interface), use the static factory methods: `Result<T>.Success(value)` or `Result<T>.Fail(error)`
- Access via `result.IsSuccess`, `result.Value`, `result.IsError`, `result.Error`

### Command Handlers
- Named `XxxHandler`, sealed classes in the same folder as the command
- Implement `ICommandHandler<TCommand>`
- Constructor dependencies (alphabetically ordered): `IXxxDataService`, `IMessenger<IEvent>` — both null-guarded
- `HandleAsync` pattern:
  1. Call `command.Validate()` if the command implements `IValidatable` — send `CommandFailed` and return if non-null
  2. Call data service method(s)
  3. On error: send `CommandFailed(command.Id, result.Error.Message)` and return
  4. On success: send the appropriate domain event
- Never define intermediate variables just to shorten code — use `result.Value` inline
- Always wrap `if` bodies in curly braces even for single statements

### Query Handlers
- Named `XxxHandler`, sealed classes in the same folder as the query
- Implement `IQueryHandler<TQuery, TResult>`
- Constructor dependency: `IXxxDataService` — null-guarded
- Return `result.Error` on failure (implicit operator applies)
- For collection results where `T` is an interface, use `Result<T>.Success(...)` — map entities to DTOs using the entity constructor

### Data Service Interfaces
- Named `IXxxDataService`, live in `CM.Domain/Xxx/` alongside the entity
- Define the persistence contract for a single entity type
- `ICheckableDataService`: `DeleteAsync(Guid)`, `GetByCheckListAsync(Guid)`, `GetByIdAsync(Guid)`, `UpsertAsync(Checkable)`
- `ICheckListDataService`: `DeleteAsync(Guid)`, `GetByIdAsync(Guid)`, `GetByUserAsync(Guid)`, `UpsertAsync(CheckList)`
- `IUserDataService`: `ExistsByEmailAsync(string)`, `GetByIdAsync(Guid)`, `UpsertAsync(User)`
- All methods return `Task<Result<T>>`
- `ExistsByEmailAsync` is case-insensitive in both implementations

### Messaging
- `IMessenger<TMessage>` — single method: `Task SendAsync(TMessage, CancellationToken)`
- Handlers depend on `IMessenger<IEvent>` to publish domain events
- `CommandFailed(Guid? CommandId, string Message)` — cross-cutting event sent by any handler on failure or validation error

### DI Registration
- `Extensions.cs` — static class in each project's root namespace
- `CM.Domain`: `AddDefaultHandlers(this IServiceCollection)` registers all command and query handlers as scoped
- `CM.Bogus`: `AddBogusDataServices(this IServiceCollection)` registers the three Bogus data services as scoped
- `CM.LiteDB`: `AddLiteDbDataServices(this IServiceCollection)` registers the three LiteDB data services as scoped
- All require `Microsoft.Extensions.DependencyInjection.Abstractions` package
- Typical setup: call `AddDefaultHandlers()` plus one of `AddBogusDataServices()` or `AddLiteDbDataServices()`

---

## Relationships
- All `CheckList` instances belong to one `User` (via `UserId`)
- All `Checkable` instances belong to one `CheckList` (via `CheckListId`)


## Solution Overview
Check Mate is a to-do style application. The solution file is `CheckMate.slnx`.

### Projects
| Project | Type | Purpose |
|---|---|---|
| `CM.Domain` | Class library, `net10.0` | All domain entities, CQRS interfaces, commands, events, queries, and result types |
| `CM.LiteDB` | Class library, `net10.0` | LiteDB persistence implementations of CQRS handlers |
| `CM.Bogus` | Class library, `net10.0` | Deterministic Bogus-generated seed data for development and testing |

Project names follow the `CM.*` prefix convention. Projects are organised into sub-folders:
- `Domain/CM.Domain` — domain layer
- `Library/CM.Bogus` — Bogus seed data library
- `Library/CM.LiteDB` — LiteDB persistence library

---

## Code Conventions

### Language and Framework
- C# 14, .NET 10
- `Nullable`, `ImplicitUsings`, and `LangVersion` are set in `Directory.Build.props` — never add them to `.csproj` files

### Naming
- Follow Microsoft naming conventions throughout
- Acronyms of 3+ letters use Pascal casing: `Cqrs` not `CQRS`, `Xml` not `XML`
- Two-letter acronyms remain uppercase: `Id` follows .NET convention
- No abbreviations except well-accepted ones (`Id`, `Dto`)
- Project names use the `CM.*` prefix (`CM.Domain`, etc.)

### Classes
- All classes must be `sealed`
- Member ordering within a class: private fields → constructors → properties → methods
- Within each group, members are ordered **alphabetically**
- All public types and members must have XML documentation (`///` comments)

### Records (Commands, Events, Queries, Results)
- Use **positional record syntax**: `public record Foo(Type Param) : IInterface;`
- Parameters are ordered **alphabetically**
- `///` `<param>` docs go on the record declaration line
- No `using` directives inside type files — use `_GlobalUsings.cs` only

### Global Usings
- Every project has a `_GlobalUsings.cs` containing all `global using` directives for the project
- No `using` directives in individual type files

---

## CM.Domain Structure

```
Domain/CM.Domain/
  _GlobalUsings.cs
  Extensions.cs        — static class; AddDefaultHandlers() extension on IServiceCollection
  Cqrs/
    CommandFailed.cs   — record; IEvent raised when a command fails; (Guid? CommandId, string Message)
    FailResult.cs      — sealed class; failure carrier with Message property
    ICommand.cs        — interface; Guid Id { get; }
    ICommandHandler.cs — interface; Task HandleAsync(TCommand, CancellationToken)
    IEvent.cs          — interface; Guid? CommandId { get; }
    IMessenger.cs      — interface; Task SendAsync(TMessage, CancellationToken)
    IQuery.cs          — interface; pure marker generic interface IQuery<TResult>, no properties
    IQueryHandler.cs   — interface; Task<Result<TResult>> HandleAsync(TQuery, CancellationToken)
    IValidatable.cs    — interface; FailResult? Validate()
    Result.cs          — sealed class; generic discriminated union of T | FailResult
  Checkables/
    Checkable.cs             — sealed entity class
    ICheckableDataService.cs — interface; DeleteAsync, GetByCheckListAsync, GetByIdAsync, UpsertAsync
    Commands/
      CheckCheckable.cs
      CheckCheckableHandler.cs
      CreateCheckable.cs       — implements IValidatable
      CreateCheckableHandler.cs
      DeleteCheckable.cs
      DeleteCheckableHandler.cs
      UncheckCheckable.cs
      UncheckCheckableHandler.cs
      UpdateCheckable.cs       — implements IValidatable
      UpdateCheckableHandler.cs
    Events/
      CheckableChecked.cs
      CheckableCreated.cs
      CheckableDeleted.cs
      CheckableUnchecked.cs
      CheckableUpdated.cs
    Queries/
      CheckableDto.cs                    — has secondary ctor: CheckableDto(Checkable)
      GetCheckablesByCheckList.cs
      GetCheckablesByCheckListHandler.cs
  CheckLists/
    CheckList.cs             — sealed entity class
    ICheckListDataService.cs — interface; DeleteAsync, GetByIdAsync, GetByUserAsync, UpsertAsync
    Commands/
      CreateCheckList.cs       — implements IValidatable
      CreateCheckListHandler.cs
      DeleteCheckList.cs
      DeleteCheckListHandler.cs
      UpdateCheckList.cs       — implements IValidatable
      UpdateCheckListHandler.cs
    Events/
      CheckListCreated.cs
      CheckListDeleted.cs
      CheckListUpdated.cs
    Queries/
      CheckListDto.cs              — has secondary ctor: CheckListDto(CheckList)
      GetCheckList.cs
      GetCheckListHandler.cs
      GetCheckListsByUser.cs
      GetCheckListsByUserHandler.cs
  Users/
    User.cs             — sealed entity class
    IUserDataService.cs — interface; GetByIdAsync, UpsertAsync
    Commands/
      CreateUser.cs       — implements IValidatable
      CreateUserHandler.cs
      UpdateUser.cs       — implements IValidatable
      UpdateUserHandler.cs
    Events/
      UserCreated.cs
      UserUpdated.cs
    Queries/
      GetUser.cs
      GetUserHandler.cs
      UserDto.cs          — has secondary ctor: UserDto(User)
```

---

## CM.Bogus Structure

```
Library/CM.Bogus/
  _GlobalUsings.cs
  DataSeed.cs          — static class; deterministic seed data via Bogus
  DataStore.cs         — internal static class; loads seed data into mutable lists; exposes Lock SyncRoot
  Extensions.cs        — static class; AddBogusDataServices() extension on IServiceCollection
  Checkables/
    CheckableDataService.cs  — internal sealed; implements ICheckableDataService
  CheckLists/
    CheckListDataService.cs  — internal sealed; implements ICheckListDataService
  Users/
    UserDataService.cs       — internal sealed; implements IUserDataService
```

### DataSeed
- `static class DataSeed` — all data generated once in the static constructor using seed `20250417`
- Three public static methods: `GetUsers()`, `GetCheckLists()`, `GetCheckables()` — each returns `IReadOnlyList<T>`
- Seeded user: Bilbo Baggins (`bilbo.baggins@shire.me`), fixed `Id` of `00000000-0000-0000-0000-000000000001`
- Generates 10 `CheckList` instances, each with 3–20 `Checkable` items
- `Checkable.CheckListId` is always correctly set to its parent `CheckList.Id`

### DataStore
- `internal static class DataStore` — loads seed data from `DataSeed` into private `List<T>` fields in its static constructor
- Exposes `Checkables`, `CheckLists`, `Users` as `internal static List<T>` properties
- Exposes `internal static Lock SyncRoot` for thread-safe access
- Data service implementations use `using var scope = DataStore.SyncRoot.EnterScope()` for locking

### Bogus Data Services
- Each `XxxDataService` is `internal sealed`, lives in `CM.Bogus/Xxx/`
- Implements the corresponding `IXxxDataService` from `CM.Domain` using `DataStore` as the backing store
- Uses `Task.FromResult` (synchronous operations wrapped as tasks)
- `AddBogusDataServices(this IServiceCollection)` in `Extensions.cs` registers all three as scoped

---

## CM.LiteDB Structure

```
Library/CM.LiteDB/
  _GlobalUsings.cs
  Extensions.cs        — static class; AddLiteDbDataServices() extension on IServiceCollection
  Checkables/
    CheckableDataService.cs  — internal sealed; implements ICheckableDataService
  CheckLists/
    CheckListDataService.cs  — internal sealed; implements ICheckListDataService
  Users/
    UserDataService.cs       — internal sealed; implements IUserDataService
```

### LiteDB Data Services
- Each `XxxDataService` is `internal sealed`, lives in `CM.LiteDB/Xxx/`
- Implements the corresponding `IXxxDataService` from `CM.Domain` using `ILiteDatabase`
- `ILiteDatabase` is injected via constructor, null-guarded
- Entities are mapped to/from `BsonDocument` explicitly — no auto-mapper
- `DateTimeOffset` stored as ISO 8601 string (`"O"` format)
- Collections and indexing:
  - `User` → collection `"users"`, keyed by `Id`
  - `CheckList` → collection `"checklists"`, keyed by `Id`, indexed on `userId`
  - `Checkable` → collection `"checkables"`, keyed by `Id`, indexed on `checkListId`
- `AddLiteDbDataServices(this IServiceCollection)` in `Extensions.cs` registers all three as scoped

---

## Domain Entities

### `User` (sealed)
| Property | Type | Mutability |
|---|---|---|
| `Created` | `DateTimeOffset` | `init` |
| `Email` | `string` | `init` |
| `Id` | `Guid` | `init` = `Guid.CreateVersion7()` |
| `Name` | `string` | `get; set;` |

### `CheckList` (sealed)
| Property | Type | Mutability |
|---|---|---|
| `Created` | `DateTimeOffset` | `init` |
| `Id` | `Guid` | `init` = `Guid.CreateVersion7()` |
| `Name` | `string` | `get; set;` |
| `UserId` | `Guid` | `init` |

### `Checkable` (sealed)
| Property | Type | Mutability |
|---|---|---|
| `Checked` | `bool` | `get; set;` |
| `CheckListId` | `Guid` | `init` |
| `Created` | `DateTimeOffset` | `init` |
| `Description` | `string` | `get; set;` |
| `Id` | `Guid` | `init` = `Guid.CreateVersion7()` |
| `UserId` | `Guid` | `init` |

---

## CQRS Patterns

### Commands
- Named as the action, no "Command" suffix: `CreateCheckable` not `CreateCheckableCommand`
- Positional records implementing `ICommand`
- All have `public Guid Id { get; init; } = Guid.CreateVersion7();` as an explicit body property (not in the positional params) — auto-generated, overridable for deserialization rehydration
- All other properties in positional params, alphabetically ordered
- Commands with string properties implement `IValidatable` and define `Validate()` inline on the record
- `Validate()` returns `null` when valid, or a `FailResult` describing the first failure

### Events
- Named as past tense of the command: `CheckableCreated` for `CreateCheckable`
- Positional records implementing `IEvent`
- **Non-delete events**: `(XxxDto Xxx, Guid? CommandId)` — only the Dto and CommandId; all other data is accessible via the Dto
- **Delete events**: `(Guid XxxId, Guid? CommandId)` — no Dto (entity no longer exists)
- `CommandId` associates the event back to its originating command

### Queries
- Positional records implementing `IQuery<TResult>`
- Single-item queries implement `IQuery<TDto?>` (nullable) — caller handles not-found
- Collection queries implement `IQuery<IReadOnlyList<TDto>>`
- `IQueryHandler.HandleAsync` wraps the result: returns `Task<Result<TResult>>`

### DTOs (Query Results)
- Named `XxxDto`, live in `Xxx/Queries/` folder
- Positional records, params alphabetically ordered
- Mirror the entity shape
- All have a secondary constructor `XxxDto(XxxEntity entity)` that chains to the primary via `this(...)`

### Result<T>
- Discriminated union: `T` (success) or `FailResult` (failure)
- Use implicit operators to construct: `return new FailResult("msg")` or `return someValue`
- When the implicit operator cannot apply (e.g. `T` is an interface), use the static factory methods: `Result<T>.Success(value)` or `Result<T>.Fail(error)`
- Access via `result.IsSuccess`, `result.Value`, `result.IsError`, `result.Error`

### Command Handlers
- Named `XxxHandler`, sealed classes in the same folder as the command
- Implement `ICommandHandler<TCommand>`
- Constructor dependencies (alphabetically ordered): `IXxxDataService`, `IMessenger<IEvent>` — both null-guarded
- `HandleAsync` pattern:
  1. Call `command.Validate()` if the command implements `IValidatable` — send `CommandFailed` and return if non-null
  2. Call data service method(s)
  3. On error: send `CommandFailed(command.Id, result.Error.Message)` and return
  4. On success: send the appropriate domain event
- Never define intermediate variables just to shorten code — use `result.Value` inline
- Always wrap `if` bodies in curly braces even for single statements

### Query Handlers
- Named `XxxHandler`, sealed classes in the same folder as the query
- Implement `IQueryHandler<TQuery, TResult>`
- Constructor dependency: `IXxxDataService` — null-guarded
- Return `result.Error` on failure (implicit operator applies)
- For collection results where `T` is an interface, use `Result<T>.Success(...)` — map entities to DTOs using the entity constructor

### Data Service Interfaces
- Named `IXxxDataService`, live in `CM.Domain/Xxx/` alongside the entity
- Define the persistence contract for a single entity type
- `ICheckableDataService`: `DeleteAsync(Guid)`, `GetByCheckListAsync(Guid)`, `GetByIdAsync(Guid)`, `UpsertAsync(Checkable)`
- `ICheckListDataService`: `DeleteAsync(Guid)`, `GetByIdAsync(Guid)`, `GetByUserAsync(Guid)`, `UpsertAsync(CheckList)`
- `IUserDataService`: `GetByIdAsync(Guid)`, `UpsertAsync(User)`
- All methods return `Task<Result<T>>`

### Messaging
- `IMessenger<TMessage>` — single method: `Task SendAsync(TMessage, CancellationToken)`
- Handlers depend on `IMessenger<IEvent>` to publish domain events
- `CommandFailed(Guid? CommandId, string Message)` — cross-cutting event sent by any handler on failure or validation error

### DI Registration
- `Extensions.cs` — static class in each project's root namespace
- `CM.Domain`: `AddDefaultHandlers(this IServiceCollection)` registers all command and query handlers as scoped
- `CM.Bogus`: `AddBogusDataServices(this IServiceCollection)` registers the three Bogus data services as scoped
- `CM.LiteDB`: `AddLiteDbDataServices(this IServiceCollection)` registers the three LiteDB data services as scoped
- All require `Microsoft.Extensions.DependencyInjection.Abstractions` package
- Typical setup: call `AddDefaultHandlers()` plus one of `AddBogusDataServices()` or `AddLiteDbDataServices()`

---

## Relationships
- All `CheckList` instances belong to one `User` (via `UserId`)
- All `Checkable` instances belong to one `CheckList` (via `CheckListId`)
