<Query Kind="Statements" />

// Monitor Changes - Real-time working directory monitoring
// This query shows what files would be included in your next commit
// and what's still pending in your working directory

// Check if working directory is clean
if (IsClean)
{
	"✅ Working directory is clean - no changes to commit".Dump("Status");
}
else
{
	$"⚠ Working directory has {ChangedFilesCount} file(s) with changes".Dump("Status");
	
	// What would be committed if you run "git commit" now
	if (Staged.Any())
	{
		"These changes will be committed:".Dump();
		Staged
			.Select(s => new {
				File = s.FilePath,
				Staged = s.IsStaged
			})
			.Dump("Staged for Commit");
	}
	else
	{
		"No changes staged for commit".Dump();
	}
	
	// What's been modified but not staged yet
	if (Unstaged.Any())
	{
		"These changes are not staged (use 'git add' to stage):".Dump();
		Unstaged
			.Select(s => new {
				File = s.FilePath,
				Change = s.Status
			})
			.Dump("Not Staged");
	}
	
	// New files not tracked
	if (Untracked.Any())
	{
		"These files are not tracked (use 'git add' to include):".Dump();
		Untracked
			.Select(s => s.FilePath)
			.Dump("Untracked");
	}
}

// Recent commits for context
"Last 5 commits:".Dump();
Commits
	.Take(5)
	.Select(c => new {
		c.ShortSha,
		c.Author,
		c.AuthorDate,
		c.MessageShort
	})
	.Dump();

// Branch status
new {
	CurrentBranch = Head.Name,
	IsHead = Head.IsCurrentRepositoryHead,
	AheadBy = Head.AheadBy,
	BehindBy = Head.BehindBy,
	Tracking = Head.IsTracking ? Head.UpstreamBranchCanonicalName : "Not tracking",
	Status = Head.AheadBy == 0 && Head.BehindBy == 0 ? "Up to date" :
			 Head.AheadBy > 0 && Head.BehindBy == 0 ? $"Ahead by {Head.AheadBy} commit(s)" :
			 Head.AheadBy == 0 && Head.BehindBy > 0 ? $"Behind by {Head.BehindBy} commit(s)" :
			 $"Diverged: ahead {Head.AheadBy}, behind {Head.BehindBy}"
}.Dump("Current Branch Status");
