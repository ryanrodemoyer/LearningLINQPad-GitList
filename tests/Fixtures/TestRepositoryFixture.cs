using LibGit2Sharp;

namespace LearningLINQPad.GitList.Tests.Fixtures;

/// <summary>
/// Base fixture for creating temporary Git repositories for testing.
/// Creates a fresh repository in a temp directory with helper methods for common git operations.
/// </summary>
public class TestRepositoryFixture : IDisposable
{
	private bool _disposed;

	/// <summary>
	/// Path to the temporary repository directory.
	/// </summary>
	public string RepositoryPath { get; }

	/// <summary>
	/// The LibGit2Sharp Repository instance.
	/// </summary>
	public Repository Repository { get; }

	/// <summary>
	/// Default signature used for commits.
	/// </summary>
	public Signature DefaultSignature { get; }

	public TestRepositoryFixture()
	{
		RepositoryPath = Path.Combine(Path.GetTempPath(), $"git-test-{Guid.NewGuid():N}");
		Directory.CreateDirectory(RepositoryPath);
		Repository.Init(RepositoryPath);
		Repository = new Repository(RepositoryPath);
		DefaultSignature = new Signature("Test User", "test@example.com", DateTimeOffset.Now);
	}

	/// <summary>
	/// Creates a file and commits it to the repository.
	/// </summary>
	/// <param name="filename">Relative path to the file.</param>
	/// <param name="content">File content.</param>
	/// <param name="message">Commit message.</param>
	/// <param name="authorName">Optional author name (defaults to Test User).</param>
	/// <param name="authorEmail">Optional author email (defaults to test@example.com).</param>
	/// <returns>The created commit.</returns>
	public Commit CreateCommit(string filename, string content, string message, string? authorName = null, string? authorEmail = null)
	{
		var filePath = Path.Combine(RepositoryPath, filename);
		var directory = Path.GetDirectoryName(filePath);
		if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllText(filePath, content);
		Commands.Stage(Repository, filename);

		var author = authorName != null || authorEmail != null
			? new Signature(authorName ?? "Test User", authorEmail ?? "test@example.com", DateTimeOffset.Now)
			: DefaultSignature;

		return Repository.Commit(message, author, author);
	}

	/// <summary>
	/// Creates a file without staging or committing it.
	/// </summary>
	/// <param name="filename">Relative path to the file.</param>
	/// <param name="content">File content.</param>
	public void CreateFile(string filename, string content)
	{
		var filePath = Path.Combine(RepositoryPath, filename);
		var directory = Path.GetDirectoryName(filePath);
		if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllText(filePath, content);
	}

	/// <summary>
	/// Modifies an existing file without staging it.
	/// </summary>
	/// <param name="filename">Relative path to the file.</param>
	/// <param name="content">New file content.</param>
	public void ModifyFile(string filename, string content)
	{
		var filePath = Path.Combine(RepositoryPath, filename);
		File.WriteAllText(filePath, content);
	}

	/// <summary>
	/// Deletes a file from the working directory without staging.
	/// </summary>
	/// <param name="filename">Relative path to the file.</param>
	public void DeleteFile(string filename)
	{
		var filePath = Path.Combine(RepositoryPath, filename);
		File.Delete(filePath);
	}

	/// <summary>
	/// Stages a file for commit.
	/// </summary>
	/// <param name="filename">Relative path to the file.</param>
	public void StageFile(string filename)
	{
		Commands.Stage(Repository, filename);
	}

	/// <summary>
	/// Unstages a file.
	/// </summary>
	/// <param name="filename">Relative path to the file.</param>
	public void UnstageFile(string filename)
	{
		Commands.Unstage(Repository, filename);
	}

	/// <summary>
	/// Creates a new branch at the current HEAD.
	/// </summary>
	/// <param name="name">Branch name.</param>
	/// <returns>The created branch.</returns>
	public Branch CreateBranch(string name)
	{
		return Repository.CreateBranch(name);
	}

	/// <summary>
	/// Creates a new branch and checks it out.
	/// </summary>
	/// <param name="name">Branch name.</param>
	/// <returns>The created and checked out branch.</returns>
	public Branch CreateAndCheckoutBranch(string name)
	{
		var branch = Repository.CreateBranch(name);
		Commands.Checkout(Repository, branch);
		return branch;
	}

