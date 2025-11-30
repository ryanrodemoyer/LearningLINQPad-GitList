using LINQPad;
using LINQPad.Extensibility.DataContext;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;

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

		public override string GetConnectionDescription (IConnectionInfo cxInfo)
		{
			var props = new ConnectionProperties(cxInfo);
			if (!string.IsNullOrWhiteSpace(props.RepositoryPath))
			{
				string repoName = Path.GetFileName(props.RepositoryPath);
				return $"Git: {repoName}";
			}
			return "Git Repository";
		}

		public override bool ShowConnectionDialog (IConnectionInfo cxInfo, ConnectionDialogOptions dialogOptions)
			=> new ConnectionDialog (cxInfo).ShowDialog () == true;

		public override List<ExplorerItem> GetSchema (IConnectionInfo cxInfo, Type customType)
		{
			var props = new ConnectionProperties(cxInfo);
			
			if (string.IsNullOrWhiteSpace(props.RepositoryPath))
			{
				return new List<ExplorerItem>();
			}

			try
			{
				using (var repo = new LibGit2Sharp.Repository(props.RepositoryPath))
				{
					var schema = new List<ExplorerItem>();

					// Add Commits collection
					schema.Add(new ExplorerItem("Commits", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
					{
						IsEnumerable = true,
						ToolTipText = "All commits in the repository",
						DragText = "Commits",
						Children = new List<ExplorerItem>
						{
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
						}
					});

					// Add Branches collection
					schema.Add(new ExplorerItem("Branches", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
					{
						IsEnumerable = true,
						ToolTipText = "All branches (local and remote)",
						DragText = "Branches",
						Children = new List<ExplorerItem>
						{
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
						}
					});

					// Add LocalBranches collection
					schema.Add(new ExplorerItem("LocalBranches", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
					{
						IsEnumerable = true,
						ToolTipText = "Local branches only",
						DragText = "LocalBranches"
					});

					// Add RemoteBranches collection
					schema.Add(new ExplorerItem("RemoteBranches", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
					{
						IsEnumerable = true,
						ToolTipText = "Remote branches only",
						DragText = "RemoteBranches"
					});

					// Add Tags collection
					schema.Add(new ExplorerItem("Tags", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
					{
						IsEnumerable = true,
						ToolTipText = "All tags in the repository",
						DragText = "Tags",
						Children = new List<ExplorerItem>
						{
							new ExplorerItem("Name", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("CanonicalName", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("IsAnnotated", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("Message", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("Tagger", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("TaggerEmail", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("TaggerDate", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("Target", ExplorerItemKind.ReferenceLink, ExplorerIcon.ManyToOne)
						}
					});

					// Add Remotes collection
					schema.Add(new ExplorerItem("Remotes", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
					{
						IsEnumerable = true,
						ToolTipText = "Configured remotes",
						DragText = "Remotes",
						Children = new List<ExplorerItem>
						{
							new ExplorerItem("Name", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("Url", ExplorerItemKind.Property, ExplorerIcon.Column),
							new ExplorerItem("PushUrl", ExplorerItemKind.Property, ExplorerIcon.Column)
						}
					});

					// Add Head property
					schema.Add(new ExplorerItem("Head", ExplorerItemKind.Property, ExplorerIcon.ScalarFunction)
					{
						ToolTipText = "Current HEAD reference",
						DragText = "Head"
					});

					// Add RepositoryPath property
					schema.Add(new ExplorerItem("RepositoryPath", ExplorerItemKind.Property, ExplorerIcon.Column)
					{
						ToolTipText = "Path to the repository",
						DragText = "RepositoryPath"
					});

					return schema;
				}
			}
			catch (Exception ex)
			{
				// If we can't open the repository, return an empty schema
				return new List<ExplorerItem>
				{
					new ExplorerItem($"Error: {ex.Message}", ExplorerItemKind.Property, ExplorerIcon.Box)
				};
			}
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
			// Initialize the static GitContext with the repository path
			var props = new ConnectionProperties(cxInfo);
			GitContext.Initialize(props.RepositoryPath);
		}

		public override IEnumerable<string> GetAssembliesToAdd(IConnectionInfo cxInfo)
		{
			// Return this driver's assembly so GitContext is available
			return new[] { typeof(GitContext).Assembly.Location };
		}

		public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo)
		{
			// Add the namespace so types are accessible
			// Using static makes GitContext members directly accessible
			return new[] { "LearningLINQPad.GitList", "System.Linq", "static LearningLINQPad.GitList.GitContext" };
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