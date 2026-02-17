using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;

namespace TheBoys.Application.Abstractions.Services;

public interface ILanguageService
{
    Task<PaginationResponse<List<LanguageDto>>> GetAllAsync(
        PaginateRequest request,
        CancellationToken cancellationToken = default
    );
}
