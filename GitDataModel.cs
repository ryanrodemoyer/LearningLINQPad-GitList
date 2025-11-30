using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using LINQPad;

namespace LearningLINQPad.GitList
{
	/// <summary>
	/// The main typed data context that users will interact with in LINQPad
	/// </summary>
	public class GitRepository
	{
		private readonly Repository _repo;

		public GitRepository(string repositoryPath)
		{
			_repo = new Repository(repositoryPath);
			RepositoryPath = repositoryPath;
		}

		public string RepositoryPath { get; }

		/// <summary>
		/// All commits in the repository (from all branches)
		/// </summary>
		public IEnumerable<GitCommit> Commits => 
			_repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = _repo.Refs })
				.Select(c => new GitCommit(c));

		/// <summary>
		/// All branches (local and remote)
		/// </summary>
		public IEnumerable<GitBranch> Branches => 
			_repo.Branches.Select(b => new GitBranch(b));

		/// <summary>
		/// Local branches only
		/// </summary>
		public IEnumerable<GitBranch> LocalBranches => 
			_repo.Branches.Where(b => !b.IsRemote).Select(b => new GitBranch(b));

		/// <summary>
		/// Remote branches only
		/// </summary>
		public IEnumerable<GitBranch> RemoteBranches => 
			_repo.Branches.Where(b => b.IsRemote).Select(b => new GitBranch(b));

		/// <summary>
		/// All tags in the repository
		/// </summary>
		public IEnumerable<GitTag> Tags => 
			_repo.Tags.Select(t => new GitTag(t));

		/// <summary>
		/// All remotes configured for this repository
		/// </summary>
		public IEnumerable<GitRemote> Remotes => 
			_repo.Network.Remotes.Select(r => new GitRemote(r));

		/// <summary>
		/// Current HEAD reference
		/// </summary>
		public GitBranch Head => new GitBranch(_repo.Head);

		/// <summary>
		/// Repository configuration
		/// </summary>
		public Configuration Config => _repo.Config;

		public void Dispose()
		{
			_repo?.Dispose();
		}
	}

	public class GitCommit
	{
		private readonly Commit _commit;

		public GitCommit(Commit commit)
		{
			_commit = commit;
		}

		public string Sha => _commit.Sha;
		public string ShortSha => _commit.Sha.Substring(0, 7);
		public string Message => _commit.Message;
		public string MessageShort => _commit.MessageShort;
		public string Author => _commit.Author.Name;
		public string AuthorEmail => _commit.Author.Email;
		public DateTimeOffset AuthorDate => _commit.Author.When;
		public string Committer => _commit.Committer.Name;
		public string CommitterEmail => _commit.Committer.Email;
		public DateTimeOffset CommitDate => _commit.Committer.When;
		
		public IEnumerable<GitCommit> Parents => _commit.Parents.Select(p => new GitCommit(p));
		public int ParentCount => _commit.Parents.Count();
		
		/// <summary>
		/// Tree SHA for this commit
		/// </summary>
		public string TreeSha => _commit.Tree.Sha;

		/// <summary>
		/// Get files in this commit's tree
		/// </summary>
		public IEnumerable<GitTreeEntry> TreeEntries => _commit.Tree.Select(e => new GitTreeEntry(e));

		public override string ToString() => $"{ShortSha} {MessageShort}";
	}

	public class GitTreeEntry
	{
		private readonly TreeEntry _entry;

		public GitTreeEntry(TreeEntry entry)
		{
			_entry = entry;
		}

		public string Path => _entry.Path;
		public string Name => _entry.Name;
		public string Sha => _entry.Target.Sha;
		public string TargetType => _entry.TargetType.ToString();
		public int Mode => (int)_entry.Mode;

		public override string ToString() => Path;
	}

	public class GitBranch
	{
		private readonly Branch _branch;

		public GitBranch(Branch branch)
		{
			_branch = branch;
		}

		public string Name => _branch.FriendlyName;
		public string CanonicalName => _branch.CanonicalName;
		public bool IsRemote => _branch.IsRemote;
		public bool IsCurrentRepositoryHead => _branch.IsCurrentRepositoryHead;
		public bool IsTracking => _branch.IsTracking;
		
		public string RemoteName => _branch.RemoteName;
		public string UpstreamBranchCanonicalName => _branch.UpstreamBranchCanonicalName;
		
		/// <summary>
		/// The commit this branch points to
		/// </summary>
		public GitCommit Tip => _branch.Tip != null ? new GitCommit(_branch.Tip) : null;

		/// <summary>
		/// All commits reachable from this branch
		/// </summary>
		public IEnumerable<GitCommit> Commits => _branch.Commits.Select(c => new GitCommit(c));

		/// <summary>
		/// Number of commits ahead of upstream
		/// </summary>
		public int? AheadBy => _branch.TrackingDetails?.AheadBy;

		/// <summary>
		/// Number of commits behind upstream
		/// </summary>
		public int? BehindBy => _branch.TrackingDetails?.BehindBy;

		public override string ToString() => $"{Name}{(IsCurrentRepositoryHead ? " (HEAD)" : "")}";
	}

	public class GitTag
	{
		private readonly Tag _tag;

		public GitTag(Tag tag)
		{
			_tag = tag;
		}

		public string Name => _tag.FriendlyName;
		public string CanonicalName => _tag.CanonicalName;
		public bool IsAnnotated => _tag.IsAnnotated;
		
		/// <summary>
		/// The commit this tag points to
		/// </summary>
		public GitCommit Target => _tag.Target is Commit commit ? new GitCommit(commit) : null;

		/// <summary>
		/// For annotated tags, the tag message
		/// </summary>
		public string Message => _tag.Annotation?.Message;

		/// <summary>
		/// For annotated tags, the tagger information
		/// </summary>
		public string Tagger => _tag.Annotation?.Tagger.Name;
		public string TaggerEmail => _tag.Annotation?.Tagger.Email;
		public DateTimeOffset? TaggerDate => _tag.Annotation?.Tagger.When;

		public override string ToString() => Name;
	}

	public class GitRemote
	{
		private readonly Remote _remote;

		public GitRemote(Remote remote)
		{
			_remote = remote;
		}

		public string Name => _remote.Name;
		public string Url => _remote.Url;
		public string PushUrl => _remote.PushUrl;

		public override string ToString() => $"{Name} ({Url})";
	}
}
