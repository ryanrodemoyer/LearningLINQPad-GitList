<Query Kind="Statements" />

// Interactive Git Operations - Stage, Commit, and Discard with Buttons!
// Click the buttons in the results to perform git operations

"Click the buttons below to interact with your files!".Dump("Interactive Git Operations");

// Show all files with interactive buttons
if (Status.Any())
{
	Status
		.Select(s => new {
			s.FilePath,
			s.Status,
			s.StageButton,
			s.UnstageButton,
			s.CommitButton,
			s.DiscardButton
		})
		.Dump("All Changes - Click Buttons to Perform Actions");
}
else
{
	"✅ Working directory is clean!".Dump();
}

// Unstaged files - ready to stage
if (Unstaged.Any())
{
	"Files with unstaged changes - click 'Stage' to prepare for commit:".Dump();
	Unstaged
		.Select(s => new {
			s.FilePath,
			s.Status,
			Stage = s.StageButton,
			Discard = s.DiscardButton
		})
		.Dump("Unstaged Changes");
}

// Staged files - ready to commit or unstage
if (Staged.Any())
{
	"Files staged and ready to commit:".Dump();
	Staged
		.Select(s => new {
			s.FilePath,
			s.IsStaged,
			Unstage = s.UnstageButton,
			Commit = s.CommitButton
		})
		.Dump("Staged Changes");
}

// Untracked files - ready to add
if (Untracked.Any())
{
	"New untracked files - click 'Stage' to add to git:".Dump();
	Untracked
		.Select(s => new {
			s.FilePath,
			Stage = s.StageButton
		})
		.Dump("Untracked Files");
}

// Instructions
@"
INSTRUCTIONS:
-------------
• Stage Button: Add file to staging area (git add)
• Unstage Button: Remove file from staging area (git reset)
• Commit Button: Commit just this file (you'll be prompted for a message)
• Discard Button: Discard changes to this file (WARNING: cannot be undone!)

TIPS:
-----
• The CommitButton will stage the file first if needed
• You'll be prompted to confirm before discarding changes
• After clicking a button, re-run this query to see updated status
• You can commit individual files or stage multiple files then commit them together
".Dump("How to Use");
