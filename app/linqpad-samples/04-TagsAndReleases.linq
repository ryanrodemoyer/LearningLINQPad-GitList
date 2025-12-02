<Query Kind="Statements" />

// Work with tags and releases

// List all tags with their target commits
Tags
	.Select(t => new {
		t.Name,
		t.IsAnnotated,
		t.Message,
		t.Tagger,
		t.TaggerDate,
		TargetCommit = t.Target?.ShortSha,
		TargetMessage = t.Target?.MessageShort,
		TargetDate = t.Target?.AuthorDate
	})
	.OrderByDescending(t => t.TargetDate)
	.Dump("All Tags");

// Find commits between two tags (if you have tags)
var tagNames = Tags.Select(t => t.Name).Take(2).ToArray();
if (tagNames.Length >= 2)
{
	var tag1 = Tags.First(t => t.Name == tagNames[0]);
	var tag2 = Tags.First(t => t.Name == tagNames[1]);
	
	$"Commits between {tag1.Name} and {tag2.Name}".Dump();
	
	Commits
		.Where(c => c.AuthorDate >= tag1.Target.AuthorDate && 
					c.AuthorDate <= tag2.Target.AuthorDate)
		.Select(c => new {
			c.ShortSha,
			c.Author,
			c.AuthorDate,
			c.MessageShort
		})
		.Dump();
}
else
{
	"Not enough tags to compare".Dump();
}

// Count commits per tag
Tags
	.Select(t => new {
		t.Name,
		CommitsReachable = t.Target?.Parents.Count() ?? 0
	})
	.Dump("Tag Info");
