using LibGit2Sharp;

namespace LearningLINQPad.GitList.Tests.Fixtures;

/// <summary>
/// A shared fixture that creates a realistic Git repository with:
/// - Multiple commits across multiple branches
/// - Lightweight and annotated tags
/// - A remote configuration
/// - A merge commit
/// 
/// This fixture is intended for read-only tests and should not be modified during tests.
/// Use IClassFixture&lt;RealisticRepositoryFixture&gt; to share across tests in a class.
/// </summary>
public class RealisticRepositoryFixture : IDisposable
{
	private bool _disposed;

	public string RepositoryPath { get; }
	public Repository Repository { get; }
	public Signature DefaultSignature { get; }

	// Known commit references for assertions
	public Commit InitialCommit { get; private set; } = null!;
	public Commit SecondCommit { get; private set; } = null!;
	public Commit FeatureCommit { get; private set; } = null!;
	public Commit MergeCommit { get; private set; } = null!;
	public Commit PostMergeCommit { get; private set; } = null!;

	// Known branch names
	public const string MainBranchName = "main";
	public const string FeatureBranchName = "feature/add-widgets";
	public const string DevelopBranchName = "develop";

	// Known tag names
	public const string LightweightTagName = "v0.1.0";
	public const string AnnotatedTagName = "v1.0.0";
	public const string AnnotatedTagMessage = "Release version 1.0.0";

	// Known remote
	public const string RemoteName = "origin";
	public const string RemoteUrl = "https://github.com/example/repo.git";

	// Known author info
	public const string AuthorName = "Test Author";
	public const string AuthorEmail = "author@example.com";
	public const string CommitterName = "Test Committer";
	public const string CommitterEmail = "committer@example.com";

	public RealisticRepositoryFixture()
	{
		RepositoryPath = Path.Combine(Path.GetTempPath(), $"git-realistic-{Guid.NewGuid():N}");
		Directory.CreateDirectory(RepositoryPath);
		Repository.Init(RepositoryPath);
		Repository = new Repository(RepositoryPath);
		DefaultSignature = new Signature(AuthorName, AuthorEmail, DateTimeOffset.Now);

		BuildRealisticHistory();
	}

	private void BuildRealisticHistory()
	{
		var committerSig = new Signature(CommitterName, CommitterEmail, DateTimeOffset.Now);

		// Initial commit with README
		CreateFileInternal("README.md", "# Test Repository\n\nThis is a test repository.");
		Commands.Stage(Repository, "README.md");
		InitialCommit = Repository.Commit("Initial commit", DefaultSignature, committerSig);

		// Rename master to main
		if (Repository.Branches["master"] != null)
		{
			Repository.Branches.Rename("master", MainBranchName);
		}

		// Create lightweight tag on initial commit
		Repository.Tags.Add(LightweightTagName, InitialCommit);

		// Second commit - add more files
		CreateFileInternal("src/app.cs", "namespace App { class Program { static void Main() { } } }");
		CreateFileInternal("src/utils.cs", "namespace App { static class Utils { } }");
		Commands.Stage(Repository, "src/app.cs");
		Commands.Stage(Repository, "src/utils.cs");
		SecondCommit = Repository.Commit("Add application files", DefaultSignature, committerSig);

		// Create develop branch
		Repository.CreateBranch(DevelopBranchName);

		// Create feature branch from SecondCommit
		var featureBranch = Repository.CreateBranch(FeatureBranchName);
		Commands.Checkout(Repository, featureBranch);

		CreateFileInternal("src/widgets.cs", "namespace App { class Widget { public string Name { get; set; } } }");
		Commands.Stage(Repository, "src/widgets.cs");
		FeatureCommit = Repository.Commit("Add widget class", DefaultSignature, committerSig);

		// Go back to main and add a commit there to force a real merge (not fast-forward)
		Commands.Checkout(Repository, Repository.Branches[MainBranchName]);
		CreateFileInternal("docs/readme.txt", "Documentation placeholder");
		Commands.Stage(Repository, "docs/readme.txt");
		Repository.Commit("Add documentation placeholder", DefaultSignature, committerSig);

		// Now merge feature branch - this will create an actual merge commit
		var mergeResult = Repository.Merge(featureBranch, DefaultSignature, new MergeOptions());
		MergeCommit = mergeResult.Commit;

		// Post-merge commit
		ModifyFileInternal("README.md", "# Test Repository\n\nThis is a test repository.\n\n## Features\n- Widgets");
		Commands.Stage(Repository, "README.md");
		PostMergeCommit = Repository.Commit("Update README with features", DefaultSignature, committerSig);

		// Create annotated tag on the latest commit
		Repository.Tags.Add(AnnotatedTagName, PostMergeCommit, DefaultSignature, AnnotatedTagMessage);

		// Add remote
		Repository.Network.Remotes.Add(RemoteName, RemoteUrl);
	}

	private void CreateFileInternal(string relativePath, string content)
	{
		var fullPath = Path.Combine(RepositoryPath, relativePath);
		var directory = Path.GetDirectoryName(fullPath);
		if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
		File.WriteAllText(fullPath, content);
	}

	private void ModifyFileInternal(string relativePath, string content)
	{
		var fullPath = Path.Combine(RepositoryPath, relativePath);
		File.WriteAllText(fullPath, content);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				Repository.Dispose();
			}

			try
			{
				if (Directory.Exists(RepositoryPath))
				{
					var gitDir = Path.Combine(RepositoryPath, ".git");
					if (Directory.Exists(gitDir))
					{
						foreach (var file in Directory.GetFiles(gitDir, "*", SearchOption.AllDirectories))
						{
							File.SetAttributes(file, FileAttributes.Normal);
						}
					}
					Directory.Delete(RepositoryPath, recursive: true);
				}
			}
			catch
			{
				// Best effort cleanup
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

/// <summary>
/// Collection definition for tests that share the GitContext static state.
/// All tests in this collection run serially.
/// </summary>
[CollectionDefinition("GitContext")]
public class GitContextCollection : ICollectionFixture<RealisticRepositoryFixture>
{
}
