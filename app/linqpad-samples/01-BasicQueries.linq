<Query Kind="Statements" />

// Welcome to the Git Repository Data Context!
// This sample shows basic queries you can run against a git repository.

// Get the repository path
RepositoryPath.Dump("Repository Path");

// Show current HEAD branch
Head.Dump("Current HEAD");

// List all local branches
LocalBranches
	.Select(b => new { 
		b.Name, 
		b.IsCurrentRepositoryHead,
		CommitCount = b.Commits.Count(),
		LatestCommit = b.Tip?.MessageShort
	})
	.Dump("Local Branches");

// List all remotes
Remotes.Dump("Remotes");

// Get latest 10 commits
Commits
	.Take(10)
	.Select(c => new {
		c.ShortSha,
		c.Author,
		c.AuthorDate,
		c.MessageShort
	})
	.Dump("Latest 10 Commits");
