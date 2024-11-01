using SearchEngine.Core.Entities;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Core.Helpers;
using SearchEngine.Core.Interfaces.Repositories;
using SearchEngine.Shared.DTOs.Base;
using Microsoft.Extensions.Logging;
using SearchEngine.Core.Exceptions;
using SearchEngine.Core.Records;
using SearchEngine.Shared.DTOs;
using SearchEngine.Shared.DTOs.Responses;

namespace SearchEngine.Core.Services;
public class SearchService : ISearchService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ITermRepository _termRepository;
    private readonly ITermOccurrenceRepository _termOccurrenceRepository;
    private readonly ILogger<SearchService> _logger;
    private readonly ICacheService _cache;
    // TODO: Cache per term and not per query
    private const string IdfCachePrefix = "IDF_";

    // TODO: Save some high-idf terms or common base queryes for better performance
    public SearchService(
        IDocumentRepository documentRepository,
        ITermRepository termRepository,
        ITermOccurrenceRepository termOccurrenceRepository,
        ILogger<SearchService> logger,
        ICacheService cache)
    {
        _documentRepository = documentRepository;
        _termRepository = termRepository;
        _termOccurrenceRepository = termOccurrenceRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<PagedResult<SearchResultResponse>> SearchAsync(string query, PaginationParameters parameters)
    {
        try
        {
            ValidateQuery(query);
            List<string> queryTerms = PrepareQueryTerms(query);
            List<Document> documents = await GetDocuments(query);
            
            if (!documents.Any())
            {
                return PaginationHelper.CreateEmptyResult<SearchResultResponse>(parameters);
            }

            List<ScoredDocument> scoredResults = await GetScoredResults(documents, queryTerms);
            return CreatePaginatedResult(scoredResults, parameters);
        }
        catch (Exception ex) when (ex is not SearchEngineException)
        {
            _logger.LogError(ex, "Unexpected error during search. Query: {Query}", query);
            throw new SearchEngineException("An error occurred while processing your search", ex);
        }
    }

    private static void ValidateQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new InvalidQueryException("Search query cannot be empty");
        }
    }

    private static List<string> PrepareQueryTerms(string query)
    {
        return query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private async Task<List<Document>> GetDocuments(string query)
    {
        return await _documentRepository.SearchAsync(query);
    }

    private PagedResult<SearchResultResponse> CreatePaginatedResult(
        List<ScoredDocument> results,
        PaginationParameters parameters)
    {
        int totalCount = results.Count;
        int totalPages = CalculateTotalPages(totalCount, parameters.PageSize);
        List<SearchResultResponse> orderedResults = OrderAndPaginateResults(parameters, results);
        
        return new PagedResult<SearchResultResponse>
        {
            Items = orderedResults,
            TotalCount = totalCount,
            TotalPages = totalPages,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static int CalculateTotalPages(int totalCount, int pageSize)
    {
        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    private static List<SearchResultResponse> OrderAndPaginateResults(
        PaginationParameters parameters, 
        List<ScoredDocument> results)
    {
        return results
            .OrderByDescending(x => x.Score)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(MapToSearchResult)
            .ToList();
    }

    private static SearchResultResponse MapToSearchResult(
        ScoredDocument result)
    {
        return new SearchResultResponse
        {
            DocumentId = result.Document.Id,
            Title = result.Document.Title,
            Path = result.Document.Path,
            Score = result.Score,
            TermOccurrences = result.Occurrences
        };
    }

    // TODO: Future bottleneck here, parallel processing possible solution
    private async Task<List<ScoredDocument>> GetScoredResults(
        List<Document> documents, 
        List<string> queryTerms)
    {
        try
        {
            var results = new List<ScoredDocument>();
            foreach (var document in documents)
            {
                var score = await CalculateDocumentScore(document, queryTerms);
                if (score.Score > 0)
                {
                    results.Add(new ScoredDocument(document, score.Score, score.Occurrences));
                }
            }
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating scores for documents");
            throw new SearchEngineException("Error calculating document relevance scores", ex);
        }
    }

    private async Task<DocumentScore> CalculateDocumentScore(
        Document document, 
        List<string> queryTerms)
    {
        var terms = await _termRepository.GetByWordsAsync(queryTerms);
        if (!terms.Any()) return new DocumentScore(0, new List<TermOccurrenceResponse>());

        var frequencies = await GetFrequenciesAndIdfs(document.Id, terms, queryTerms);
        return CalculateScoreAndOccurrences(terms, frequencies.Frequencies, frequencies.Idfs, document.Id);
    }

    private async Task<TermFrequencies> GetFrequenciesAndIdfs(
        Guid documentId, 
        List<Term> terms, 
        List<string> queryTerms)
    {
        var termIds = terms.Select(t => t.Id).ToList();
        var frequencyTask = _termOccurrenceRepository.GetTermFrequenciesAsync(termIds, documentId);
        var idfTask = GetInverseDocumentFrequenciesAsync(queryTerms);
        
        await Task.WhenAll(frequencyTask, idfTask);
        return new TermFrequencies(await frequencyTask, await idfTask);
    }
    
    // TODO: Implement some weighted scores
    private static DocumentScore CalculateScoreAndOccurrences(
        List<Term> terms,
        Dictionary<(Guid TermId, Guid DocumentId), double> frequencies,
        Dictionary<string, double> idfs,
        Guid documentId)
    {
        var termScores = new List<double>();
        var occurrences = new List<TermOccurrenceResponse>();

        foreach (var term in terms)
        {
            if (frequencies.TryGetValue((term.Id, documentId), out double tf) && 
                idfs.TryGetValue(term.Word, out double idf))
            {
                termScores.Add(tf * idf);
                occurrences.Add(new TermOccurrenceResponse
                {
                    TermId = term.Id,
                    Value = term.Word,
                    Frequency = (int)tf
                });
            }
        }

        return new DocumentScore(
            termScores.Any() ? termScores.Average() : 0, 
            occurrences);
    }

    private async Task<Dictionary<string, double>> GetInverseDocumentFrequenciesAsync(List<string> queryTerms)
    {
        string cacheKey = $"{IdfCachePrefix}{string.Join("_", queryTerms)}";
        return await _cache.GetOrCreateAsync(cacheKey, 
            () => _termRepository.GetInverseDocumentFrequenciesAsync(queryTerms)) 
            ?? new Dictionary<string, double>();
    }

    public void InvalidateIdfCache()
    {
        _cache.Remove(IdfCachePrefix);
    }
}
