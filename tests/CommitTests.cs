using LearningLINQPad.GitList;
using LearningLINQPad.GitList.Tests.Fixtures;

namespace LearningLINQPad.GitList.Tests;

/// <summary>
/// Tests for commit queries and GitCommit property mapping.
/// Uses the shared RealisticRepositoryFixture for read-only tests.
/// </summary>
[Collection("GitContext")]
public class CommitTests : IClassFixture<RealisticRepositoryFixture>
{
	private readonly RealisticRepositoryFixture _fixture;

	public CommitTests(RealisticRepositoryFixture fixture)
	{
		_fixture = fixture;
		GitContext.Initialize(_fixture.RepositoryPath);
	}

	[Fact]
	public void Commits_ReturnsAllCommits()
	{
		// Act
		var commits = GitContext.Commits.ToList();

		// Assert - fixture has 6 commits: initial, second, feature, doc-placeholder, merge, post-merge
		Assert.True(commits.Count >= 6, $"Expected at least 6 commits, got {commits.Count}");
	}

	[Fact]
	public void Commits_IncludesInitialCommit()
	{
		// Act
		var commits = GitContext.Commits.ToList();

		// Assert
		Assert.Contains(commits, c => c.Message.Contains("Initial commit"));
	}

	[Fact]
	public void Commits_IncludesFeatureBranchCommits()
	{
		// Act
		var commits = GitContext.Commits.ToList();

		// Assert
		Assert.Contains(commits, c => c.Message.Contains("widget"));
	}

	[Fact]
	public void GitCommit_Sha_IsFortyCharacters()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert
		Assert.Equal(40, commit.Sha.Length);
		Assert.Matches("^[a-f0-9]{40}$", commit.Sha);
	}

	[Fact]
	public void GitCommit_ShortSha_IsSevenCharacters()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert
		Assert.Equal(7, commit.ShortSha.Length);
		Assert.StartsWith(commit.ShortSha, commit.Sha);
	}

	[Fact]
	public void GitCommit_Author_IsMapped()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert
		Assert.Equal(RealisticRepositoryFixture.AuthorName, commit.Author);
	}

	[Fact]
	public void GitCommit_AuthorEmail_IsMapped()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert
		Assert.Equal(RealisticRepositoryFixture.AuthorEmail, commit.AuthorEmail);
	}

	[Fact]
	public void GitCommit_AuthorDate_IsReasonable()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert - should be within the last hour (test execution time)
		var timeDiff = DateTimeOffset.Now - commit.AuthorDate;
		Assert.True(timeDiff.TotalHours < 1, "Commit date should be recent");
	}

	[Fact]
	public void GitCommit_Message_ContainsFullMessage()
	{
		// Act
		var initialCommit = GitContext.Commits.FirstOrDefault(c => c.Message.Contains("Initial commit"));

		// Assert
		Assert.NotNull(initialCommit);
		Assert.Equal("Initial commit", initialCommit.Message.Trim());
	}

	[Fact]
	public void GitCommit_MessageShort_IsFirstLine()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert
		Assert.DoesNotContain("\n", commit.MessageShort);
	}

	[Fact]
	public void GitCommit_ParentCount_InitialCommitIsZero()
	{
		// Act
		var initialCommit = GitContext.Commits.FirstOrDefault(c => c.Message.Contains("Initial commit"));

		// Assert
		Assert.NotNull(initialCommit);
		Assert.Equal(0, initialCommit.ParentCount);
	}

	[Fact]
	public void GitCommit_ParentCount_RegularCommitIsOne()
	{
		// Act
		var secondCommit = GitContext.Commits.FirstOrDefault(c => c.Message.Contains("Add application files"));

		// Assert
		Assert.NotNull(secondCommit);
		Assert.Equal(1, secondCommit.ParentCount);
	}

	[Fact]
	public void GitCommit_ParentCount_MergeCommitIsTwo()
	{
		// Act
		var commits = GitContext.Commits.ToList();
		var mergeCommit = commits.FirstOrDefault(c => c.ParentCount == 2);

		// Assert
		Assert.NotNull(mergeCommit);
	}

	[Fact]
	public void GitCommit_Parents_ReturnsParentCommits()
	{
		// Act
		var secondCommit = GitContext.Commits.FirstOrDefault(c => c.Message.Contains("Add application files"));

		// Assert
		Assert.NotNull(secondCommit);
		var parents = secondCommit.Parents.ToList();
		Assert.Single(parents);
		Assert.Contains("Initial commit", parents.First().Message);
	}

	[Fact]
	public void GitCommit_TreeEntries_ListsFilesInCommit()
	{
		// Act
		var secondCommit = GitContext.Commits.FirstOrDefault(c => c.Message.Contains("Add application files"));

		// Assert
		Assert.NotNull(secondCommit);
		var entries = secondCommit.TreeEntries.ToList();
		Assert.NotEmpty(entries);
	}

	[Fact]
	public void GitCommit_TreeSha_IsFortyCharacters()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert
		Assert.Equal(40, commit.TreeSha.Length);
	}

	[Fact]
	public void GitCommit_ToString_IncludesShortShaAndMessage()
	{
		// Act
		var commit = GitContext.Commits.First();
		var str = commit.ToString();

		// Assert
		Assert.Contains(commit.ShortSha, str);
	}

	[Fact]
	public void GitCommit_Committer_IsMapped()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert
		Assert.Equal(RealisticRepositoryFixture.CommitterName, commit.Committer);
	}

	[Fact]
	public void GitCommit_CommitterEmail_IsMapped()
	{
		// Act
		var commit = GitContext.Commits.First();

		// Assert
		Assert.Equal(RealisticRepositoryFixture.CommitterEmail, commit.CommitterEmail);
	}
}

/// <summary>
/// Tests for empty repository scenarios.
/// </summary>
[Collection("GitContext")]
public class CommitEmptyRepoTests
{
	[Fact]
	public void Commits_EmptyRepo_ReturnsEmpty()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		// Don't create any commits
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var commits = GitContext.Commits.ToList();

		// Assert
		Assert.Empty(commits);
	}

	[Fact]
	public void Commits_SingleCommit_ReturnsSingleCommit()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Only commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var commits = GitContext.Commits.ToList();

		// Assert
		Assert.Single(commits);
		Assert.Equal("Only commit", commits.First().Message.Trim());
	}
}
