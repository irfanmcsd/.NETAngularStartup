using System.Text.Json.Serialization;

namespace DevCodeArchitect.Utilities;

public class CategoryItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("term")]
    public string? Term { get; set; }

    [JsonPropertyName("sub_term")]
    public string? SubTerm { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
