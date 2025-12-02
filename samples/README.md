# Git Repository LINQPad Driver - Sample Queries

This folder contains sample queries to help you get started with the Git Repository data context.

## How to Use

1. In LINQPad, click "Add connection" in the left pane
2. Select "LearningLINQPad.GitList" from the list of drivers
3. Click "Next" and browse to select a git repository folder
4. Click "OK" to create the connection
5. Open any of these sample files and select your new connection from the dropdown
6. Press F5 to run the query!

## Sample Files

### Historical Data
- **01-BasicQueries.linq** - Introduction to basic repository queries (branches, remotes, recent commits)
- **02-CommitAnalysis.linq** - Analyze commit patterns (by author, day of week, time of day)
- **03-BranchAnalysis.linq** - Branch tracking status and comparisons
- **04-TagsAndReleases.linq** - Working with tags and releases
- **05-SearchCommits.linq** - Search commits by author, message, or date range

### Working Directory Status (NEW!)
- **06-WorkingDirectoryStatus.linq** - View current status like VS Code's Git window
- **07-MonitorChanges.linq** - Monitor what would be committed and what's pending
- **08-CompareWithLastCommit.linq** - Compare working directory with last commit

## Data Model

The data context exposes these main collections:

### Historical Collections
- **Commits** - All commits in the repository
- **Branches** - All branches (local and remote)
- **LocalBranches** - Local branches only
- **RemoteBranches** - Remote branches only
- **Tags** - All tags
- **Remotes** - Configured remotes
- **Head** - Current HEAD reference

### Working Directory Status Collections
- **Status** - All files with changes (staged, unstaged, untracked)
- **Staged** - Files ready to commit (in staging area)
- **Unstaged** - Modified files not yet staged
- **Untracked** - New files not in git
- **Ignored** - Files ignored by .gitignore
- **Conflicted** - Files with merge conflicts
- **IsClean** - Boolean: is working directory clean?
- **ChangedFilesCount** - Count of changed files

## Tips

- Use `.Dump()` to visualize any query result
- Combine LINQ operators like `Where()`, `Select()`, `GroupBy()`, `OrderBy()`
- Use IntelliSense (Ctrl+Space) to explore available properties
- The driver uses LibGit2Sharp under the hood, so it's fast and reliable
- Status collections refresh on each query - perfect for monitoring changes!

## Example Queries

```csharp
// Check if working directory is clean
IsClean.Dump();

// Show all staged files
Staged.Select(s => s.FilePath).Dump();

// Find recent commits by a specific author
Commits
    .Where(c => c.Author.Contains("Ryan"))
    .Take(10)
    .Dump();

// Compare current branch with main
Head.Name.Dump("Current Branch");
new { Head.AheadBy, Head.BehindBy }.Dump("Status");
```

Enjoy exploring your git repositories with LINQ!
