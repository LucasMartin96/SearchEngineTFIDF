using SearchEngine.Shared.DTOs;

namespace SearchEngine.Core.Interfaces.Services;

public interface IGutenbergService
{
    Task<GutenbergBookDto?> GetBookAsync(int bookId);
} 