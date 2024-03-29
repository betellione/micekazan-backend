// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable CollectionNeverQueried.Local

using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Packaging;

namespace WebApp1.External.Qtickets;

public class QueryBuilder
{
    private static readonly JsonSerializerOptions Options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, };

    private readonly Query _query = new();

    public QueryBuilder Where(string column, object? value, string? @operator = null)
    {
        var clause = new WhereClause(column, value, @operator);
        _query.Where ??= new List<WhereClause>();
        _query.Where.Add(clause);
        return this;
    }

    public QueryBuilder Select(string field)
    {
        _query.Select ??= new List<string>();
        _query.Select.Add(field);
        return this;
    }

    public QueryBuilder Select(params string[] fields)
    {
        _query.Select ??= new List<string>();
        _query.Select.AddRange(fields);
        return this;
    }

    public QueryBuilder PerPage(int perPage)
    {
        _query.PerPage = perPage;
        return this;
    }

    public QueryBuilder Page(int page)
    {
        _query.Page = page;
        return this;
    }

    public QueryBuilder OrderByAscending(string fieldName)
    {
        _query.OrderByRules ??= new Dictionary<string, string>();
        _query.OrderByRules.TryAdd(fieldName, "asc");
        return this;
    }

    public QueryBuilder OrderByDescending(string fieldName)
    {
        _query.OrderByRules ??= new Dictionary<string, string>();
        _query.OrderByRules.TryAdd(fieldName, "desc");
        return this;
    }

    public QueryBuilder OrderByAscending(params string[] fieldNames)
    {
        throw new NotImplementedException();
    }

    public QueryBuilder OrderByDescending(params string[] fieldNames)
    {
        throw new NotImplementedException();
    }

    public string Build()
    {
        return JsonSerializer.Serialize(_query, Options);
    }

    private class Query
    {
        [JsonPropertyName("select")]
        public ICollection<string>? Select { get; set; }

        [JsonPropertyName("where")]
        public ICollection<WhereClause>? Where { get; set; }

        [JsonPropertyName("page")]
        public int? Page { get; set; }

        [JsonPropertyName("perPage")]
        public int? PerPage { get; set; }

        [JsonPropertyName("orderBy")]
        public Dictionary<string, string>? OrderByRules { get; set; }
    }

    private record WhereClause(
        // ReSharper disable once NotAccessedPositionalProperty.Local
        [property: JsonPropertyName("column")] string Column,

        // ReSharper disable once NotAccessedPositionalProperty.Local
        [property: JsonPropertyName("value")] object? Value,

        // ReSharper disable once NotAccessedPositionalProperty.Local
        [property: JsonPropertyName("operator")] string? Operator);
}