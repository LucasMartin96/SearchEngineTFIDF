namespace SearchEngine.Core.Interfaces.Services;

using SearchEngine.Shared.DTOs.Responses;

public interface IStatisticsService
{
    Task<StatisticsResponse> GetStatisticsAsync();
} 