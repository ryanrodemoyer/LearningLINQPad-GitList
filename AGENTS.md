# Agent Guidelines for LearningLINQPad.GitList

## Build & Deploy
- **Build**: `dotnet build --configuration Release` (auto-deploys to LINQPad via PostBuild hook)
- **Clean**: `dotnet clean`
- **No tests**: This is a LINQPad driver with no unit test framework

## Code Style
- **Imports**: Group `using` statements - System first, then third-party (LibGit2Sharp, LINQPad), then project namespaces
- **Formatting**: Tabs for indentation, opening braces on same line for properties/methods, K&R style
- **Naming**: PascalCase for public members, camelCase for parameters, `_camelCase` for private fields, `cmd_` prefix for action button properties
- **Properties**: Use expression-bodied members (`=>`) for simple getters
- **Types**: Explicit types preferred, use `var` only when type is obvious from right side
- **Nullability**: Check for `null` on `_repository` field before operations, return `null` or empty collections gracefully
- **Comments**: XML doc comments (`///`) on all public properties and classes
- **Action Buttons**: Return `LINQPad.Controls.Button` from properties named `cmd_ActionName`, return `null` when button shouldn't appear
- **Error Handling**: Wrap git operations in try-catch, use `.Dump()` to show errors/success messages to user
- **Static Context**: All public collections in `GitContext.cs` are static properties that check `_repo != null` before accessing
- **Schema**: Keep StaticDriver.cs `GetSchema()` in sync with model classes - every collection needs `Children` defined for consistency

## Architecture
- **StaticDriver.cs**: LINQPad driver entry point, defines schema explorer
- **GitContext.cs**: Static context exposing git data as properties (Commits, Branches, Tags, Status, Stashes, etc.)
- **GitDataModel.cs**: Wrapper classes around LibGit2Sharp types with action buttons
- **ConnectionDialog**: WPF dialog for repository path and Beyond Compare configuration
