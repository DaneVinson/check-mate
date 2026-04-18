# Check Mate — Copilot Instructions

## Solution Overview
Check Mate is a to-do style application. The solution file is `CheckMate.slnx`.

### Projects
| Project | Type | Purpose |
|---|---|---|
| `CM.Domain` | Class library, `net10.0` | All domain entities, CQRS interfaces, commands, events, queries, and result types |

Project names follow the `CM.*` prefix convention. No `src/` folder — projects sit at the repo root.

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
CM.Domain/
  _GlobalUsings.cs
  Cqrs/
    ErrorResult.cs       — sealed class; failure carrier with Message property
    ICommand.cs          — interface; Guid Id { get; }
    ICommandHandler.cs   — interface; Task HandleAsync(TCommand, CancellationToken)
    IEvent.cs            — interface; Guid? CommandId { get; }
    IQuery.cs            — interface; pure marker generic interface IQuery<TResult>, no properties
    IQueryHandler.cs     — interface; Task<Result<TResult>> HandleAsync(TQuery, CancellationToken)
    Result.cs            — sealed class; generic discriminated union of T | ErrorResult
  Checkables/
    Checkable.cs         — sealed entity class
    Commands/
      CheckCheckable.cs
      CreateCheckable.cs
      DeleteCheckable.cs
      UncheckCheckable.cs
      UpdateCheckable.cs
    Events/
      CheckableChecked.cs
      CheckableCreated.cs
      CheckableDeleted.cs
      CheckableUnchecked.cs
      CheckableUpdated.cs
    Queries/
      CheckableDto.cs
      GetCheckablesByCheckList.cs
  CheckLists/
    CheckList.cs         — sealed entity class
    Commands/
      CreateCheckList.cs
      DeleteCheckList.cs
      UpdateCheckList.cs
    Events/
      CheckListCreated.cs
      CheckListDeleted.cs
      CheckListUpdated.cs
    Queries/
      CheckListDto.cs
      GetCheckList.cs
      GetCheckListsByUser.cs
  Users/
    User.cs              — sealed entity class
    Commands/
      CreateUser.cs
      UpdateUser.cs
    Events/
      UserCreated.cs
      UserUpdated.cs
    Queries/
      GetUser.cs
      UserDto.cs
```

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

### Events
- Named as past tense of the command: `CheckableCreated` for `CreateCheckable`
- Positional records implementing `IEvent`
- **Non-delete events**: `(XxxDto Xxx, Guid? CommandId)` — only the Dto and CommandId; all other data is accessible via the Dto
- **Delete events**: `(Guid XxxId, Guid? CommandId)` — no Dto (entity no longer exists)
- `CommandId` associates the event back to its originating command

### Queries
- Positional records implementing `IQuery<TResult>`
- Single-item queries return `IQuery<TDto?>` (nullable) — caller handles not-found
- Collection queries return `IQuery<IReadOnlyList<TDto>>`
- `IQueryHandler.HandleAsync` wraps the result: returns `Task<Result<TResult>>`

### DTOs (Query Results)
- Named `XxxDto`, live in `Xxx/Queries/` folder
- Positional records, params alphabetically ordered
- Mirror the entity shape

### Result<T>
- Discriminated union: `T` (success) or `ErrorResult` (failure)
- Use implicit operators to construct: `return new ErrorResult("msg")` or `return someValue`
- Access via `result.IsSuccess`, `result.Value`, `result.IsError`, `result.Error`

---

## Relationships
- All `CheckList` instances belong to one `User` (via `UserId`)
- All `Checkable` instances belong to one `CheckList` (via `CheckListId`)
