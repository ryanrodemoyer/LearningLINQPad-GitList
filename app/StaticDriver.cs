using System;
using System.Collections.Generic;
using System.IO;

using LINQPad.Extensibility.DataContext;

namespace LearningLINQPad.GitList
{
    public class StaticDriver : StaticDataContextDriver
    {
        static StaticDriver()
        {
            // Uncomment the following code to attach to Visual Studio's debugger when an exception is thrown:
            //AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
            //{
            //	if (args.Exception.StackTrace.Contains ("LearningLINQPad.GitList"))
            //		Debugger.Launch ();
            //};
        }

        public override string Name => "LearningLINQPad.GitList";

        public override string Author => "Ryan Rodemoyer @ LearningLINQPad.com";

        public override string GetConnectionDescription(IConnectionInfo cxInfo)
        {
            var props = new ConnectionProperties(cxInfo);
            if (!string.IsNullOrWhiteSpace(props.RepositoryPath))
            {
                string repoName = Path.GetFileName(props.RepositoryPath);
                return $"Git: {repoName}";
            }
            return "Git Repository";
        }

        public override bool ShowConnectionDialog(IConnectionInfo cxInfo, ConnectionDialogOptions dialogOptions)
            => new ConnectionDialog(cxInfo).ShowDialog() == true;

