namespace SearchEngine.Core.Interfaces.Services;

public interface ILanguageDetectionService
{
    Task<string> DetectLanguageAsync(string text);
    bool IsSupported(string languageCode);
} 