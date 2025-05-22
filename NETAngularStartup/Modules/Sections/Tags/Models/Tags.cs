using System.ComponentModel.DataAnnotations.Schema;
using AngleSharp.Dom;
using DevCodeArchitect.Entity;
using Newtonsoft.Json;

namespace DevCodeArchitect.DBContext
{
    /// <summary>
    /// Represents a tag entity used for categorizing and organizing content
    /// </summary>
    /// <remarks>
    /// Tags can be applied to various content types (blogs, properties, etc.)
    /// and classified by importance level and type.
    /// </remarks>
    public class Tags
    {
        /// <summary>
        /// Gets or sets the unique identifier for the tag
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the display title of the tag
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL-friendly version of the tag (slug)
        /// </summary>
        [JsonProperty("term")]
        public string Term { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the tag is enabled (1) or disabled (0)
        /// </summary>
        [JsonProperty("isEnabled")]
        public Types.ActionTypes IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the importance level of the tag
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// 0 = Low importance
        /// 1 = Medium importance
        /// 2 = High importance
        /// </remarks>
        [JsonProperty("tagLevel")]
        public TagEnum.TagLevel TagLevel { get; set; }

        /// <summary>
        /// Gets or sets the classification type of the tag
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// 0 = Normal tag
        /// 1 = User search term
        /// 2 = System-generated tag
        /// </remarks>
        [JsonProperty("tagType")]
        public TagEnum.TagType TagType { get; set; }

        /// <summary>
        /// Gets or sets the content type this tag applies to
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// 0 = Blogs
        /// 1 = Properties
        /// 2 = Products
        /// 3 = General content
        /// </remarks>
        [JsonProperty("type")]
        public TagEnum.Types Type { get; set; }

        /// <summary>
        /// Gets or sets the count of records associated with this tag
        /// </summary>
        [JsonProperty("records")]
        public int Records { get; set; }

        /// <summary>
        /// Gets or sets a transient status message (not persisted in database)
        /// </summary>
        [NotMapped]
        [JsonProperty("actionStatus")]
        public string? ActionStatus { get; set; }
    }
}