<Query Kind="Statements" />

// Compare Working Directory with Last Commit
// See what's changed since the last commit

var lastCommit = Commits.FirstOrDefault();
if (lastCommit != null)
{
	new {
		lastCommit.ShortSha,
		lastCommit.Author,
		lastCommit.AuthorDate,
		lastCommit.Message
	}.Dump("Last Commit");
	
	// Files in the last commit
	$"Files in commit {lastCommit.ShortSha}:".Dump();
	lastCommit.TreeEntries
		.OrderBy(e => e.Path)
		.Take(20)
		.Select(e => new {
			e.Path,
			e.TargetType,
			SHA = e.Sha.Substring(0, 7)
		})
		.Dump("Last Commit Files (first 20)");
}

// Current working directory changes
if (Status.Any())
{
	"Current working directory changes:".Dump();
	Status
		.Select(s => new {
			s.FilePath,
			s.Status,
			Summary = s.IsStaged ? "Staged" :
					 s.HasUnstagedChanges ? "Modified" :
					 s.IsUntracked ? "New" :
					 "Unknown"
		})
		.Dump("Changes Since Last Commit");
		
	// Breakdown
	new {
		NewFiles = Status.Count(s => s.IsUntracked),
		ModifiedFiles = Status.Count(s => s.HasUnstagedChanges),
		StagedFiles = Status.Count(s => s.IsStaged),
		TotalChanges = Status.Count()
	}.Dump("Change Summary");
}
else
{
	"✅ No changes in working directory".Dump();
}

// Compare with specific branch (optional - change branch name as needed)
var mainBranch = LocalBranches.FirstOrDefault(b => b.Name == "main" || b.Name == "master");
if (mainBranch != null && Head.Name != mainBranch.Name)
{
	var mainCommit = mainBranch.Tip;
	var currentCommit = Head.Tip;
	
	$"Comparing {Head.Name} with {mainBranch.Name}:".Dump();
	new {
		CurrentBranch = Head.Name,
		CurrentCommit = currentCommit?.ShortSha,
		MainBranch = mainBranch.Name,
		MainCommit = mainCommit?.ShortSha,
		CommitsBehind = Head.BehindBy,
		CommitsAhead = Head.AheadBy
	}.Dump("Branch Comparison");
}
