
using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.Entity;

public class Urls
{
    public static string PreviewUrl(UrlEntity entity)
    {
        string directory = "";
        var url = "";
        if (!string.IsNullOrEmpty(entity.Slug))
        {
            if (!string.IsNullOrEmpty(entity.Directory))
                directory = entity.Directory;

            url = directory + "/" + entity.Slug;
        }
        else
        {
            if (!string.IsNullOrEmpty(entity.Directory))
                directory = entity.Directory;

            if (string.IsNullOrEmpty(entity.Title))
                url = directory + "/" + entity.Id;

            else
            {
                string _title;
                if (entity.Title == null)
                    entity.Title = "";
                int maxium_length = ApplicationSettings.MaximumUrlCharacters;
                if (entity.Title.Length > maxium_length && maxium_length > 0)
                    _title = entity.Title.Substring(0, maxium_length);
                else if (entity.Title.Length < 3)
                {
                    if (!string.IsNullOrEmpty(entity.DefaultLabel))
                        _title = entity.DefaultLabel;
                    else
                        _title = "preview";
                }
                else
                    _title = entity.Title;


                // you can adjust url pattern here
                if (entity.Id == 0)
                    url = directory + "/" + UtilityHelper.ReplaceSpaceWithHyphen(_title.ToLower());
                else
                    url = directory + "/" + entity.Id + "/" + UtilityHelper.ReplaceSpaceWithHyphen(_title.ToLower());
            }

        }

        return ApplicationSettings.Domain.Backend + url;

    }

}

public class UrlEntity
{
    public long Id { get; set; }
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? DefaultLabel { get; set; }
    public string? Directory { get; set; }
}
