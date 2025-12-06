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
    public static class GitContext
    {
        private static Repository _repo;
        private static string _repositoryPath;

        public static string RepositoryPath => _repositoryPath;

        public static void Initialize(string repositoryPath, string beyondComparePath = null)
        {
            if (_repositoryPath != repositoryPath)
            {
                _repo?.Dispose();
                _repo = new Repository(repositoryPath);
                _repositoryPath = repositoryPath;

                // Set the repository reference for GitStatusEntry and GitStash so buttons can perform git operations
                GitStatusEntry.SetRepository(_repo);
                GitStash.SetRepository(_repo);
            }

            // Always update Beyond Compare path (in case it changed)
            GitStatusEntry.SetBeyondComparePath(beyondComparePath);
        }


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
                if (_repo == null)
                    return Enumerable.Empty<GitBranch>();
                return _repo.Branches.Select(b => new GitBranch(b));
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
        /// All stashes in the repository
        /// </summary>
        public static IEnumerable<GitStash> Stashes
        {
            get
            {
                if (_repo == null) return Enumerable.Empty<GitStash>();
                return _repo.Stashes.Select((s, index) => new GitStash(s, index));
            }
        }

        /// <summary>
        /// Current HEAD reference
        /// </summary>
        public static GitBranch Head
        {
            get
            {
                return _repo == null ? null : new GitBranch(_repo.Head);
            }
        }

        /// <summary>
        /// Repository configuration
        /// </summary>
        public static Configuration Config
        {
            get
            {
                return _repo?.Config;
            }
        }

        /// <summary>
        /// All files with status (modified, staged, untracked, etc.) - excludes ignored files
        /// </summary>
        public static IEnumerable<GitStatusEntry> Status
        {
            get
            {
                if (_repo == null) return Enumerable.Empty<GitStatusEntry>();
                var status = _repo.RetrieveStatus(new StatusOptions());
                return status.Where(s => !s.State.HasFlag(FileStatus.Ignored)).Select(s => new GitStatusEntry(s));
            }
        }

        /// <summary>
        /// Files that are staged (in the index, ready to commit)
        /// </summary>
        public static IEnumerable<GitStatusEntry> Staged
        {
            get
            {
                if (_repo == null) return Enumerable.Empty<GitStatusEntry>();
                var status = _repo.RetrieveStatus(new StatusOptions());
                return status
                    .Where(s => s.State.HasFlag(FileStatus.NewInIndex) ||
                                s.State.HasFlag(FileStatus.ModifiedInIndex) ||
                                s.State.HasFlag(FileStatus.DeletedFromIndex) ||
                                s.State.HasFlag(FileStatus.RenamedInIndex) ||
                                s.State.HasFlag(FileStatus.TypeChangeInIndex))
                    .Select(s => new GitStatusEntry(s));
            }
        }

        /// <summary>
        /// Files with unstaged changes (modified in working directory but not staged)
        /// </summary>
        public static IEnumerable<GitStatusEntry> Unstaged
        {
            get
            {
                if (_repo == null) return Enumerable.Empty<GitStatusEntry>();
                var status = _repo.RetrieveStatus(new StatusOptions());
                return status
                    .Where(s => (s.State.HasFlag(FileStatus.ModifiedInWorkdir) ||
                                 s.State.HasFlag(FileStatus.DeletedFromWorkdir) ||
                                 s.State.HasFlag(FileStatus.RenamedInWorkdir) ||
                                 s.State.HasFlag(FileStatus.TypeChangeInWorkdir)) &&
                                !s.State.HasFlag(FileStatus.NewInWorkdir))
                    .Select(s => new GitStatusEntry(s));
            }
        }

        /// <summary>
        /// Files that are untracked (not in git yet)
        /// </summary>
        public static IEnumerable<GitStatusEntry> Untracked
        {
            get
            {
                if (_repo == null) return Enumerable.Empty<GitStatusEntry>();
                var status = _repo.RetrieveStatus(new StatusOptions());
                return status
                    .Where(s => s.State.HasFlag(FileStatus.NewInWorkdir))
                    .Select(s => new GitStatusEntry(s));
            }
        }

        /// <summary>
        /// Files that are ignored
        /// </summary>
        public static IEnumerable<GitStatusEntry> Ignored
        {
            get
            {
                if (_repo == null) return Enumerable.Empty<GitStatusEntry>();
                var status = _repo.RetrieveStatus(new StatusOptions
                {
                    Show = StatusShowOption.IndexAndWorkDir, IncludeIgnored = true
                });
                return status
                    .Where(s => s.State.HasFlag(FileStatus.Ignored))
                    .Select(s => new GitStatusEntry(s));
            }
        }

        /// <summary>
        /// Files with merge conflicts
        /// </summary>
        public static IEnumerable<GitStatusEntry> Conflicted
        {
            get
            {
                if (_repo == null) return Enumerable.Empty<GitStatusEntry>();
                var status = _repo.RetrieveStatus(new StatusOptions());
                return status
                    .Where(s => s.State.HasFlag(FileStatus.Conflicted))
                    .Select(s => new GitStatusEntry(s));
            }
        }

        /// <summary>
        /// Is the working directory clean (no changes)?
        /// </summary>
        public static bool IsClean
        {
            get => _repo?.RetrieveStatus().IsDirty == false;
        }

        /// <summary>
        /// Number of files with changes (excludes ignored files)
        /// </summary>
        public static int ChangedFilesCount
        {
            get
            {
                if (_repo == null) return 0;
                return _repo.RetrieveStatus().Count(s => !s.State.HasFlag(FileStatus.Ignored));
            }
        }
    }
}