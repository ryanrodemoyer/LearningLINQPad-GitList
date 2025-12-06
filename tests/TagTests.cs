using LearningLINQPad.GitList;
using LearningLINQPad.GitList.Tests.Fixtures;

namespace LearningLINQPad.GitList.Tests;

/// <summary>
/// Tests for tag queries and GitTag property mapping.
/// Uses the shared RealisticRepositoryFixture for read-only tests.
/// </summary>
[Collection("GitContext")]
public class TagTests : IClassFixture<RealisticRepositoryFixture>
{
	private readonly RealisticRepositoryFixture _fixture;

	public TagTests(RealisticRepositoryFixture fixture)
	{
		_fixture = fixture;
		GitContext.Initialize(_fixture.RepositoryPath);
	}

	[Fact]
	public void Tags_ReturnsAllTags()
	{
		// Act
		var tags = GitContext.Tags.ToList();

		// Assert - fixture has v0.1.0 (lightweight) and v1.0.0 (annotated)
		Assert.Equal(2, tags.Count);
	}

	[Fact]
	public void Tags_IncludesLightweightTag()
	{
		// Act
		var tags = GitContext.Tags.ToList();

		// Assert
		Assert.Contains(tags, t => t.Name == RealisticRepositoryFixture.LightweightTagName);
	}

	[Fact]
	public void Tags_IncludesAnnotatedTag()
	{
		// Act
		var tags = GitContext.Tags.ToList();

		// Assert
		Assert.Contains(tags, t => t.Name == RealisticRepositoryFixture.AnnotatedTagName);
	}

	[Fact]
	public void GitTag_Name_ReturnsTagName()
	{
		// Act
		var lightweightTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.LightweightTagName);

		// Assert
		Assert.NotNull(lightweightTag);
		Assert.Equal(RealisticRepositoryFixture.LightweightTagName, lightweightTag.Name);
	}

	[Fact]
	public void GitTag_CanonicalName_ReturnsFullRefName()
	{
		// Act
		var lightweightTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.LightweightTagName);

		// Assert
		Assert.NotNull(lightweightTag);
		Assert.Equal($"refs/tags/{RealisticRepositoryFixture.LightweightTagName}", lightweightTag.CanonicalName);
	}

	[Fact]
	public void GitTag_IsAnnotated_FalseForLightweight()
	{
		// Act
		var lightweightTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.LightweightTagName);

		// Assert
		Assert.NotNull(lightweightTag);
		Assert.False(lightweightTag.IsAnnotated);
	}

	[Fact]
	public void GitTag_IsAnnotated_TrueForAnnotated()
	{
		// Act
		var annotatedTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.AnnotatedTagName);

		// Assert
		Assert.NotNull(annotatedTag);
		Assert.True(annotatedTag.IsAnnotated);
	}

	[Fact]
	public void GitTag_Message_NullForLightweight()
	{
		// Act
		var lightweightTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.LightweightTagName);

		// Assert
		Assert.NotNull(lightweightTag);
		Assert.Null(lightweightTag.Message);
	}

	[Fact]
	public void GitTag_Message_PopulatedForAnnotated()
	{
		// Act
		var annotatedTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.AnnotatedTagName);

		// Assert
		Assert.NotNull(annotatedTag);
		Assert.Equal(RealisticRepositoryFixture.AnnotatedTagMessage, annotatedTag.Message?.Trim());
	}

	[Fact]
	public void GitTag_Target_ResolvesToCommit()
	{
		// Act
		var lightweightTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.LightweightTagName);

		// Assert
		Assert.NotNull(lightweightTag);
		Assert.NotNull(lightweightTag.Target);
		Assert.NotNull(lightweightTag.Target.Sha);
	}

	[Fact]
	public void GitTag_LightweightTarget_PointsToInitialCommit()
	{
		// Act
		var lightweightTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.LightweightTagName);

		// Assert
		Assert.NotNull(lightweightTag);
		Assert.Contains("Initial commit", lightweightTag.Target.Message);
	}

	[Fact]
	public void GitTag_AnnotatedTarget_PointsToPostMergeCommit()
	{
		// Act
		var annotatedTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.AnnotatedTagName);

		// Assert
		Assert.NotNull(annotatedTag);
		Assert.Contains("README", annotatedTag.Target.Message);
	}

	[Fact]
	public void GitTag_Tagger_NullForLightweight()
	{
		// Act
		var lightweightTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.LightweightTagName);

		// Assert
		Assert.NotNull(lightweightTag);
		Assert.Null(lightweightTag.Tagger);
	}

	[Fact]
	public void GitTag_Tagger_PopulatedForAnnotated()
	{
		// Act
		var annotatedTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.AnnotatedTagName);

		// Assert
		Assert.NotNull(annotatedTag);
		Assert.Equal(RealisticRepositoryFixture.AuthorName, annotatedTag.Tagger);
	}

	[Fact]
	public void GitTag_TaggerEmail_PopulatedForAnnotated()
	{
		// Act
		var annotatedTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.AnnotatedTagName);

		// Assert
		Assert.NotNull(annotatedTag);
		Assert.Equal(RealisticRepositoryFixture.AuthorEmail, annotatedTag.TaggerEmail);
	}

	[Fact]
	public void GitTag_TaggerDate_NullForLightweight()
	{
		// Act
		var lightweightTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.LightweightTagName);

		// Assert
		Assert.NotNull(lightweightTag);
		Assert.Null(lightweightTag.TaggerDate);
	}

	[Fact]
	public void GitTag_TaggerDate_PopulatedForAnnotated()
	{
		// Act
		var annotatedTag = GitContext.Tags.FirstOrDefault(t => t.Name == RealisticRepositoryFixture.AnnotatedTagName);

		// Assert
		Assert.NotNull(annotatedTag);
		Assert.NotNull(annotatedTag.TaggerDate);
	}

	[Fact]
	public void GitTag_ToString_ReturnsName()
	{
		// Act
		var tag = GitContext.Tags.First();

		// Assert
		Assert.Equal(tag.Name, tag.ToString());
	}
}

/// <summary>
/// Tests for tag creation scenarios with per-test fixtures.
/// </summary>
[Collection("GitContext")]
public class TagMutationTests
{
	[Fact]
	public void Tags_EmptyRepo_ReturnsEmpty()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		// No tags created
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var tags = GitContext.Tags.ToList();

		// Assert
		Assert.Empty(tags);
	}

	[Fact]
	public void Tags_NewlyCreatedLightweight_AppearsInList()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateLightweightTag("test-tag");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var tags = GitContext.Tags.ToList();

		// Assert
		Assert.Single(tags);
		Assert.Equal("test-tag", tags.First().Name);
		Assert.False(tags.First().IsAnnotated);
	}

	[Fact]
	public void Tags_NewlyCreatedAnnotated_AppearsInList()
	{
		// Arrange
		using var fixture = new TestRepositoryFixture();
		fixture.CreateCommit("file.txt", "content", "Initial commit");
		fixture.CreateAnnotatedTag("release-1.0", "First release");
		GitContext.Initialize(fixture.RepositoryPath);

		// Act
		var tags = GitContext.Tags.ToList();

		// Assert
		Assert.Single(tags);
		Assert.Equal("release-1.0", tags.First().Name);
		Assert.True(tags.First().IsAnnotated);
		Assert.Equal("First release", tags.First().Message?.Trim());
	}
}
