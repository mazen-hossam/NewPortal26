using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Misc;
using TheBoys.Domain.Entities;

namespace TheBoys.Infrastructure.Services;

public class SeedingService : ISeedingService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SeedingService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public void SeedLanguages()
    {
        if (StaticLanguages.LanguageModels.Any())
        {
            return;
        }

        const string fileName = "Languages.json";
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Data", fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File {fileName} not found at path {filePath}");
        }

        var languagesJson = File.ReadAllText(filePath);
        var languages = JsonSerializer.Deserialize<IEnumerable<LanguageModel>>(languagesJson);
        if (languages is null)
        {
            return;
        }

        StaticLanguages.LanguageModels.AddRange(languages);
    }
}
