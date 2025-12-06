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

    /// <summary>
    /// Represents a file in the working directory with its status
    /// </summary>
    public class GitStatusEntry
    {
        private readonly StatusEntry _entry;
        private static Repository _repository;
        private static string _beyondComparePath;
        private static string _repositoryPath;

        public GitStatusEntry(StatusEntry entry)
        {
            _entry = entry;
        }

        internal static void SetRepository(Repository repo)
        {
            _repository = repo;
            _repositoryPath = repo?.Info.WorkingDirectory;
        }

        internal static void SetBeyondComparePath(string beyondComparePath)
        {
            _beyondComparePath = beyondComparePath;
        }

        /// <summary>
        /// File path relative to repository root
        /// </summary>
        public string FilePath => _entry.FilePath;

        /// <summary>
        /// Status in the index (staging area)
        /// </summary>
        public string IndexStatus => _entry.State.HasFlag(FileStatus.NewInIndex) ? "Added" :
                                      _entry.State.HasFlag(FileStatus.ModifiedInIndex) ? "Modified" :
                                      _entry.State.HasFlag(FileStatus.DeletedFromIndex) ? "Deleted" :
                                      _entry.State.HasFlag(FileStatus.RenamedInIndex) ? "Renamed" :
                                      _entry.State.HasFlag(FileStatus.TypeChangeInIndex) ? "TypeChange" :
                                      "Unmodified";

        /// <summary>
        /// Status in the working directory
        /// </summary>
        public string WorkDirStatus => _entry.State.HasFlag(FileStatus.NewInWorkdir) ? "Untracked" :
                                        _entry.State.HasFlag(FileStatus.ModifiedInWorkdir) ? "Modified" :
                                        _entry.State.HasFlag(FileStatus.DeletedFromWorkdir) ? "Deleted" :
                                        _entry.State.HasFlag(FileStatus.RenamedInWorkdir) ? "Renamed" :
                                        _entry.State.HasFlag(FileStatus.TypeChangeInWorkdir) ? "TypeChange" :
                                        "Unmodified";

        /// <summary>
        /// Combined status description
        /// </summary>
        public string Status
        {
            get
            {
                var parts = new List<string>();
                if (IndexStatus != "Unmodified") parts.Add($"Index: {IndexStatus}");
                if (WorkDirStatus != "Unmodified") parts.Add($"WorkDir: {WorkDirStatus}");
                return parts.Any() ? string.Join(", ", parts) : "Unmodified";
            }
        }

        /// <summary>
        /// Is this file staged (in the index)?
        /// </summary>
        public bool IsStaged => IndexStatus != "Unmodified";

        /// <summary>
        /// Does this file have unstaged changes?
        /// </summary>
        public bool HasUnstagedChanges => WorkDirStatus != "Unmodified" && WorkDirStatus != "Untracked";

        /// <summary>
        /// Is this file untracked?
        /// </summary>
        public bool IsUntracked => _entry.State.HasFlag(FileStatus.NewInWorkdir);

        /// <summary>
        /// Is this file ignored?
        /// </summary>
        public bool IsIgnored => _entry.State.HasFlag(FileStatus.Ignored);

        /// <summary>
        /// Is this file conflicted?
        /// </summary>
        public bool IsConflicted => _entry.State.HasFlag(FileStatus.Conflicted);

        /// <summary>
        /// Button to stage this file
        /// </summary>
        public object cmd_Stage
        {
            get
            {
                if (IsStaged || _repository == null) return null;

                var button = new LINQPad.Controls.Button("Stage", _ =>
                {
                    try
                    {
                        Commands.Stage(_repository, FilePath);
                        $"✓ Staged: {FilePath}".Dump();
                    }
                    catch (Exception ex)
                    {
                        $"Error staging {FilePath}: {ex.Message}".Dump();
                    }
                });
                return button;
            }
        }

        /// <summary>
        /// Button to unstage this file
        /// </summary>
        public object cmd_Unstage
        {
            get
            {
                if (!IsStaged || _repository == null) return null;

                var button = new LINQPad.Controls.Button("Unstage", _ =>
                {
                    try
                    {
                        Commands.Unstage(_repository, FilePath);
                        $"✓ Unstaged: {FilePath}".Dump();
                    }
                    catch (Exception ex)
                    {
                        $"Error unstaging {FilePath}: {ex.Message}".Dump();
                    }
                });
                return button;
            }
        }

        /// <summary>
        /// Button to commit just this file
        /// </summary>
        public object cmd_Commit
        {
            get
            {
                if (_repository == null) return null;

                var button = new LINQPad.Controls.Button("Commit", _ =>
                {
                    try
                    {
                        // Prompt for commit message
                        var message = Util.ReadLine("Commit message:", "");
                        if (string.IsNullOrWhiteSpace(message))
                        {
                            "Commit cancelled - no message provided".Dump();
                            return;
                        }

                        // Stage the file if not already staged
                        if (!IsStaged)
                        {
                            Commands.Stage(_repository, FilePath);
                            $"Staged: {FilePath}".Dump();
                        }

                        // Create signature
                        var signature = _repository.Config.BuildSignature(DateTimeOffset.Now);

                        // Commit
                        var commit = _repository.Commit(message, signature, signature);

                        $"✓ Committed {FilePath}".Dump();
                        new
                        {
                            commit.Sha,
                            ShortSha = commit.Sha.Substring(0, 7),
                            commit.Message,
                            commit.Author
                        }.Dump("Commit Details");
                    }
                    catch (Exception ex)
                    {
                        $"Error committing {FilePath}: {ex.Message}".Dump();
                    }
                });
                return button;
            }
        }

        /// <summary>
        /// Button to discard changes to this file
        /// </summary>
        public object cmd_Discard
        {
            get
            {
                if (!HasUnstagedChanges || _repository == null) return null;

                var button = new LINQPad.Controls.Button("Discard", _ =>
                {
                    try
                    {
                        var confirmed = Util.ReadLine($"Are you sure you want to discard changes to {FilePath}? (yes/no)", "no");
                        if (confirmed?.ToLower() != "yes")
                        {
                            "Discard cancelled".Dump();
                            return;
                        }

                        _repository.CheckoutPaths("HEAD", new[] { FilePath }, new CheckoutOptions
                        {
                            CheckoutModifiers = CheckoutModifiers.Force
                        });

                        $"✓ Discarded changes to: {FilePath}".Dump();
                    }
                    catch (Exception ex)
                    {
                        $"Error discarding changes to {FilePath}: {ex.Message}".Dump();
                    }
                });
                return button;
            }
        }

        /// <summary>
        /// Button to view diff (in Beyond Compare if configured, otherwise in LINQPad)
        /// </summary>
        public object cmd_ViewDiff
        {
            get
            {
                if (_repository == null) return null;

                // Only show for files with changes
                if (!IsStaged && !HasUnstagedChanges && !IsUntracked) return null;

                var button = new LINQPad.Controls.Button("View Diff", _ =>
                {
                    try
                    {
                        string fullFilePath = System.IO.Path.Combine(_repositoryPath, FilePath);

                        // Check if Beyond Compare is configured and available
                        bool useBeyondCompare = !string.IsNullOrWhiteSpace(_beyondComparePath) &&
                                                System.IO.File.Exists(_beyondComparePath);

                        if (IsUntracked)
                        {
                            if (useBeyondCompare)
                            {
                                // For untracked files, open in Beyond Compare
                                System.Diagnostics.Process.Start(_beyondComparePath, $"\"{fullFilePath}\"");
                            }
                            else
                            {
                                // Show file contents in LINQPad
                                var workingContent = System.IO.File.ReadAllText(fullFilePath);
                                new
                                {
                                    File = FilePath,
                                    Status = "Untracked (new file)",
                                    Content = workingContent
                                }.Dump("New File");
                            }
                        }
                        else if (IsStaged || HasUnstagedChanges)
                        {
                            // Get the HEAD version of the file
                            var headCommit = _repository.Head.Tip;
                            var treeEntry = headCommit[FilePath];

                            if (treeEntry == null)
                            {
                                // File is new (added but was not in HEAD)
                                if (useBeyondCompare)
                                {
                                    System.Diagnostics.Process.Start(_beyondComparePath, $"\"{fullFilePath}\"");
                                }
                                else
                                {
                                    var workingContent = System.IO.File.ReadAllText(fullFilePath);
                                    new
                                    {
                                        File = FilePath,
                                        Status = "New file",
                                        Content = workingContent
                                    }.Dump("New File");
                                }
                            }
                            else
                            {
                                // Get HEAD and working versions
                                var blob = (Blob)treeEntry.Target;
                                string headContent;
                                using (var stream = blob.GetContentStream())
                                using (var reader = new System.IO.StreamReader(stream))
                                {
                                    headContent = reader.ReadToEnd();
                                }

                                string workingContent = System.IO.File.ReadAllText(fullFilePath);

                                if (useBeyondCompare)
                                {
                                    // Extract HEAD version to temp file
                                    var tempFile = System.IO.Path.GetTempFileName();
                                    System.IO.File.WriteAllText(tempFile, headContent);

                                    // Launch Beyond Compare with both files
                                    System.Diagnostics.Process.Start(_beyondComparePath, $"\"{tempFile}\" \"{fullFilePath}\" /title1=\"HEAD: {FilePath}\" /title2=\"Working: {FilePath}\"");

                                    $"✓ Opened diff in Beyond Compare".Dump();
                                }
                                else
                                {
                                    // Use LINQPad's built-in diff
                                    Util.Dif(headContent, workingContent).Dump($"Diff: {FilePath} (HEAD vs Working)");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        $"Error opening diff for {FilePath}: {ex.Message}".Dump();
                    }
                });
                return button;
            }
        }

        public override string ToString() => $"{FilePath} ({Status})";
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

    public class GitStash
    {
        private readonly Stash _stash;
        private readonly int _index;
        private static Repository _repository;

        public GitStash(Stash stash, int index)
        {
            _stash = stash;
            _index = index;
        }

        internal static void SetRepository(Repository repo)
        {
            _repository = repo;
        }

        /// <summary>
        /// Index of this stash (0 = most recent)
        /// </summary>
        public int Index => _index;

        /// <summary>
        /// Stash reference name (e.g., stash@{0})
        /// </summary>
        public string Reference => $"stash@{{{Index}}}";

        /// <summary>
        /// Stash message
        /// </summary>
        public string Message => _stash.Message;

        /// <summary>
        /// The commit representing this stash's working tree
        /// </summary>
        public GitCommit WorkTree => new GitCommit(_stash.WorkTree);

        /// <summary>
        /// When the stash was created
        /// </summary>
        public DateTimeOffset When => _stash.WorkTree.Committer.When;

        /// <summary>
        /// Button to apply this stash (keeps the stash)
        /// </summary>
        public object cmd_Apply
        {
            get
            {
                if (_repository == null) return null;

                var button = new LINQPad.Controls.Button("Apply", _ =>
                {
                    try
                    {
                        _repository.Stashes.Apply(Index);
                        $"✓ Applied stash: {Reference}".Dump();
                    }
                    catch (Exception ex)
                    {
                        $"Error applying stash {Reference}: {ex.Message}".Dump();
                    }
                });
                return button;
            }
        }

        /// <summary>
        /// Button to pop this stash (applies and removes)
        /// </summary>
        public object cmd_Pop
        {
            get
            {
                if (_repository == null) return null;

                var button = new LINQPad.Controls.Button("Pop", _ =>
                {
                    try
                    {
                        _repository.Stashes.Pop(Index);
                        $"✓ Popped stash: {Reference}".Dump();
                    }
                    catch (Exception ex)
                    {
                        $"Error popping stash {Reference}: {ex.Message}".Dump();
                    }
                });
                return button;
            }
        }

        /// <summary>
        /// Button to drop this stash (removes without applying)
        /// </summary>
        public object cmd_Drop
        {
            get
            {
                if (_repository == null) return null;

                var button = new LINQPad.Controls.Button("Drop", _ =>
                {
                    try
                    {
                        var confirmed = Util.ReadLine($"Are you sure you want to drop {Reference}? (yes/no)", "no");
                        if (confirmed?.ToLower() != "yes")
                        {
                            "Drop cancelled".Dump();
                            return;
                        }

                        _repository.Stashes.Remove(Index);
                        $"✓ Dropped stash: {Reference}".Dump();
                    }
                    catch (Exception ex)
                    {
                        $"Error dropping stash {Reference}: {ex.Message}".Dump();
                    }
                });
                return button;
            }
        }

        public override string ToString() => $"{Reference}: {Message}";
    }
}