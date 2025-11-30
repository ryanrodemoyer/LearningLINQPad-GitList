using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using LINQPad;

namespace LearningLINQPad.GitList
{
	/// <summary>
	/// Static typed data context for LINQPad
	/// </summary>
	public class GitContext
	{
		private static Repository _repo;
		private static string _repositoryPath;

		static GitContext()
		{
		}

		public static void Initialize(string repositoryPath)
		{
			if (_repositoryPath != repositoryPath)
			{
				_repo?.Dispose();
				_repo = new Repository(repositoryPath);
				_repositoryPath = repositoryPath;
			}
		}

		public static string RepositoryPath => _repositoryPath;

		/// <summary>
		/// All commits in the repository (from all branches)
		/// </summary>
		public static IEnumerable<GitCommit> Commits
		{
			get
			{
				if (_repo == null) return Enumerable.Empty<GitCommit>();
				return _repo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = _repo.Refs })
					.Select(c => new GitCommit(c));
			}
		}

		/// <summary>
		/// All branches (local and remote)
		/// </summary>
		public static IEnumerable<GitBranch> Branches
		{
			get
			{
				if (_repo == null) return Enumerable.Empty<GitBranch>();
				return _repo.Branches.Select(b => new GitBranch(b));
			}
		}

		/// <summary>
		/// Local branches only
		/// </summary>
		public static IEnumerable<GitBranch> LocalBranches
		{
			get
			{
				if (_repo == null) return Enumerable.Empty<GitBranch>();
				return _repo.Branches.Where(b => !b.IsRemote).Select(b => new GitBranch(b));
			}
		}

		/// <summary>
		/// Remote branches only
		/// </summary>
		public static IEnumerable<GitBranch> RemoteBranches
		{
			get
			{
				if (_repo == null) return Enumerable.Empty<GitBranch>();
				return _repo.Branches.Where(b => b.IsRemote).Select(b => new GitBranch(b));
			}
		}

		/// <summary>
		/// All tags in the repository
		/// </summary>
		public static IEnumerable<GitTag> Tags
		{
			get
			{
				if (_repo == null) return Enumerable.Empty<GitTag>();
				return _repo.Tags.Select(t => new GitTag(t));
			}
		}

		/// <summary>
		/// All remotes configured for this repository
		/// </summary>
		public static IEnumerable<GitRemote> Remotes
		{
			get
			{
				if (_repo == null) return Enumerable.Empty<GitRemote>();
				return _repo.Network.Remotes.Select(r => new GitRemote(r));
			}
		}

		/// <summary>
		/// Current HEAD reference
		/// </summary>
		public static GitBranch Head
		{
			get
			{
				if (_repo == null) return null;
				return new GitBranch(_repo.Head);
			}
		}

		/// <summary>
		/// Repository configuration
		/// </summary>
		public static Configuration Config
		{
			get
			{
				if (_repo == null) return null;
				return _repo.Config;
			}
		}
	}
}
