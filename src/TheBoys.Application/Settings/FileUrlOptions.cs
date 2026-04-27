namespace TheBoys.Application.Settings;

public sealed class FileUrlOptions
{
    public const string SectionName = "FileUrls";

    public string UploadsRequestPath { get; set; } = "/uploads";
    public string PublicBaseUrl { get; set; }
    public string LegacyFilesBaseUrl { get; set; }
}