	/// <summary>
	/// Checks out an existing branch.
	/// </summary>
	/// <param name="name">Branch name.</param>
	public void Checkout(string name)
	{
		Commands.Checkout(Repository, Repository.Branches[name]);
	}

	/// <summary>
	/// Creates a lightweight tag.
	/// </summary>
	/// <param name="name">Tag name.</param>
	/// <param name="target">Optional target commit (defaults to HEAD).</param>
	/// <returns>The created tag.</returns>
	public Tag CreateLightweightTag(string name, Commit? target = null)
	{
		return Repository.Tags.Add(name, target ?? Repository.Head.Tip);
	}

	/// <summary>
	/// Creates an annotated tag.
	/// </summary>
	/// <param name="name">Tag name.</param>
	/// <param name="message">Tag message.</param>
	/// <param name="target">Optional target commit (defaults to HEAD).</param>
	/// <returns>The created tag.</returns>
	public Tag CreateAnnotatedTag(string name, string message, Commit? target = null)
	{
		return Repository.Tags.Add(name, target ?? Repository.Head.Tip, DefaultSignature, message);
	}

	/// <summary>
	/// Adds a remote to the repository.
	/// </summary>
	/// <param name="name">Remote name.</param>
	/// <param name="url">Remote URL.</param>
	/// <returns>The created remote.</returns>
	public Remote AddRemote(string name, string url)
	{
		return Repository.Network.Remotes.Add(name, url);
	}

	/// <summary>
	/// Creates a stash from current changes.
	/// </summary>
	/// <param name="message">Stash message.</param>
	/// <returns>The created stash.</returns>
	public Stash CreateStash(string message)
	{
		return Repository.Stashes.Add(DefaultSignature, message);
	}

	/// <summary>
	/// Creates a .gitignore file with the specified patterns.
	/// </summary>
	/// <param name="patterns">Patterns to ignore, one per line.</param>
	public void CreateGitIgnore(params string[] patterns)
	{
		var content = string.Join(Environment.NewLine, patterns);
		File.WriteAllText(Path.Combine(RepositoryPath, ".gitignore"), content);
		Commands.Stage(Repository, ".gitignore");
		Repository.Commit("Add .gitignore", DefaultSignature, DefaultSignature);
	}

	/// <summary>
	/// Merges a branch into the current branch.
	/// </summary>
	/// <param name="branchName">Branch to merge.</param>
	/// <returns>The merge result.</returns>
	public MergeResult Merge(string branchName)
	{
		var branch = Repository.Branches[branchName];
		return Repository.Merge(branch, DefaultSignature);
	}

	/// <summary>
	/// Sets up a merge conflict scenario.
	/// Creates two branches that modify the same file differently.
	/// </summary>
	/// <returns>Tuple of (conflicting file path, main branch name, feature branch name).</returns>
	public (string FilePath, string MainBranch, string FeatureBranch) SetupMergeConflict()
	{
		const string filename = "conflict.txt";
		const string mainBranch = "main";
		const string featureBranch = "feature-conflict";

		// Initial commit on main
		CreateCommit(filename, "Line 1\nLine 2\nLine 3\n", "Initial commit");

		// Create and rename default branch to main
		var main = Repository.Branches["master"] ?? Repository.Branches[mainBranch];
		if (main.FriendlyName == "master")
		{
			Repository.Branches.Rename("master", mainBranch);
		}

		// Create feature branch and modify file
		CreateAndCheckoutBranch(featureBranch);
		ModifyFile(filename, "Line 1\nFeature change on Line 2\nLine 3\n");
		StageFile(filename);
		Repository.Commit("Feature change", DefaultSignature, DefaultSignature);

		// Go back to main and make conflicting change
		Checkout(mainBranch);
		ModifyFile(filename, "Line 1\nMain change on Line 2\nLine 3\n");
		StageFile(filename);
		Repository.Commit("Main change", DefaultSignature, DefaultSignature);

		// Attempt merge (will create conflict)
		try
		{
			Merge(featureBranch);
		}
		catch
		{
			// Merge conflict expected
		}

		return (filename, mainBranch, featureBranch);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				Repository.Dispose();
			}

			// Clean up temp directory
			try
			{
				if (Directory.Exists(RepositoryPath))
				{
					// Remove read-only attributes from .git directory files
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
