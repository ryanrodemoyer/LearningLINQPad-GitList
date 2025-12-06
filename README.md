# LearningLINQPad.GitList

A LINQPad driver for exploring and managing Git repositories using LINQ. Query commits, branches, tags, and working directory status directly from LINQPad.

[![Build and Test](https://github.com/ryanrodemoyer/LearningLINQPad-GitList/actions/workflows/build-and-publish.yml/badge.svg)](https://github.com/ryanrodemoyer/LearningLINQPad-GitList/actions/workflows/build-and-publish.yml)
[![NuGet Version](https://img.shields.io/nuget/v/LearningLINQPad.GitList)](https://www.nuget.org/packages/LearningLINQPad.GitList)
[![NuGet Downloads](https://img.shields.io/nuget/dt/LearningLINQPad.GitList)](https://www.nuget.org/packages/LearningLINQPad.GitList)
[![License](https://img.shields.io/github/license/ryanrodemoyer/LearningLINQPad-GitList)](LICENSE)
![.NET 8.0](https://img.shields.io/badge/.NET-8.0--windows-512BD4)

## Installation

### From NuGet (Recommended)

1. Open LINQPad
2. Click **Add connection** in the left pane
3. Click **View more drivers...**
4. Search for **LearningLINQPad.GitList**
5. Click **Install**

### Manual Installation

Download the latest `.nupkg` from [NuGet](https://www.nuget.org/packages/LearningLINQPad.GitList) or [GitHub Releases](https://github.com/ryanrodemoyer/LearningLINQPad-GitList/releases), then:

1. In LINQPad, click **Add connection**
2. Click **View more drivers...**
3. Click **Install driver from .nupkg file...**
4. Select the downloaded file

## Getting Started

1. Click **Add connection** in the left pane
2. Select **LearningLINQPad.GitList** from the list of drivers
3. Click **Next** and browse to select a Git repository folder
4. (Optional) Configure Beyond Compare path for diff viewing
5. Click **OK** to create the connection
6. Write LINQ queries against your repository!

## Features

### Query Repository Data

- **Commits** - All commits across all branches
- **Branches** - Local and remote branches with tracking info
- **Tags** - Lightweight and annotated tags
- **Remotes** - Configured remote repositories
- **Stashes** - Stashed changes

### Working Directory Status

- **Status** - All files with changes
- **Staged** - Files ready to commit
- **Unstaged** - Modified files not yet staged
- **Untracked** - New files not in Git
- **Ignored** - Files ignored by .gitignore
- **Conflicted** - Files with merge conflicts

### Interactive Actions

The driver includes action buttons for common Git operations:

| Action | Description |
|--------|-------------|
| **Stage** | Stage a file for commit |
| **Unstage** | Remove a file from staging |
| **Commit** | Commit a staged file with a message |
| **Discard** | Discard changes to a file |
| **View Diff** | View file diff (in LINQPad or Beyond Compare) |
| **Apply/Pop/Drop Stash** | Manage stashed changes |

## Example Queries

### Basic Queries

```csharp
// Show current branch
Head.Name.Dump("Current Branch");

// List all branches
Branches.Dump();

// Recent commits
Commits.Take(10).Dump();

// Check if working directory is clean
IsClean.Dump("Working Directory Clean?");
```

### Commit Analysis

```csharp
// Commits by author
Commits
    .GroupBy(c => c.Author)
    .Select(g => new { Author = g.Key, Count = g.Count() })
    .OrderByDescending(x => x.Count)
    .Dump("Commits by Author");

// Commits in the last 7 days
Commits
    .Where(c => c.AuthorDate > DateTimeOffset.Now.AddDays(-7))
    .Dump("Recent Commits");

// Search commits by message
Commits
    .Where(c => c.Message.Contains("fix", StringComparison.OrdinalIgnoreCase))
    .Take(20)
    .Dump("Bug Fix Commits");
```

### Branch Analysis

```csharp
// Branch tracking status
Branches
    .Where(b => !b.IsRemote && b.IsTracking)
    .Select(b => new { 
        b.Name, 
        b.AheadBy, 
        b.BehindBy,
        Status = b.AheadBy > 0 ? "Ahead" : b.BehindBy > 0 ? "Behind" : "Up to date"
    })
    .Dump("Branch Status");
```

### Working Directory Status

```csharp
// Files ready to commit
Staged.Dump("Staged Files");

// Modified but not staged
Unstaged.Dump("Unstaged Changes");

// New files
Untracked.Dump("Untracked Files");

// Count of changed files
ChangedFilesCount.Dump("Changed Files");
```

### Tags and Releases

```csharp
// All tags with their target commits
Tags
    .Select(t => new { 
        t.Name, 
        t.IsAnnotated, 
        Commit = t.Target?.ShortSha,
        t.Message 
    })
    .Dump("Tags");
```

## Configuration

### Beyond Compare Integration

For enhanced diff viewing, configure Beyond Compare:

1. When adding a connection, enter the path to `BCompare.exe`
2. Typically: `C:\Program Files\Beyond Compare 4\BCompare.exe`
3. The **View Diff** button will open diffs in Beyond Compare instead of LINQPad

## Sample Queries

The driver includes sample LINQPad queries in the `linqpad-samples` folder:

- `01-BasicQueries.linq` - Introduction to repository queries
- `02-CommitAnalysis.linq` - Analyze commit patterns
- `03-BranchAnalysis.linq` - Branch tracking and comparisons
- `04-TagsAndReleases.linq` - Working with tags
- `05-SearchCommits.linq` - Search commits
- `06-WorkingDirectoryStatus.linq` - View current status
- `07-MonitorChanges.linq` - Monitor pending changes
- `08-CompareWithLastCommit.linq` - Compare with last commit
- `09-InteractiveGitOperations.linq` - Stage, unstage, commit actions
- `10-QuickGitWorkflows.linq` - Common Git workflows

## Requirements

- LINQPad 8+ (uses .NET 8)
- Windows (WPF-based connection dialog)

## Building from Source

```bash
# Clone the repository
git clone https://github.com/ryanrodemoyer/LearningLINQPad-GitList.git
cd LearningLINQPad-GitList

# Build
dotnet build app/LearningLINQPad.GitList.csproj --configuration Release

# Run tests
dotnet test tests/LearningLINQPad.GitList.Tests.csproj

# The PostBuild hook automatically deploys to LINQPad's driver folder
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [LibGit2Sharp](https://github.com/libgit2/libgit2sharp) - Git implementation for .NET
- [LINQPad](https://www.linqpad.net/) - The .NET programmer's playground

## Author

**Ryan Rodemoyer** - [LearningLINQPad](https://www.learninglinqpad.com)
