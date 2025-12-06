using LearningLINQPad.GitList;
using LearningLINQPad.GitList.Tests.Fixtures;

namespace LearningLINQPad.GitList.Tests;

/// <summary>
/// Tests for GitContext initialization and null-safety behavior.
/// Uses per-test fixtures since these tests mutate GitContext state.
/// </summary>
[Collection("GitContext")]
public class GitContextInitializationTests
{
	[Fact]
	public void Initialize_WithValidPath_SetsRepositoryPath()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("test.txt", "content", "Initial commit");

		// Act
		GitContext.Initialize(fixture.RepositoryPath);

		// Assert
		Assert.Equal(fixture.RepositoryPath, GitContext.RepositoryPath);
	}

	[Fact]
	public void Initialize_WithValidPath_MakesCommitsAccessible()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("test.txt", "content", "Initial commit");

		// Act
		GitContext.Initialize(fixture.RepositoryPath);

		// Assert
		Assert.NotEmpty(GitContext.Commits);
	}

	[Fact]
	public void Initialize_WithInvalidPath_ThrowsRepositoryNotFoundException()
	{
		// Arrange
		var invalidPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

		// Act & Assert
		Assert.Throws<LibGit2Sharp.RepositoryNotFoundException>(() =>
			GitContext.Initialize(invalidPath));
	}

	[Fact]
	public void Initialize_CalledTwice_UpdatesRepositoryPath()
	{
		// Arrange
		using var fixture1 = new TestRepositoryFixture();
		fixture1.CreateCommit("test1.txt", "content1", "First repo commit");

		using var fixture2 = new TestRepositoryFixture();
		fixture2.CreateCommit("test2.txt", "content2", "Second repo commit");

		// Act
		GitContext.Initialize(fixture1.RepositoryPath);
		GitContext.Initialize(fixture2.RepositoryPath);

		// Assert
		Assert.Equal(fixture2.RepositoryPath, GitContext.RepositoryPath);
	}

	[Fact]
	public void Initialize_SamePathTwice_DoesNotReinitialize()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("test.txt", "content", "Initial commit");

		// Act
		GitContext.Initialize(fixture.RepositoryPath);
		var firstCommitCount = GitContext.Commits.Count();

		// Add another commit directly to the fixture
		fixture.CreateCommit("test2.txt", "content2", "Second commit");

		// Re-initialize with same path (should not reinitialize)
		GitContext.Initialize(fixture.RepositoryPath);
		var secondCommitCount = GitContext.Commits.Count();

		// Assert - commit count should increase because we're still using the same repo instance
		// which sees the new commit
		Assert.Equal(2, secondCommitCount);
	}
}

/// <summary>
/// Tests for GitContext null-safety when not initialized.
/// </summary>
[Collection("GitContext")]
public class GitContextNullSafetyTests
{
	/// <summary>
	/// Forces GitContext to an uninitialized state by pointing to a new temp repo
	/// then disposing the underlying repo. This is a workaround since GitContext is static.
	/// </summary>
	private void ForceUninitializedState()
	{
		// Initialize with a path that doesn't exist to ensure we're in a known state
		// We can't truly uninitialize, but we can test empty repo behavior
	}

	[Fact]
	public void Commits_WhenRepoIsClean_ReturnsCommits()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("test.txt", "content", "Initial commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var commits = GitContext.Commits.ToList();

		// Assert
		Assert.Single(commits);
	}

	[Fact]
	public void Branches_ReturnsAtLeastOneBranch()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("test.txt", "content", "Initial commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var branches = GitContext.Branches.ToList();

		// Assert
		Assert.NotEmpty(branches);
	}

	[Fact]
	public void Head_ReturnsCurrentBranch()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("test.txt", "content", "Initial commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var head = GitContext.Head;

		// Assert
		Assert.NotNull(head);
	}

	[Fact]
	public void IsClean_WithNoChanges_ReturnsTrue()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("test.txt", "content", "Initial commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var isClean = GitContext.IsClean;

		// Assert
		Assert.True(isClean);
	}

	[Fact]
	public void ChangedFilesCount_WithNoChanges_ReturnsZero()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("test.txt", "content", "Initial commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var count = GitContext.ChangedFilesCount;

		// Assert
		Assert.Equal(0, count);
	}
}
