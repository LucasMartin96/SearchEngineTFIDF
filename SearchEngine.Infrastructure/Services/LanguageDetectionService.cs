using SearchEngine.Core.Interfaces.Services;



namespace SearchEngine.Infrastructure.Services;



public class LanguageDetectionService : ILanguageDetectionService
{
    private static readonly HashSet<string> SupportedLanguages = new()
    {
        "en", 
        "es", 
        "pt", 
        "fr" 
    };

    public async Task<string> DetectLanguageAsync(string text)
    {
        string[] words = text.ToLower().Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        Dictionary<string, int> scores = new Dictionary<string, int>
        {
            ["en"] = 0,
            ["es"] = 0,
            ["pt"] = 0,
            ["fr"] = 0
        };

        foreach (string word in words)
        {
            switch (word)
            {
                case "the" or "is" or "at" or "which" or "this":
                    scores["en"]++;
                    break;
                case "el" or "la" or "los" or "las" or "esto":
                    scores["es"]++;
                    break;
                case "os" or "isso" or "esta" or "você" or "são":
                    scores["pt"]++;
                    break;
            }

            if (word is "le" or "la" or "les" or "cette" or "vous")
                scores["fr"]++;
        }

        string detectedLanguage = scores.OrderByDescending(x => x.Value).First().Key;
        return await Task.FromResult(detectedLanguage);
    }

    public bool IsSupported(string languageCode)
    {
        return SupportedLanguages.Contains(languageCode.ToLower());
    }
} 