using Microsoft.EntityFrameworkCore;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;
using TheBoys.Application.Extensions;
using TheBoys.Application.Misc;
using TheBoys.Infrastructure.Persistence;

namespace TheBoys.Infrastructure.Services;

public class LanguageService : ILanguageService
{
    private readonly ApplicationDbContext _context;

    public LanguageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResponse<List<LanguageDto>>> GetAllAsync(
        PaginateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var response = new PaginationResponse<List<LanguageDto>>() { Result = new List<LanguageDto>() };
        var query = _context.Languages.AsNoTracking().OrderBy(x => x.LCID.ToLower()).AsQueryable();

        response.TotalCount = await query.CountAsync(cancellationToken);
        query = query.Paginate(request.PageIndex, request.PageSize);

        response.PageIndex = request.PageIndex;
        response.PageSize = request.PageSize;

        var languages = await query.ToListAsync(cancellationToken);
        foreach (var language in StaticLanguages.LanguageModels)
        {
            var lang = languages.FirstOrDefault(x =>
                x.LCID.Trim().ToLower() == language.Code.Trim().ToLower()
            );
            if (lang is null)
            {
                continue;
            }

            response.Result.Add(
                new LanguageDto
                {
                    Id = lang.Id,
                    Name = language.Name,
                    Code = language.Code,
                    Flag = language.Flag
                }
            );
        }

        response.Count = response.Result.Count;
        response.SendSuccess();
        return response;
    }
}
