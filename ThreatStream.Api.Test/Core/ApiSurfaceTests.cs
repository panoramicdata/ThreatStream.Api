using Refit;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ThreatStream.Api.Test.Core;

/// <summary>
/// Enforces the rules of docs/IMPLEMENTING.md on every endpoint interface, client property and model, so each category
/// added later is held to them.
/// </summary>
public class ApiSurfaceTests
{
	private const string InterfaceNamespace = "ThreatStream.Api.Interfaces";
	private const string ModelNamespace = "ThreatStream.Api.Models";

	private static readonly Type[] ExportedTypes = typeof(ThreatStreamClient).Assembly.GetExportedTypes();

	private static readonly List<Type> Interfaces = [.. ExportedTypes.Where(t => t.IsInterface && t.Namespace == InterfaceNamespace)];

	private static readonly List<(Type Interface, MethodInfo Method, HttpMethodAttribute Attribute)> Operations =
		[.. Interfaces.SelectMany(t => t.GetMethods()
			.Select(m => (Interface: t, Method: m, Attribute: m.GetCustomAttribute<HttpMethodAttribute>()!)))];

	private static readonly List<Type> ModelTypes = [.. ExportedTypes.Where(t => t.Namespace is { } ns
		&& (ns == ModelNamespace || ns.StartsWith(ModelNamespace + ".", StringComparison.Ordinal))
		&& t.IsClass
		&& t != typeof(ThreatStream.Api.Models.ThreatStreamFile))];

	private static void AssertNone(IEnumerable<string> offending, string because)
	{
		var list = offending.ToList();
		list.Should().BeEmpty("{0}; found {1}:{2}{3}", because, list.Count, Environment.NewLine, string.Join(Environment.NewLine, list));
	}

	[Fact]
	public void PublicApi_HasNoOptionalParameters()
	{
		const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

		var optional = ExportedTypes
			.SelectMany(t => t.GetMethods(flags).Cast<MethodBase>().Concat(t.GetConstructors(flags))
				.Where(m => m.IsPublic || (!t.IsSealed && (m.IsFamily || m.IsFamilyOrAssembly)))
				.SelectMany(m => m.GetParameters()
					.Where(p => p.IsOptional || p.HasDefaultValue)
					.Select(p => $"{t.FullName}.{m.Name}({p.Name})")));

		AssertNone(optional, "the public API takes no optional parameters (Sonar S2360): pass CancellationToken.None or a null options object instead");
	}

	[Fact]
	public void EndpointInterfaces_ArePublic_WithOnlyPublicMembers()
	{
		Interfaces.Should().NotBeEmpty();
		AssertNone(
			typeof(ThreatStreamClient).Assembly.GetTypes().Where(t => t.IsInterface && t.Namespace == InterfaceNamespace && !t.IsPublic).Select(t => t.Name),
			"every endpoint interface is exposed by a public ThreatStreamClient property");
		AssertNone(
			Interfaces.SelectMany(t => t.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(m => $"{t.Name}.{m.Name}")),
			"a non-public interface member cannot be implemented or mocked outside the library");
	}

	[Fact]
	public void EndpointMethods_AreRefitOperations_EndingWithACancellationToken()
		=> AssertNone(
			Operations.Where(o => o.Attribute is null
				|| o.Method.GetParameters() is not [.., { ParameterType: var last }] || last != typeof(CancellationToken))
				.Select(o => $"{o.Interface.Name}.{o.Method.Name}"),
			"every endpoint method has a Refit verb attribute and takes a required CancellationToken as its last parameter");

	[Fact]
	public void EndpointPaths_AreRelative_VersionedAndEndInASlash()
		=> AssertNone(
			Operations.Where(o => o.Attribute is not null)
				.Select(o => (Name: $"{o.Interface.Name}.{o.Method.Name}", Path: o.Attribute.Path.Split('?')[0]))
				.Where(o => o.Path.StartsWith('/') || !o.Path.StartsWith("api/v", StringComparison.Ordinal) || !o.Path.EndsWith('/'))
				.Select(o => $"{o.Name}: {o.Path}"),
			"paths are relative (no leading slash), start with the api/vN/ prefix and end in a slash, as Django expects");

	[Fact]
	public void EndpointMethods_ReturnModels_NotRawResponses()
		=> AssertNone(
			Operations.Where(o => !IsAllowedReturnType(o.Method.ReturnType)).Select(o => $"{o.Interface.Name}.{o.Method.Name}: {o.Method.ReturnType}"),
			"methods return Task or Task<T>, never HttpResponseMessage or ApiResponse<T>, which bypass the error mapping");

	private static bool IsAllowedReturnType(Type type)
		=> type == typeof(Task)
			|| (type.IsGenericType
				&& type.GetGenericTypeDefinition() == typeof(Task<>)
				&& type.GetGenericArguments()[0] is var result
				&& result != typeof(HttpResponseMessage)
				&& !typeof(IApiResponse).IsAssignableFrom(result));

	[Fact]
	public void EveryInterface_HasExactlyOneClientProperty_AndNamesAreUnique()
	{
		var properties = typeof(ThreatStreamClient).GetProperties().Where(p => p.PropertyType.Namespace == InterfaceNamespace).ToList();

		AssertNone(
			Interfaces.Where(i => properties.Count(p => p.PropertyType == i) != 1).Select(i => i.Name),
			"each endpoint interface is exposed by exactly one ThreatStreamClient property");
		properties.Select(p => p.Name).Should().OnlyHaveUniqueItems();
		Interfaces.Select(i => i.Name).Should().OnlyHaveUniqueItems();
	}

	[Fact]
	public void ModelProperties_NameTheirWireField()
		=> AssertNone(
			ModelTypes.SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Where(p => p.GetCustomAttribute<JsonPropertyNameAttribute>() is null
					&& p.GetCustomAttribute<AliasAsAttribute>() is null
					&& p.GetCustomAttribute<JsonExtensionDataAttribute>() is null
					&& p.GetCustomAttribute<JsonIgnoreAttribute>() is null)
				.Select(p => $"{t.Name}.{p.Name}")),
			"every model property carries [JsonPropertyName] (or [AliasAs] on query options)");

	[Fact]
	public void ModelTimestamps_AreNullableDateTimeOffsets()
		=> AssertNone(
			ModelTypes.SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
				.Select(p => $"{t.Name}.{p.Name}: {p.PropertyType.Name}")),
			"timestamps are DateTimeOffset?, the type ThreatStreamJson reads tolerantly (offset-less text as UTC, epoch seconds, empty strings)");
}
