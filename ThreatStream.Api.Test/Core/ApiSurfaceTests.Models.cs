using Refit;
using System.Reflection;
using System.Text.Json.Serialization;
using static ThreatStream.Api.Test.Support.Offenders;

namespace ThreatStream.Api.Test.Core;

public partial class ApiSurfaceTests
{
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
