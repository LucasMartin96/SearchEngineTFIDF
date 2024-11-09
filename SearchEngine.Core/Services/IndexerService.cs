using SearchEngine.Core.Entities;
using SearchEngine.Core.Interfaces.Repositories;
using SearchEngine.Core.Interfaces.Services;
using System.Text;
using System.Globalization;
using Microsoft.Extensions.Logging;
using SearchEngine.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SearchEngine.Core.Services;

// TODO: Implement batch upload
// TODO: Weight in common terms is good here
public partial class IndexerService : IIndexerService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ITermRepository _termRepository;
    private readonly ITermOccurrenceRepository _termOccurrenceRepository;
    private readonly IStopWordRepository _stopWordRepository;
    private readonly ILanguageDetectionService _languageDetectionService;
    private IDictionary<string, HashSet<string>> _vocabulary;
    private readonly ILogger<IndexerService> _logger;
    private readonly ISearchService _searchService;
    private readonly ICacheService _cacheService;
    private const string StopWordsCacheKey = "STOPWORDS_BY_LANGUAGE";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);
    
    private static readonly Regex WordSplitRegex = MyWordSplitRegex(); 
    private static readonly Regex NonAlphanumericRegex = MyNonAlphanumericRegex();

    public IndexerService(
        IDocumentRepository documentRepository,
        ITermRepository termRepository,
        ITermOccurrenceRepository termOccurrenceRepository,
        IStopWordRepository stopWordRepository,
        ILanguageDetectionService languageDetectionService,
        ISearchService searchService,
        ICacheService cacheService,
        ILogger<IndexerService> logger)
    {
        _documentRepository = documentRepository;
        _termRepository = termRepository;
        _termOccurrenceRepository = termOccurrenceRepository;
        _stopWordRepository = stopWordRepository;
        _languageDetectionService = languageDetectionService;
        _searchService = searchService;
        _cacheService = cacheService;
        _logger = logger;
        LoadStopWords().Wait();
    }

    private async Task LoadStopWords()
    {
        _vocabulary = await _cacheService.GetOrCreateAsync(
            StopWordsCacheKey,
            async () => await _stopWordRepository.GetAllStopWordsByLanguageAsync(),
            CacheDuration) ?? new Dictionary<string, HashSet<string>>();
    }

    public async Task<Document> IndexDocumentAsync(string title, string path, string content)
    {
        try
        {
            _logger.LogInformation("Starting indexing for document: {Path}", path);

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new IndexingException(path, "Document content cannot be empty");
            }

            Document document = new Document
            {
                Title = title,
                Path = path,
                WordCount = 0
            };

            List<string> terms = await ExtractTermsAsync(content);
            document.WordCount = terms.Count;

            await _documentRepository.AddAsync(document);
            await _documentRepository.SaveChangesAsync();

            await UpdateTermFrequenciesAsync(document, terms);
            
            _searchService.InvalidateIdfCache();

            _logger.LogInformation("Successfully indexed document: {Path}", path);
            return document;
        }
        catch (Exception ex) when (ex is not SearchEngineException)
        {
            _logger.LogError(ex, "Failed to index document: {Path}", path);
            throw new IndexingException(path, ex.Message);
        }
    }

    private async Task<List<string>> ExtractTermsAsync(string content)
    {
        try
        {
            string detectedLanguage = await _languageDetectionService.DetectLanguageAsync(content);
            _logger.LogInformation("Detected language: {Language}", detectedLanguage);

            return ExtractTermsWithLanguage(content, detectedLanguage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting terms from content");
            throw new IndexingException("unknown", "Failed to extract terms from document");
        }
    }

    private List<string> ExtractTermsWithLanguage(string content, string language)
    {
        content = NormalizeText(content);

        List<string> words = content
            .Split(new[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(term => term.Length > 2)
            .Select(term => term.ToLower()).ToList();

        if (_vocabulary.TryGetValue(language, out HashSet<string>? stopWords))
        {
            words = words.Where(w => !stopWords.Contains(w)).ToList();
        }

        return words;
    }

    private string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        
        text = text.Normalize(NormalizationForm.FormD);
        text = new string(text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
        
        text = text.ToLowerInvariant();
        
        List<string> words = WordSplitRegex.Split(text)
            .Select(word => NonAlphanumericRegex.Replace(word, ""))
            .Where(word => !string.IsNullOrWhiteSpace(word)).ToList();

        return string.Join(" ", words);
    }

    private async Task UpdateTermFrequenciesAsync(Document document, IEnumerable<string> terms)
    {
        try
        {
            Dictionary<string, int> termFrequencies = terms.GroupBy(x => x)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach ((string word, int frequency) in termFrequencies)
            {
                Term? term = await _termRepository.GetByWordAsync(word);

                if (term == null)
                {
                    term = new Term { Word = word, DocumentCount = 1 };
                    await _termRepository.AddAsync(term);
                }
                else
                {
                    term.DocumentCount++;
                    await _termRepository.UpdateAsync(term);
                }

                TermOccurrence occurrence = new TermOccurrence
                {
                    Term = term,
                    Document = document,
                    Frequency = frequency
                };

                await _termOccurrenceRepository.AddAsync(occurrence);
            }

            await _termRepository.SaveChangesAsync();
            await _termOccurrenceRepository.SaveChangesAsync();

            _logger.LogInformation("Updated term frequencies for document: {Path}", document.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating term frequencies for document: {Path}", document.Path);
            throw new IndexingException(document.Path, "Failed to update term frequencies");
        }
    }

    [GeneratedRegex(@"[\s,\.;:\-_]+", RegexOptions.Compiled)]
    private static partial Regex MyWordSplitRegex();
    
    [GeneratedRegex(@"[^a-zA-Z0-9]", RegexOptions.Compiled)]
    private static partial Regex MyNonAlphanumericRegex();
}