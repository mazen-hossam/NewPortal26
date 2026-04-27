using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Settings;

namespace TheBoys.Infrastructure.Services;

public sealed class FileUrlService : IFileUrlService
{
    private const string LegacyPortalPath = "/PrtlFiles";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FileUrlOptions _options;

    public FileUrlService(IHttpContextAccessor httpContextAccessor, IOptions<FileUrlOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public string ToAbsoluteUrl(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        var normalizedPath = NormalizePath(path);
        if (IsAbsoluteHttpUrl(normalizedPath))
        {
            return normalizedPath;
        }

        if (MatchesPathPrefix(normalizedPath, LegacyPortalPath))
        {
            return CombineAbsoluteUrl(ResolveLegacyBaseUrl(), EnsureLeadingSlash(normalizedPath));
        }

        if (MatchesPathPrefix(normalizedPath, _options.UploadsRequestPath))
        {
            return CombineAbsoluteUrl(ResolveApplicationBaseUrl(), EnsureLeadingSlash(normalizedPath));
        }

        if (normalizedPath.StartsWith('/'))
        {
            return CombineAbsoluteUrl(ResolveApplicationBaseUrl(), normalizedPath);
        }

        return CombineAbsoluteUrl(
            ResolveApplicationBaseUrl(),
            CombinePathSegments(_options.UploadsRequestPath, normalizedPath)
        );
    }

    private string ResolveApplicationBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request?.Host.HasValue == true)
        {
            return $"{request.Scheme}://{request.Host.ToUriComponent()}{request.PathBase.ToUriComponent()}".TrimEnd('/');
        }

        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
        {
            return _options.PublicBaseUrl.Trim().TrimEnd('/');
        }

        throw new InvalidOperationException(
            "Unable to resolve the current application base URL for file path normalization."
        );
    }

    private string ResolveLegacyBaseUrl()
    {
        if (!string.IsNullOrWhiteSpace(_options.LegacyFilesBaseUrl))
        {
            return _options.LegacyFilesBaseUrl.Trim().TrimEnd('/');
        }

        return ResolveApplicationBaseUrl();
    }

    private static string NormalizePath(string path) => path.Trim().Replace('\\', '/');

    private static bool IsAbsoluteHttpUrl(string path) =>
        Uri.TryCreate(path, UriKind.Absolute, out var absoluteUri)
        && (
            absoluteUri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            || absoluteUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
        );

    private static bool MatchesPathPrefix(string path, string prefix)
    {
        var normalizedPath = EnsureLeadingSlash(NormalizePath(path));
        var normalizedPrefix = EnsureLeadingSlash(NormalizePath(prefix).TrimEnd('/'));

        if (normalizedPath.Equals(normalizedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return normalizedPath.StartsWith(
            $"{normalizedPrefix}/",
            StringComparison.OrdinalIgnoreCase
        );
    }

    private static string EnsureLeadingSlash(string path) =>
        path.StartsWith('/') ? path : $"/{path}";

    private static string CombinePathSegments(string basePath, string relativePath)
    {
        var normalizedBase = EnsureLeadingSlash(NormalizePath(basePath).TrimEnd('/'));
        var normalizedRelative = NormalizePath(relativePath).TrimStart('/');
        return $"{normalizedBase}/{normalizedRelative}";
    }

    private static string CombineAbsoluteUrl(string baseUrl, string path)
    {
        var normalizedBase = $"{baseUrl.TrimEnd('/')}/";
        var normalizedPath = path.TrimStart('/');
        return new Uri(new Uri(normalizedBase), normalizedPath).ToString();
    }
}