        public override List<ExplorerItem> GetSchema(IConnectionInfo cxInfo, Type customType)
        {
            var props = new ConnectionProperties(cxInfo);

            if (string.IsNullOrWhiteSpace(props.RepositoryPath))
            {
                return new List<ExplorerItem>();
            }

            try
            {
                using var re3po = new LibGit2Sharp.Repository(props.RepositoryPath);
            }
            catch (Exception ex)
            {
                // If we can't open the repository, return an empty schema
                return new List<ExplorerItem>
                {
                    new ExplorerItem($"Error: {ex.Message}", ExplorerItemKind.Property, ExplorerIcon.Box)
                };
            }

            var schema = new List<ExplorerItem>
            {
                // Add Commits collection
                new ExplorerItem("Commits", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                {
                    IsEnumerable = true,
                    ToolTipText = "All commits in the repository",
                    DragText = "Commits",
                    Children =
                        [
                            new ExplorerItem("Sha", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("ShortSha", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Message", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("MessageShort", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Author", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("AuthorEmail", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("AuthorDate", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Committer", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("CommitterEmail", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("CommitDate", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("ParentCount", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("TreeSha", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Parents", ExplorerItemKind.CollectionLink, ExplorerIcon.OneToMany),
                            new ExplorerItem("TreeEntries", ExplorerItemKind.CollectionLink, ExplorerIcon.OneToMany)
                        ]
                },

                // Add Branches collection
                new ExplorerItem("Branches", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                {
                    IsEnumerable = true,
                    ToolTipText = "All branches (local and remote)",
                    DragText = "Branches",
                    Children =
                        [
                            new ExplorerItem("Name", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("CanonicalName", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IsRemote", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IsCurrentRepositoryHead", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IsTracking", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("RemoteName", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("AheadBy", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("BehindBy", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Tip", ExplorerItemKind.ReferenceLink, ExplorerIcon.ManyToOne),
                            new ExplorerItem("Commits", ExplorerItemKind.CollectionLink, ExplorerIcon.OneToMany)
                        ]
                },

                // Add Tags collection
                new ExplorerItem("Tags", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                {
                    IsEnumerable = true,
                    ToolTipText = "All tags in the repository",
                    DragText = "Tags",
                    Children =
                        [
                            new ExplorerItem("Name", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("CanonicalName", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IsAnnotated", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Message", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Tagger", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("TaggerEmail", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("TaggerDate", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Target", ExplorerItemKind.ReferenceLink, ExplorerIcon.ManyToOne)
                        ]
                },

                // Add Remotes collection
                new ExplorerItem("Remotes", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                {
                    IsEnumerable = true,
                    ToolTipText = "Configured remotes",
                    DragText = "Remotes",
                    Children =
                        [
                            new ExplorerItem("Name", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Url", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("PushUrl", ExplorerItemKind.Property, ExplorerIcon.Column)
                        ]
                },

                // Add Stashes collection
                new ExplorerItem("Stashes", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                {
                    IsEnumerable = true,
                    ToolTipText = "All stashes in the repository",
                    DragText = "Stashes",
                    Children =
                    [
                        new ExplorerItem("Index", ExplorerItemKind.Property, ExplorerIcon.Column),
                        new ExplorerItem("Reference", ExplorerItemKind.Property, ExplorerIcon.Column),
                        new ExplorerItem("Message", ExplorerItemKind.Property, ExplorerIcon.Column),
                        new ExplorerItem("When", ExplorerItemKind.Property, ExplorerIcon.Column),
                        new ExplorerItem("WorkTree", ExplorerItemKind.ReferenceLink, ExplorerIcon.ManyToOne),
                        new ExplorerItem("cmd_Apply", ExplorerItemKind.Property, ExplorerIcon.Box),
                        new ExplorerItem("cmd_Pop", ExplorerItemKind.Property, ExplorerIcon.Box),
                        new ExplorerItem("cmd_Drop", ExplorerItemKind.Property, ExplorerIcon.Box)
                    ]
                },

                // Add Head property
                new ExplorerItem("Head", ExplorerItemKind.Property, ExplorerIcon.Column)
                {
                    ToolTipText = "Current HEAD reference",
                    DragText = "Head"
                },

                // Add RepositoryPath property
                new ExplorerItem("RepositoryPath", ExplorerItemKind.Property, ExplorerIcon.Column)
                {
                    ToolTipText = "Path to the repository",
                    DragText = "RepositoryPath"
                },

                // Add Status collection (all files with changes)
                new ExplorerItem("Status", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
                {
                    IsEnumerable = true,
                    ToolTipText = "All files with status changes",
                    DragText = "Status",
                    Children =
                        [
                            new ExplorerItem("FilePath", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("Status", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IndexStatus", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("WorkDirStatus", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IsStaged", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("HasUnstagedChanges", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IsUntracked", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IsIgnored", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("IsConflicted", ExplorerItemKind.Property, ExplorerIcon.Column),
                            new ExplorerItem("cmd_Stage", ExplorerItemKind.Property, ExplorerIcon.Box),
                            new ExplorerItem("cmd_Unstage", ExplorerItemKind.Property, ExplorerIcon.Box),
                            new ExplorerItem("cmd_Commit", ExplorerItemKind.Property, ExplorerIcon.Box),
                            new ExplorerItem("cmd_Discard", ExplorerItemKind.Property, ExplorerIcon.Box),
                            new ExplorerItem("cmd_ViewDiff", ExplorerItemKind.Property, ExplorerIcon.Box),
                        ]
                },

                // Add status properties
                new ExplorerItem("IsClean", ExplorerItemKind.Property, ExplorerIcon.Column)
                {
                    ToolTipText = "Is the working directory clean?",
                    DragText = "IsClean"
                },
                new ExplorerItem("ChangedFilesCount", ExplorerItemKind.Property, ExplorerIcon.Column)
                {
                    ToolTipText = "Number of files with changes",
                    DragText = "ChangedFilesCount"
                }
            };

            return schema;
        }

        public override ParameterDescriptor[] GetContextConstructorParameters(IConnectionInfo cxInfo)
        {
            return null; // Use default constructor
        }

        public override object[] GetContextConstructorArguments(IConnectionInfo cxInfo)
        {
            return null; // Use default constructor
        }

        public override void InitializeContext(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager)
        {
            // Initialize the static GitContext with the repository path and Beyond Compare path
            var props = new ConnectionProperties(cxInfo);
            GitContext.Initialize(props.RepositoryPath, props.BeyondComparePath);
        }

        public override IEnumerable<string> GetAssembliesToAdd(IConnectionInfo cxInfo)
        {
            // Return this driver's assembly so GitContext is available
            return [typeof(GitContext).Assembly.Location];
        }

        public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo)
        {
            // Add the namespace so types are accessible
            // Using static makes GitContext members directly accessible
            return ["LearningLINQPad.GitList", "System.Linq", "static LearningLINQPad.GitList.GitContext"];
        }

        public override void OnQueryFinishing(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager)
        {
            // No cleanup needed - GitContext manages its own repository instance
        }

#if NETCORE
        // Put stuff here that's just for LINQPad 6+ (.NET Core and .NET 5+).
#else
		// Put stuff here that's just for LINQPad 5 (.NET Framework)
#endif
    }
}