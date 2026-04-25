using System.Text.RegularExpressions;
using TheBoys.Application.Misc;

namespace TheBoys.Application.Extensions;

public static class StringExtensions
{
    public static bool HasValue(this string value) =>
        value is not null && !string.IsNullOrWhiteSpace(value) && !string.IsNullOrEmpty(value);

    public static string GetFullPath(Guid ownerId, string imgName)
    {
        if (!imgName.HasValue())
        {
            return string.Empty;
        }

        if (
            Uri.TryCreate(imgName, UriKind.Absolute, out var absoluteUri)
            && (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps)
        )
        {
            return absoluteUri.ToString();
        }

        var fileName = Path.GetFileName(imgName.Replace('\\', '/'));
        if (!fileName.HasValue())
        {
            return string.Empty;
        }

        if (
            ImageHelper.Images.TryGetValue(ownerId.ToString().ToLowerInvariant(), out var path)
        )
        {
            return $"{path}{Uri.EscapeDataString(fileName)}";
        }

        return $"/uploads/{ownerId.ToString().ToLowerInvariant()}/{Uri.EscapeDataString(fileName)}";
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
