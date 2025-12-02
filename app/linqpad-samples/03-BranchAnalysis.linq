<Query Kind="Statements" />

// Branch tracking and comparison

// Show tracking status for local branches
LocalBranches
	.Where(b => b.IsTracking)
	.Select(b => new {
		b.Name,
		b.RemoteName,
		UpstreamBranch = b.UpstreamBranchCanonicalName,
		b.AheadBy,
		b.BehindBy,
		Status = b.AheadBy == 0 && b.BehindBy == 0 ? "Up to date" :
				 b.AheadBy > 0 && b.BehindBy == 0 ? $"Ahead by {b.AheadBy}" :
				 b.AheadBy == 0 && b.BehindBy > 0 ? $"Behind by {b.BehindBy}" :
				 $"Ahead by {b.AheadBy}, Behind by {b.BehindBy}"
	})
	.Dump("Branch Tracking Status");

// Compare branch commit counts
Branches
	.Select(b => new {
		b.Name,
		b.IsRemote,
		CommitCount = b.Commits.Count(),
		LatestCommit = b.Tip?.AuthorDate
	})
	.OrderByDescending(x => x.LatestCommit)
	.Dump("All Branches with Commit Counts");

// Find branches that share the same tip commit
Branches
	.GroupBy(b => b.Tip?.Sha)
	.Where(g => g.Count() > 1 && g.Key != null)
	.Select(g => new {
		CommitSha = g.Key,
		BranchCount = g.Count(),
		Branches = string.Join(", ", g.Select(b => b.Name))
	})
	.Dump("Branches Pointing to Same Commit");
