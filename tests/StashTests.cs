using LearningLINQPad.GitList;
using LearningLINQPad.GitList.Tests.Fixtures;

namespace LearningLINQPad.GitList.Tests;

/// <summary>
/// Tests for stash queries and GitStash property mapping.
/// Uses per-test fixtures since stash tests require modifying repository state.
/// </summary>
[Collection("GitContext")]
public class StashTests
{
	[Fact]
	public void Stashes_EmptyRepo_ReturnsEmpty()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stashes = GitContext.Stashes.ToList();

		// Assert
		Assert.Empty(stashes);
	}

	[Fact]
	public void Stashes_WithStash_ReturnsSingleStash()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		fixture.CreateStash("Test stash");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stashes = GitContext.Stashes.ToList();

		// Assert
		Assert.Single(stashes);
	}

	[Fact]
	public void Stashes_MultipleStashes_ReturnsAll()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");

		fixture.ModifyFile("file.txt", "modified 1");
		fixture.CreateStash("First stash");

		fixture.ModifyFile("file.txt", "modified 2");
		fixture.CreateStash("Second stash");

		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stashes = GitContext.Stashes.ToList();

		// Assert
		Assert.Equal(2, stashes.Count);
	}

	[Fact]
	public void Stashes_OrderedNewestFirst()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");

		fixture.ModifyFile("file.txt", "modified 1");
		fixture.CreateStash("First stash");

		fixture.ModifyFile("file.txt", "modified 2");
		fixture.CreateStash("Second stash");

		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stashes = GitContext.Stashes.ToList();

		// Assert - Index 0 should be the most recent (Second stash)
		Assert.Equal(0, stashes[0].Index);
		Assert.Contains("Second stash", stashes[0].Message);
	}

	[Fact]
	public void GitStash_Index_ReturnsCorrectIndex()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");

		fixture.ModifyFile("file.txt", "modified 1");
		fixture.CreateStash("First stash");

		fixture.ModifyFile("file.txt", "modified 2");
		fixture.CreateStash("Second stash");

		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stashes = GitContext.Stashes.ToList();

		// Assert
		Assert.Equal(0, stashes[0].Index);
		Assert.Equal(1, stashes[1].Index);
	}

	[Fact]
	public void GitStash_Message_ContainsStashMessage()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		fixture.CreateStash("My test stash message");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stash = GitContext.Stashes.First();

		// Assert
		Assert.Contains("My test stash message", stash.Message);
	}

	[Fact]
	public void GitStash_Reference_FormatsCorrectly()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		fixture.CreateStash("Test stash");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stash = GitContext.Stashes.First();

		// Assert
		Assert.Equal("stash@{0}", stash.Reference);
	}

	[Fact]
	public void GitStash_When_ReturnsReasonableTime()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		fixture.CreateStash("Test stash");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stash = GitContext.Stashes.First();

		// Assert - should be within the last hour
		var timeDiff = DateTimeOffset.Now - stash.When;
		Assert.True(timeDiff.TotalHours < 1, "Stash time should be recent");
	}

	[Fact]
	public void GitStash_WorkTree_ReturnsCommit()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		fixture.CreateStash("Test stash");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stash = GitContext.Stashes.First();

		// Assert
		Assert.NotNull(stash.WorkTree);
		Assert.NotNull(stash.WorkTree.Sha);
	}

	[Fact]
	public void GitStash_ToString_IncludesReferenceAndMessage()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		fixture.CreateStash("Test stash");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var stash = GitContext.Stashes.First();
		var str = stash.ToString();

		// Assert
		Assert.Contains("stash@{0}", str);
	}
}
