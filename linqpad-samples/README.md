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

- **01-BasicQueries.linq** - Introduction to basic repository queries (branches, remotes, recent commits)
- **02-CommitAnalysis.linq** - Analyze commit patterns (by author, day of week, time of day)
- **03-BranchAnalysis.linq** - Branch tracking status and comparisons
- **04-TagsAndReleases.linq** - Working with tags and releases
- **05-SearchCommits.linq** - Search commits by author, message, or date range

## Data Model

The data context exposes these main collections:

- **Commits** - All commits in the repository
- **Branches** - All branches (local and remote)
- **LocalBranches** - Local branches only
- **RemoteBranches** - Remote branches only
- **Tags** - All tags
- **Remotes** - Configured remotes
- **Head** - Current HEAD reference

## Tips

- Use `.Dump()` to visualize any query result
- Combine LINQ operators like `Where()`, `Select()`, `GroupBy()`, `OrderBy()`
- Use IntelliSense (Ctrl+Space) to explore available properties
- The driver uses LibGit2Sharp under the hood, so it's fast and reliable

Enjoy exploring your git repositories with LINQ!
