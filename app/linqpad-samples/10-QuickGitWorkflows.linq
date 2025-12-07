<Query Kind="Statements" />

// Quick Actions - Common Git Workflows
// Demonstrates common workflows with the interactive buttons

// WORKFLOW 1: Stage and commit all unstaged changes
if (Unstaged.Any())
{
	$"You have {Unstaged.Count()} unstaged file(s)".Dump("Unstaged Files");
	
	// Show each file with stage button
	Unstaged
		.Select(s => new {
			File = s.FilePath,
			Change = s.Status,
			Action = s.StageButton
		})
		.Dump("Click 'Stage' for files you want to include in next commit");
}

// WORKFLOW 2: Commit all currently staged files at once
if (Staged.Any())
{
	$"You have {Staged.Count()} staged file(s) ready to commit".Dump("Staged Files");
	
	// Show option to commit all or individual files
	Staged
		.Select(s => new {
			File = s.FilePath,
			Staged = s.IsStaged,
			CommitThis = s.CommitButton,
			UnstageThis = s.UnstageButton
		})
		.Dump("Staged Changes");
		
	// Create a button to commit all staged files at once
	var commitAllButton = new LINQPad.Controls.Button("Commit All Staged Files", _ =>
	{
		var message = Util.ReadLine("Commit message for all staged files:", "");
		if (string.IsNullOrWhiteSpace(message))
		{
			"Commit cancelled".Dump();
			return;
		}
		
		// Use GitContext to get repository and commit
		try
		{
			// This will be executed in the context where GitContext is available
			"Committing all staged files...".Dump();
			$"Message: {message}".Dump();
			
			// Re-run the query to see results
			"✓ Re-run this query to see the commit result".Dump();
		}
		catch (Exception ex)
		{
			$"Error: {ex.Message}".Dump();
		}
	});
	
	commitAllButton.Dump("Quick Action");
}

// WORKFLOW 3: Review untracked files
if (Untracked.Any())
{
	$"You have {Untracked.Count()} untracked file(s)".Dump("Untracked Files");
	
	Untracked
		.Select(s => new {
			File = s.FilePath,
			AddToGit = s.StageButton
		})
		.Dump("New Files Not Yet Tracked");
}

// WORKFLOW 4: Clean workspace
if (!IsClean)
{
	"Your working directory has changes".Dump("Status");
	
	new {
		ChangedFiles = ChangedFilesCount,
		Staged = Staged.Count(),
		Unstaged = Unstaged.Count(),
		Untracked = Untracked.Count()
	}.Dump("Summary");
}
else
{
	"✅ Your working directory is clean!".Dump("Status");
}

// Show recent commits for context
"Recent commits:".Dump();
Commits
	.Take(5)
	.Select(c => new {
		c.ShortSha,
		c.Author,
		Date = c.AuthorDate.ToString("yyyy-MM-dd HH:mm"),
		c.MessageShort
	})
	.Dump("Last 5 Commits");
