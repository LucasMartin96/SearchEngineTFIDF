using SearchEngine.Core.Interfaces.Repositories;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.Core.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ITermRepository _termRepository;
    private readonly ITermOccurrenceRepository _termOccurrenceRepository;
    private readonly ICacheService _cacheService;
    private const string StatisticsCacheKey = "STATISTICS_CACHE";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public StatisticsService(
        IDocumentRepository documentRepository,
        ITermRepository termRepository,
        ITermOccurrenceRepository termOccurrenceRepository,
        ICacheService cacheService)
    {
        _documentRepository = documentRepository;
        _termRepository = termRepository;
        _termOccurrenceRepository = termOccurrenceRepository;
        _cacheService = cacheService;
    }

    public async Task<StatisticsResponse> GetStatisticsAsync()
    {
        return await _cacheService.GetOrCreateAsync(StatisticsCacheKey, async () =>
        {
            var statistics = await _documentRepository.GetDocumentStatisticsAsync();
            
            return new StatisticsResponse
            {
                TotalDocuments = statistics.TotalDocs,
                TotalUniqueTerms = await _termRepository.GetTotalCountAsync(),
                TotalWordCount = statistics.TotalWords
            };
        }, CacheDuration) ?? new StatisticsResponse();
    }
} 