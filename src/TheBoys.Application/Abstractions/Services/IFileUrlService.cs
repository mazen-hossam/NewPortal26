namespace TheBoys.Application.Abstractions.Services;

public interface IFileUrlService
{
    string ToAbsoluteUrl(string path);
}
