using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;

namespace TheBoys.Application.Abstractions.Services;

public interface IFacultyNewsService
{
    Task<PaginationResponse<List<FacultyNewsItemDto>>> GetPublishedAsync(
        int publicFacultyCode,
        int langId,
        PaginateRequest request,
        CancellationToken cancellationToken = default
    );

    Task<ResponseOf<FacultyNewsDetailsDto>> GetDetailsAsync(
        int publicFacultyCode,
        int id,
        int langId,
        CancellationToken cancellationToken = default
    );
}
