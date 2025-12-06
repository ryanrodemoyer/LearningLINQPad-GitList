using LearningLINQPad.GitList;
using LearningLINQPad.GitList.Tests.Fixtures;

namespace LearningLINQPad.GitList.Tests;

/// <summary>
/// Tests for remote queries and GitRemote property mapping.
/// Uses the shared RealisticRepositoryFixture for read-only tests.
/// </summary>
[Collection("GitContext")]
public class RemoteTests : IClassFixture<RealisticRepositoryFixture>
{
	private readonly RealisticRepositoryFixture _fixture;

	public RemoteTests(RealisticRepositoryFixture fixture)
	{
		_fixture = fixture;
		GitContext.Initialize(_fixture.RepositoryPath);
	}

	[Fact]
	public void Remotes_ReturnsConfiguredRemotes()
	{
		// Act
		var remotes = GitContext.Remotes.ToList();

		// Assert
		Assert.Single(remotes);
	}

	[Fact]
	public void GitRemote_Name_ReturnsRemoteName()
	{
		// Act
		var remote = GitContext.Remotes.First();

		// Assert
		Assert.Equal(RealisticRepositoryFixture.RemoteName, remote.Name);
	}

	[Fact]
	public void GitRemote_Url_ReturnsRemoteUrl()
	{
		// Act
		var remote = GitContext.Remotes.First();

		// Assert
		Assert.Equal(RealisticRepositoryFixture.RemoteUrl, remote.Url);
	}

	[Fact]
	public void GitRemote_PushUrl_ReturnsUrl()
	{
		// Act
		var remote = GitContext.Remotes.First();

		// Assert
		// PushUrl falls back to Url if not explicitly set
		Assert.Equal(RealisticRepositoryFixture.RemoteUrl, remote.PushUrl);
	}

	[Fact]
	public void GitRemote_ToString_ContainsNameAndUrl()
	{
		// Act
		var remote = GitContext.Remotes.First();
		var str = remote.ToString();

		// Assert
		Assert.Contains(RealisticRepositoryFixture.RemoteName, str);
		Assert.Contains(RealisticRepositoryFixture.RemoteUrl, str);
	}
}

/// <summary>
/// Tests for remote operations with per-test fixtures.
/// </summary>
[Collection("GitContext")]
public class RemoteMutationTests
{
	[Fact]
	public void Remotes_NoRemotes_ReturnsEmpty()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		// No remotes added
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var remotes = GitContext.Remotes.ToList();

		// Assert
		Assert.Empty(remotes);
	}

	[Fact]
	public void Remotes_MultipleRemotes_ReturnsAll()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.AddRemote("origin", "https://github.com/example/repo1.git");
		fixture.AddRemote("upstream", "https://github.com/example/repo2.git");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var remotes = GitContext.Remotes.ToList();

		// Assert
		Assert.Equal(2, remotes.Count);
		Assert.Contains(remotes, r => r.Name == "origin");
		Assert.Contains(remotes, r => r.Name == "upstream");
	}
}
