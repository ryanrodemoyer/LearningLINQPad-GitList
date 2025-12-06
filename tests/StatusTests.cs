using LearningLINQPad.GitList;
using LearningLINQPad.GitList.Tests.Fixtures;

namespace LearningLINQPad.GitList.Tests;

/// <summary>
/// Tests for working directory status queries and GitStatusEntry property mapping.
/// Uses per-test fixtures since status tests require modifying the working directory.
/// </summary>
[Collection("GitContext")]
public class StatusTests
{
	[Fact]
	public void IsClean_WithNoChanges_ReturnsTrue()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var isClean = GitContext.IsClean;

		// Assert
		Assert.True(isClean);
	}

	[Fact]
	public void IsClean_WithModifiedFile_ReturnsFalse()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var isClean = GitContext.IsClean;

		// Assert
		Assert.False(isClean);
	}

	[Fact]
	public void IsClean_WithUntrackedFile_ReturnsFalse()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("untracked.txt", "new file");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var isClean = GitContext.IsClean;

		// Assert
		Assert.False(isClean);
	}

	[Fact]
	public void ChangedFilesCount_WithNoChanges_ReturnsZero()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var count = GitContext.ChangedFilesCount;

		// Assert
		Assert.Equal(0, count);
	}

	[Fact]
	public void ChangedFilesCount_WithMultipleChanges_ReturnsCorrectCount()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file1.txt", "content", "Initial commit");
		fixture.ModifyFile("file1.txt", "modified");
		fixture.CreateFile("file2.txt", "new");
		fixture.CreateFile("file3.txt", "another new");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var count = GitContext.ChangedFilesCount;

		// Assert
		Assert.Equal(3, count);
	}

	[Fact]
	public void Status_ExcludesIgnoredFiles()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateGitIgnore("*.log");
		fixture.CreateFile("debug.log", "log content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var status = GitContext.Status.ToList();

		// Assert
		Assert.DoesNotContain(status, s => s.FilePath.EndsWith(".log"));
	}

	[Fact]
	public void Staged_ContainsStagedFiles()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("new.txt", "new content");
		fixture.StageFile("new.txt");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var staged = GitContext.Staged.ToList();

		// Assert
		Assert.Single(staged);
		Assert.Equal("new.txt", staged.First().FilePath);
	}

	[Fact]
	public void Staged_ContainsModifiedAndStagedFiles()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		fixture.StageFile("file.txt");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var staged = GitContext.Staged.ToList();

		// Assert
		Assert.Single(staged);
		Assert.Equal("file.txt", staged.First().FilePath);
	}

	[Fact]
	public void Unstaged_ContainsModifiedFiles()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var unstaged = GitContext.Unstaged.ToList();

		// Assert
		Assert.Single(unstaged);
		Assert.Equal("file.txt", unstaged.First().FilePath);
	}

	[Fact]
	public void Unstaged_DoesNotContainUntrackedFiles()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("untracked.txt", "new file");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var unstaged = GitContext.Unstaged.ToList();

		// Assert
		Assert.Empty(unstaged);
	}

	[Fact]
	public void Untracked_ContainsNewFiles()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("untracked.txt", "new file");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var untracked = GitContext.Untracked.ToList();

		// Assert
		Assert.Single(untracked);
		Assert.Equal("untracked.txt", untracked.First().FilePath);
	}

	[Fact]
	public void Ignored_ContainsIgnoredFiles()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateGitIgnore("*.log");
		fixture.CreateFile("debug.log", "log content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var ignored = GitContext.Ignored.ToList();

		// Assert
		Assert.Single(ignored);
		Assert.Equal("debug.log", ignored.First().FilePath);
	}
}

/// <summary>
/// Tests for GitStatusEntry property mapping.
/// </summary>
[Collection("GitContext")]
public class GitStatusEntryTests
{
	[Fact]
	public void GitStatusEntry_FilePath_ReturnCorrectPath()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("subdir/nested.txt", "nested content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Untracked.FirstOrDefault(s => s.FilePath.Contains("nested"));

		// Assert
		Assert.NotNull(entry);
		Assert.Equal("subdir/nested.txt", entry.FilePath);
	}

	[Fact]
	public void GitStatusEntry_IndexStatus_ReturnsAddedForNewInIndex()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("new.txt", "new content");
		fixture.StageFile("new.txt");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Staged.First();

		// Assert
		Assert.Equal("Added", entry.IndexStatus);
	}

	[Fact]
	public void GitStatusEntry_IndexStatus_ReturnsModifiedForModifiedInIndex()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		fixture.StageFile("file.txt");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Staged.First();

		// Assert
		Assert.Equal("Modified", entry.IndexStatus);
	}

	[Fact]
	public void GitStatusEntry_WorkDirStatus_ReturnsUntrackedForNewInWorkdir()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("untracked.txt", "content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Untracked.First();

		// Assert
		Assert.Equal("Untracked", entry.WorkDirStatus);
	}

	[Fact]
	public void GitStatusEntry_WorkDirStatus_ReturnsModifiedForModifiedInWorkdir()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Unstaged.First();

		// Assert
		Assert.Equal("Modified", entry.WorkDirStatus);
	}

	[Fact]
	public void GitStatusEntry_IsStaged_TrueForStagedFile()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("new.txt", "new content");
		fixture.StageFile("new.txt");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Staged.First();

		// Assert
		Assert.True(entry.IsStaged);
	}

	[Fact]
	public void GitStatusEntry_IsStaged_FalseForUnstagedFile()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Unstaged.First();

		// Assert
		Assert.False(entry.IsStaged);
	}

	[Fact]
	public void GitStatusEntry_HasUnstagedChanges_TrueForModifiedFile()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.ModifyFile("file.txt", "modified content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Unstaged.First();

		// Assert
		Assert.True(entry.HasUnstagedChanges);
	}

	[Fact]
	public void GitStatusEntry_IsUntracked_TrueForNewFile()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("untracked.txt", "content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Untracked.First();

		// Assert
		Assert.True(entry.IsUntracked);
	}

	[Fact]
	public void GitStatusEntry_IsIgnored_TrueForIgnoredFile()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateGitIgnore("*.log");
		fixture.CreateFile("debug.log", "log content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Ignored.First();

		// Assert
		Assert.True(entry.IsIgnored);
	}

	[Fact]
	public void GitStatusEntry_ToString_ContainsFilePathAndStatus()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateFile("untracked.txt", "content");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Untracked.First();
		var str = entry.ToString();

		// Assert
		Assert.Contains("untracked.txt", str);
	}

	[Fact]
	public void GitStatusEntry_IndexStatus_ReturnsDeletedForDeletedFromIndex()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.DeleteFile("file.txt");
		fixture.StageFile("file.txt");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Staged.First();

		// Assert
		Assert.Equal("Deleted", entry.IndexStatus);
	}

	[Fact]
	public void GitStatusEntry_WorkDirStatus_ReturnsDeletedForDeletedFromWorkdir()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.DeleteFile("file.txt");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var entry = GitContext.Unstaged.First();

		// Assert
		Assert.Equal("Deleted", entry.WorkDirStatus);
	}
}
