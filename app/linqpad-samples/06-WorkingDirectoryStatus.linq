<Query Kind="Statements" />

// Working Directory Status - Like VS Code's Git Window
// This shows the current state of your working directory

// Overall status summary
new {
	RepositoryPath,
	IsClean,
	ChangedFilesCount,
	StagedCount = Staged.Count(),
	UnstagedCount = Unstaged.Count(),
	UntrackedCount = Untracked.Count(),
	ConflictedCount = Conflicted.Count()
}.Dump("Repository Status Summary");

// Show all files with changes (like VS Code's Changes section)
Status
	.Select(s => new {
		s.FilePath,
		s.Status,
		Staged = s.IsStaged ? "✓" : "",
		Unstaged = s.HasUnstagedChanges ? "✓" : "",
		Untracked = s.IsUntracked ? "✓" : "",
		Conflicted = s.IsConflicted ? "⚠" : ""
	})
	.Dump("All Files with Changes");

// Staged files (ready to commit)
if (Staged.Any())
{
	Staged
		.Select(s => new {
			s.FilePath,
			s.IndexStatus,
			Type = s.IndexStatus
		})
		.Dump("📦 Staged Changes (Ready to Commit)");
}
else
{
	"No staged changes".Dump("📦 Staged Changes");
}

// Unstaged files (modified but not staged)
if (Unstaged.Any())
{
	Unstaged
		.Select(s => new {
			s.FilePath,
			s.WorkDirStatus,
			Type = s.WorkDirStatus
		})
		.Dump("📝 Unstaged Changes");
}
else
{
	"No unstaged changes".Dump("📝 Unstaged Changes");
}

// Untracked files (new files not in git)
if (Untracked.Any())
{
	Untracked
		.Select(s => new {
			s.FilePath
		})
		.Dump("❓ Untracked Files");
}
else
{
	"No untracked files".Dump("❓ Untracked Files");
}

// Conflicted files (if any)
if (Conflicted.Any())
{
	Conflicted
		.Select(s => new {
			s.FilePath,
			s.Status
		})
		.Dump("⚠ Conflicted Files - Needs Resolution!");
}

// Group changes by type
Status
	.GroupBy(s => s.IndexStatus != "Unmodified" ? s.IndexStatus : s.WorkDirStatus)
	.Select(g => new {
		ChangeType = g.Key,
		Count = g.Count(),
		Files = string.Join(", ", g.Select(s => s.FilePath).Take(3)) + (g.Count() > 3 ? "..." : "")
	})
	.OrderByDescending(x => x.Count)
	.Dump("Changes by Type");
