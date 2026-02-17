using System.Text.Json;
using TheBoys.API.Entities;
using TheBoys.API.Misc;

namespace TheBoys.API.Seeding;

public class SeedingService : ISeedingService
{
    readonly IWebHostEnvironment _webHostEnvironment;

    public SeedingService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public void SeedLanguages()
    {
        if (StaticLanguages.languageModels.Any())
            return;

        var fileName = "Languages.json";
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Data", fileName);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File {fileName} not found at path {filePath}");

        var languagesJson = File.ReadAllText(filePath);
        var languages = JsonSerializer.Deserialize<IEnumerable<LanguageModel>>(languagesJson);
        StaticLanguages.languageModels.AddRange(languages);
    }
}
