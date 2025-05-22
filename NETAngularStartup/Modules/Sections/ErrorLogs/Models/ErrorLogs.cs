using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.DBContext;

/// <summary>
/// Represents an error log entry in the system, capturing exception details
/// and contextual information for debugging and monitoring purposes.
/// </summary>
public class ErrorLogs
{
    /// <summary>
    /// Primary key identifier for the error log entry
    /// JSON Property: "id" (integer)
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Brief description of the error that occurred
    /// JSON Property: "description" (string, nullable)
    /// Example: "Database connection timeout"
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// URL or endpoint where the error originated
    /// JSON Property: "url" (string, nullable)
    /// Example: "/api/products/get/123"
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Complete stack trace of the exception
    /// JSON Property: "stack_trace" (string, nullable)
    /// </summary>
    [JsonPropertyName("stack_trace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Details of any inner exception that may have caused the primary exception
    /// JSON Property: "inner_exception" (string, nullable)
    /// </summary>
    [JsonPropertyName("inner_exception")]
    public string? InnerException { get; set; }

    /// <summary>
    /// Timestamp when the error was logged (automatically set by the system)
    /// JSON Property: "created_at" (string in ISO 8601 format)
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Status indicator for UI actions (not persisted to database)
    /// JSON Property: "actionstatus" (string, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("actionstatus")]
    public string? ActionStatus { get; set; }
}