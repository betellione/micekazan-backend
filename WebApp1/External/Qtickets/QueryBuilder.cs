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
    }

    private class WhereClause(string column, object? value, string? @operator)
    {
        [JsonPropertyName("column")]
        public string Column { get; set; } = column;

        [JsonPropertyName("value")]
        public object? Value { get; set; } = value;

        [JsonPropertyName("operator")]
        public string? Operator { get; set; } = @operator;
    }
}