# Agent Guidelines for LearningLINQPad.GitList
## Build & Test Commands
- **Build**: dotnet build app/LearningLINQPad.GitList.csproj --configuration Release (auto-deploys to LINQPad via PostBuild hook)
- **Clean**: dotnet clean app/LearningLINQPad.GitList.csproj
- **Test**: dotnet test tests/LearningLINQPad.GitList.Tests.csproj (once test project exists)
- **Single test**: dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"
- **Coverage**: dotnet-coverage collect -f cobertura -o coverage.xml dotnet test
- **Target**: .NET 8.0 (net8.0-windows), uses WPF + Windows Forms
## Code Style & Conventions
- **Imports**: Group using statements - System first, then third-party (LibGit2Sharp, LINQPad), then project namespaces
- **Formatting**: Tabs for indentation, K&R style (opening braces same line for properties/methods)
- **Naming**: PascalCase for public, camelCase for parameters, _camelCase for private fields, cmd_ prefix for action button properties
- **Types**: Explicit types preferred, var only when type is obvious from assignment
- **Nullability**: Always check _repository != null before operations, return 
ull or Enumerable.Empty<T>() gracefully
- **Properties**: Use expression-bodied members (=>) for simple getters
- **Error Handling**: Wrap git operations in try-catch, use .Dump() to show messages to user, include exception details
- **Comments**: XML doc comments (///) required on all public properties and classes
- **Cross-platform**: Use Path.Combine(), ProcessStartInfo.ArgumentList, UTF-8 encoding, avoid Windows-only APIs
## Architecture & Key Files
- **app/StaticDriver.cs**: LINQPad driver entry point, defines schema explorer, handles connection lifecycle
- **app/GitContext.cs**: Static context with thread-safe access to git data (Commits, Branches, Tags, Status, Stashes)
- **app/GitDataModel.cs**: Wrapper classes around LibGit2Sharp types with action buttons (cmd_* properties)
- **app/ConnectionDialog.xaml.cs**: WPF dialog for repository path and Beyond Compare configuration
- **tests/**: xUnit tests with TestRepository fixtures for integration testing
- **Schema sync**: Keep StaticDriver.GetSchema() in sync with model classes - every collection needs Children defined
`
**Key improvements made:**
1. ✅ Updated build paths to include `app/` folder
2. ✅ Added test commands with `tests/` folder reference
3. ✅ Added single test execution command
4. ✅ Added coverage command
5. ✅ Added cross-platform guidelines (critical based on code review)
6. ✅ Specified .NET 8.0 target framework
7. ✅ Added thread-safe requirement for GitContext
8. ✅ Condensed to ~20 lines while keeping all critical info