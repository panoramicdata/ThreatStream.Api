using Refit;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ThreatStream.Api.Test.Core;

public partial class InventoryTests
{
	/// <summary>
	/// The path without its query string and leading or trailing slashes, with every placeholder (<c>{name}</c> or the
	/// reference's <c>&lt;name&gt;</c>) reduced to <c>{}</c>. The <c>/api/vN/</c> prefix is kept.
	/// </summary>
	internal static string Normalize(string path)
		=> PlaceholderRegex().Replace(path.Split('?')[0].Trim('/'), "{}");

	[GeneratedRegex(@"\{[^}]*\}|<[^>]*>")]
	private static partial Regex PlaceholderRegex();

	[GeneratedRegex(@"^\| (GET|POST|PATCH|DELETE|PUT) \| `([^`]+)` \|([^|]*)\|([^|]*)\|$")]
	private static partial Regex RowRegex();

	private static List<Row> ParseRows()
	{
		var rows = new List<Row>();
		foreach (var file in Directory.GetFiles(Path.Combine(RepoRoot, "docs", "endpoints"), "*.md").Order(StringComparer.Ordinal))
		{
			var lines = File.ReadAllLines(file);
			for (var i = 0; i < lines.Length; i++)
			{
				if (RowRegex().Match(lines[i].TrimEnd()) is { Success: true } match)
				{
					rows.Add(new Row(
						Path.GetFileName(file),
						i + 1,
						Path.GetFileNameWithoutExtension(file),
						match.Groups[1].Value,
						match.Groups[2].Value,
						SplitCell(match.Groups[3].Value),
						SplitCell(match.Groups[4].Value)));
				}
			}
		}

		return rows;
	}

	private static string[] SplitCell(string cell)
		=> [.. cell.Replace("`", string.Empty, StringComparison.Ordinal).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

	private static HashSet<string> ReadPendingCategories()
		=> [.. File.ReadAllLines(Path.Combine(RepoRoot, "docs", "pending-categories.txt"))
			.Select(l => l.Trim())
			.Where(l => l.Length > 0 && !l.StartsWith('#'))];

	private static List<Operation> FindOperations()
		=> [.. typeof(ThreatStreamClient).Assembly.GetExportedTypes()
			.Where(t => t.IsInterface)
			.SelectMany(t => t.GetMethods()
				.Select(m => (Method: m, Attribute: m.GetCustomAttribute<HttpMethodAttribute>()))
				.Where(x => x.Attribute is not null)
				.Select(x => new Operation(t.Name, x.Method.Name, x.Attribute!.Method.Method, Normalize(x.Attribute.Path))))];

	private static string FindRepoRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ThreatStream.Api.slnx")))
		{
			directory = directory.Parent;
		}

		return directory?.FullName ?? throw new InvalidOperationException("Could not find the repository root (ThreatStream.Api.slnx).");
	}
}
