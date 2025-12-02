<Query Kind="Statements" />

// Analyze commit patterns and authors

// Commits by author
Commits
	.GroupBy(c => c.Author)
	.Select(g => new {
		Author = g.Key,
		CommitCount = g.Count(),
		FirstCommit = g.Min(c => c.AuthorDate),
		LastCommit = g.Max(c => c.AuthorDate)
	})
	.OrderByDescending(x => x.CommitCount)
	.Dump("Commits by Author");

// Commits by day of week
Commits
	.GroupBy(c => c.AuthorDate.DayOfWeek)
	.Select(g => new {
		DayOfWeek = g.Key,
		CommitCount = g.Count()
	})
	.OrderBy(x => x.DayOfWeek)
	.Dump("Commits by Day of Week");

// Commits by hour of day
Commits
	.GroupBy(c => c.AuthorDate.Hour)
	.Select(g => new {
		Hour = g.Key,
		CommitCount = g.Count()
	})
	.OrderBy(x => x.Hour)
	.Dump("Commits by Hour");

// Find merge commits (commits with multiple parents)
Commits
	.Where(c => c.ParentCount > 1)
	.Take(10)
	.Select(c => new {
		c.ShortSha,
		c.Author,
		c.MessageShort,
		c.ParentCount
	})
	.Dump("Recent Merge Commits");
