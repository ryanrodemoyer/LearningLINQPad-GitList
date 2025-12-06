using LearningLINQPad.GitList;
using LearningLINQPad.GitList.Tests.Fixtures;

namespace LearningLINQPad.GitList.Tests;

/// <summary>
/// Tests for branch queries and GitBranch property mapping.
/// Uses the shared RealisticRepositoryFixture for read-only tests.
/// </summary>
[Collection("GitContext")]
public class BranchTests : IClassFixture<RealisticRepositoryFixture>
{
	private readonly RealisticRepositoryFixture _fixture;

	public BranchTests(RealisticRepositoryFixture fixture)
	{
		_fixture = fixture;
		GitContext.Initialize(_fixture.RepositoryPath);
	}

	[Fact]
	public void Branches_ReturnsAllBranches()
	{
		// Act
		var branches = GitContext.Branches.ToList();

		// Assert - fixture has main, develop, feature/add-widgets
		Assert.True(branches.Count >= 3, $"Expected at least 3 branches, got {branches.Count}");
	}

	[Fact]
	public void Branches_IncludesMainBranch()
	{
		// Act
		var branches = GitContext.Branches.ToList();

		// Assert
		Assert.Contains(branches, b => b.Name == RealisticRepositoryFixture.MainBranchName);
	}

	[Fact]
	public void Branches_IncludesDevelopBranch()
	{
		// Act
		var branches = GitContext.Branches.ToList();

		// Assert
		Assert.Contains(branches, b => b.Name == RealisticRepositoryFixture.DevelopBranchName);
	}

	[Fact]
	public void Branches_IncludesFeatureBranch()
	{
		// Act
		var branches = GitContext.Branches.ToList();

		// Assert
		Assert.Contains(branches, b => b.Name == RealisticRepositoryFixture.FeatureBranchName);
	}

	[Fact]
	public void GitBranch_Name_ReturnsFriendlyName()
	{
		// Act
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		Assert.Equal(RealisticRepositoryFixture.MainBranchName, mainBranch.Name);
	}

	[Fact]
	public void GitBranch_CanonicalName_ReturnsFullRefName()
	{
		// Act
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		Assert.Equal($"refs/heads/{RealisticRepositoryFixture.MainBranchName}", mainBranch.CanonicalName);
	}

	[Fact]
	public void GitBranch_IsRemote_FalseForLocalBranches()
	{
		// Act
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		Assert.False(mainBranch.IsRemote);
	}

	[Fact]
	public void Head_ReturnsCurrentBranch()
	{
		// Act
		var head = GitContext.Head;

		// Assert
		Assert.NotNull(head);
		Assert.Equal(RealisticRepositoryFixture.MainBranchName, head.Name);
	}

	[Fact]
	public void GitBranch_IsCurrentRepositoryHead_TrueOnlyForHead()
	{
		// Act
		var branches = GitContext.Branches.ToList();
		var headBranches = branches.Where(b => b.IsCurrentRepositoryHead).ToList();

		// Assert
		Assert.Single(headBranches);
		Assert.Equal(RealisticRepositoryFixture.MainBranchName, headBranches.First().Name);
	}

	[Fact]
	public void GitBranch_Tip_ReturnsLatestCommit()
	{
		// Act
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		Assert.NotNull(mainBranch.Tip);
		Assert.NotNull(mainBranch.Tip.Sha);
	}

	[Fact]
	public void GitBranch_Commits_ReturnsAllReachableCommits()
	{
		// Act
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		var commits = mainBranch.Commits.ToList();
		Assert.True(commits.Count >= 4, $"Main branch should have at least 4 commits, got {commits.Count}");
	}

	[Fact]
	public void GitBranch_ToString_IncludesName()
	{
		// Act
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		var str = mainBranch.ToString();
		Assert.Contains(RealisticRepositoryFixture.MainBranchName, str);
	}

	[Fact]
	public void GitBranch_ToString_IncludesHeadMarkerForCurrentBranch()
	{
		// Act
		var head = GitContext.Head;

		// Assert
		Assert.NotNull(head);
		var str = head.ToString();
		Assert.Contains("(HEAD)", str);
	}

	[Fact]
	public void GitBranch_IsTracking_FalseWhenNoUpstream()
	{
		// Act - local branches without remote tracking
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		Assert.False(mainBranch.IsTracking);
	}

	[Fact]
	public void GitBranch_AheadBy_NullWhenNoUpstream()
	{
		// Act
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		Assert.Null(mainBranch.AheadBy);
	}

	[Fact]
	public void GitBranch_BehindBy_NullWhenNoUpstream()
	{
		// Act
		var mainBranch = GitContext.Branches.FirstOrDefault(b => b.Name == RealisticRepositoryFixture.MainBranchName);

		// Assert
		Assert.NotNull(mainBranch);
		Assert.Null(mainBranch.BehindBy);
	}
}

/// <summary>
/// Tests for branch operations with per-test fixtures.
/// </summary>
[Collection("GitContext")]
public class BranchMutationTests
{
	[Fact]
	public void Branches_NewlyCreatedBranch_AppearsInList()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateBranch("new-feature");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var branches = GitContext.Branches.ToList();

		// Assert
		Assert.Contains(branches, b => b.Name == "new-feature");
	}

	[Fact]
	public void Branches_AfterCheckout_HeadChanges()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateAndCheckoutBranch("new-feature");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var head = GitContext.Head;

		// Assert
		Assert.Equal("new-feature", head.Name);
	}
}
