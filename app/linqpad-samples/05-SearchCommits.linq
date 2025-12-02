<Query Kind="Statements" />

// Search for specific commits

// Search commits by author
var authorName = ""; // Set this to search for a specific author
if (!string.IsNullOrEmpty(authorName))
{
	Commits
		.Where(c => c.Author.Contains(authorName, StringComparison.OrdinalIgnoreCase))
		.Take(20)
		.Select(c => new {
			c.ShortSha,
			c.Author,
			c.AuthorDate,
			c.MessageShort
		})
		.Dump($"Commits by {authorName}");
}

// Search commits by message keyword
var keyword = "fix"; // Change this to search for different keywords
Commits
	.Where(c => c.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
	.Take(20)
	.Select(c => new {
		c.ShortSha,
		c.Author,
		c.AuthorDate,
		c.MessageShort
	})
	.Dump($"Commits containing '{keyword}'");

// Find commits in a date range
var startDate = DateTimeOffset.Now.AddMonths(-1);
var endDate = DateTimeOffset.Now;

Commits
	.Where(c => c.AuthorDate >= startDate && c.AuthorDate <= endDate)
	.Select(c => new {
		c.ShortSha,
		c.Author,
		c.AuthorDate,
		c.MessageShort
	})
	.Dump($"Commits between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}");

// Find commits with specific patterns
Commits
	.Where(c => c.MessageShort.StartsWith("Merge", StringComparison.OrdinalIgnoreCase))
	.Take(10)
	.Select(c => new {
		c.ShortSha,
		c.Author,
		c.AuthorDate,
		c.MessageShort,
		c.ParentCount
	})
	.Dump("Merge Commits");
