using System.Text.RegularExpressions;
using TheBoys.Application.Misc;

namespace TheBoys.Application.Extensions;

public static class StringExtensions
{
    public static bool HasValue(this string value) =>
        value is not null && !string.IsNullOrWhiteSpace(value) && !string.IsNullOrEmpty(value);

    public static string GetFullPath(Guid ownerId, string imgName, string? forcedBasePath = null)
    {
        if (!imgName.HasValue())
        {
            return string.Empty;
        }

        // If imgName already contains full path, use it directly
        if (imgName.StartsWith("https://"))
        {
            return imgName;
        }

        if (forcedBasePath.HasValue())
        {
            return $"{forcedBasePath!.TrimEnd('/')}/{imgName}";
        }

        if (ImageHelper.images.TryGetValue(ownerId.ToString().ToLower(), out string path))
        {
            return $"{path}{imgName}";
        }

        return $"https://mu.menofia.edu.eg/PrtlFiles/Sectors/UNIVPRES/Portal/Images/{imgName}";
    }

    public static string StripHtml(string html)
    {
        if (!html.HasValue())
        {
            return string.Empty;
        }

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.OptionFixNestedTags = true;
        doc.LoadHtml(html);

        var text = doc.DocumentNode.InnerText;
        text = System.Net.WebUtility.HtmlDecode(text);
        text = text.Replace("\r", " ").Replace("\n", " ");
        text = Regex.Replace(text, @"\s+", " ");

        return text.Trim();
    }
}
