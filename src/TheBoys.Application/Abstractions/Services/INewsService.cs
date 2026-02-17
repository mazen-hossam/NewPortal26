using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;

namespace TheBoys.Application.Abstractions.Services;

public interface INewsService
{
    Task<PaginationResponse<List<NewsDto>>> PaginateSectorNewsAsync(
        PaginateNewsRequest request,
        CancellationToken cancellationToken = default
    );

    Task<PaginationResponse<List<NewsDto>>> PaginateUniversityNewsAsync(
        PaginateNewsRequest request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsDto>> GetSectorNewsByIdAsync(
        int id,
        int languageId,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<NewsDto>> GetUniversityNewsByIdAsync(
        int id,
        int languageId,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<List<NewsDto>>> SearchByOwnerAbbreviationAsync(
        string abbreviation,
        int languageId,
        CancellationToken cancellationToken = default
    );
}
