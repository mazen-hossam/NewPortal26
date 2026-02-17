using Microsoft.EntityFrameworkCore;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;
using TheBoys.Application.Extensions;
using TheBoys.Application.Misc;
using TheBoys.Domain.Entities;
using TheBoys.Infrastructure.Persistence;

namespace TheBoys.Infrastructure.Services;

public class NewsService : INewsService
{
    private readonly ApplicationDbContext _context;

    public NewsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResponse<List<NewsDto>>> PaginateSectorNewsAsync(
        PaginateNewsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.PageSize >= 10)
        {
            request.PageSize = 10;
        }

        var response = new PaginationResponse<List<NewsDto>>();
        var query = _context
            .News.AsNoTracking()
            .Include(x => x.NewsTranslations)
            .ThenInclude(x => x.Language)
            .Where(x =>
                x.Published &&
                x.NewsTranslations != null &&
                x.NewsTranslations.Any(t => t.LangId == request.LanguageId)
            )
            .Select(n => new
            {
                n.NewsId,
                n.NewsDate,
                n.IsFeatured,
                n.NewsImg,
                n.OwnerId,
                Translation = n.NewsTranslations.FirstOrDefault(t => t.LangId == request.LanguageId)
            });

        if (request.Search.HasValue())
        {
            query = query.Where(x => EF.Functions.Like(x.Translation.NewsHead, $"{request.Search}%"));
        }

        response.TotalCount = await query.CountAsync(cancellationToken);

        response.Result = await query
            .AsSplitQuery()
            .OrderByDescending(x => x.NewsDate)
            .Paginate(request.PageIndex, request.PageSize)
            .Where(x => x.Translation != null)
            .Select(x => new NewsDto
            {
                Id = x.NewsId,
                Date = x.NewsDate,
                IsFeatured = x.IsFeatured,
                NewsImg = StringExtensions.GetFullPath(x.OwnerId, x.NewsImg),
                NewsDetails = new NewsTranslationDto
                {
                    Id = x.Translation.Id,
                    LanguageId = x.Translation.LangId,
                    Head = StringExtensions.StripHtml(x.Translation.NewsHead),
                    Abbr = StringExtensions.StripHtml(x.Translation.NewsAbbr),
                    Body = StringExtensions.StripHtml(x.Translation.NewsBody),
                    Source = StringExtensions.StripHtml(x.Translation.NewsSource),
                    ImgAlt = StringExtensions.GetFullPath(x.OwnerId, x.Translation.ImgAlt)
                }
            })
            .ToListAsync(cancellationToken);

        response.Count = response.Result.Count;
        response.PageIndex = request.PageIndex;
        response.PageSize = request.PageSize;
        return response;
    }

    public async Task<PaginationResponse<List<NewsDto>>> PaginateUniversityNewsAsync(
        PaginateNewsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.PageSize >= 10)
        {
            request.PageSize = 10;
        }

        var response = new PaginationResponse<List<NewsDto>>();
        var query = _context
            .NewsUnivs.AsNoTracking()
            .Include(x => x.NewsUnivTranslations)
            .ThenInclude(x => x.Language)
            .Where(x =>
                x.Published &&
                x.NewsUnivTranslations != null &&
                x.NewsUnivTranslations.Any(t => t.LangId == request.LanguageId)
            )
            .Select(n => new
            {
                n.NewsId,
                n.NewsDate,
                n.IsFeatured,
                n.NewsImg,
                n.OwnerId,
                Translation = n.NewsUnivTranslations.FirstOrDefault(t => t.LangId == request.LanguageId)
            });

        if (request.Search.HasValue())
        {
            query = query.Where(x => EF.Functions.Like(x.Translation.NewsHead, $"{request.Search}%"));
        }

        response.TotalCount = await query.CountAsync(cancellationToken);

        response.Result = await query
            .AsSplitQuery()
            .OrderByDescending(x => x.NewsDate)
            .Paginate(request.PageIndex, request.PageSize)
            .Where(x => x.Translation != null)
            .Select(x => new NewsDto
            {
                Id = x.NewsId,
                Date = x.NewsDate,
                IsFeatured = x.IsFeatured,
                NewsImg = StringExtensions.GetFullPath(x.OwnerId, x.NewsImg),
                NewsDetails = new NewsTranslationDto
                {
                    Id = x.Translation.Id,
                    LanguageId = x.Translation.LangId,
                    Head = StringExtensions.StripHtml(x.Translation.NewsHead),
                    Abbr = StringExtensions.StripHtml(x.Translation.NewsAbbr),
                    Body = StringExtensions.StripHtml(x.Translation.NewsBody),
                    Source = StringExtensions.StripHtml(x.Translation.NewsSource),
                    ImgAlt = x.Translation.ImgAlt
                }
            })
            .ToListAsync(cancellationToken);

        response.Count = response.Result.Count;
        response.PageIndex = request.PageIndex;
        response.PageSize = request.PageSize;
        return response;
    }

    public async Task<ResponseOf<NewsDto>> GetSectorNewsByIdAsync(
        int id,
        int languageId,
        CancellationToken cancellationToken = default
    )
    {
        var response = new ResponseOf<NewsDto>();
        if (!await _context.NewsTranslations.AnyAsync(x => x.NewsId == id && x.LangId == languageId))
        {
            response.SendBadRequest("No information for news with your language");
            return response;
        }

        response.Result = await _context
            .News.AsNoTracking()
            .Include(x => x.NewsTranslations)
            .ThenInclude(x => x.Language)
            .Where(x => x.NewsId == id)
            .Select(news => new NewsDto
            {
                Id = news.NewsId,
                Date = news.NewsDate,
                IsFeatured = news.IsFeatured,
                NewsImg = StringExtensions.GetFullPath(news.OwnerId, news.NewsImg),
                NewsDetails =
                    news.NewsTranslations != null && news.NewsTranslations.Any()
                        ? news.NewsTranslations
                            .Select(t => new NewsTranslationDto
                            {
                                Id = t.Id,
                                Head = StringExtensions.StripHtml(t.NewsHead),
                                Abbr = StringExtensions.StripHtml(t.NewsAbbr),
                                Body = StringExtensions.StripHtml(t.NewsBody),
                                Source = StringExtensions.StripHtml(t.NewsSource),
                                ImgAlt = t.ImgAlt,
                                LanguageId = t.LangId
                            })
                            .FirstOrDefault(x => x.LanguageId == languageId)
                        : null,
                Languages = news
                    .NewsTranslations.Select(x => new LanguageModel
                    {
                        Id = x.Language.Id,
                        Code = x.Language.LCID
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        foreach (var language in response.Result.Languages)
        {
            var exactLanguage = StaticLanguages.LanguageModels.FirstOrDefault(x =>
                x.Code.Trim().ToLower() == language.Code.Trim().ToLower()
            );

            if (exactLanguage is null)
            {
                continue;
            }

            language.Flag = exactLanguage.Flag;
            language.Name = exactLanguage.Name;
        }

        return response;
    }

    public async Task<ResponseOf<NewsDto>> GetUniversityNewsByIdAsync(
        int id,
        int languageId,
        CancellationToken cancellationToken = default
    )
    {
        var response = new ResponseOf<NewsDto>();
        if (
            !await _context.NewsUnivsTranslations.AnyAsync(
                x => x.NewsId == id && x.LangId == languageId,
                cancellationToken
            )
        )
        {
            response.SendBadRequest("No information for news with your language");
            return response;
        }

        response.Result = await _context
            .NewsUnivs.AsNoTracking()
            .Include(x => x.NewsUnivTranslations)
            .ThenInclude(x => x.Language)
            .Where(x => x.NewsId == id)
            .Select(news => new NewsDto
            {
                Id = news.NewsId,
                Date = news.NewsDate,
                IsFeatured = news.IsFeatured,
                NewsImg = StringExtensions.GetFullPath(news.OwnerId, news.NewsImg),
                NewsDetails =
                    news.NewsUnivTranslations != null && news.NewsUnivTranslations.Any()
                        ? news.NewsUnivTranslations
                            .Where(t => t.LangId == languageId)
                            .Select(t => new NewsTranslationDto
                            {
                                Id = t.Id,
                                Head = StringExtensions.StripHtml(t.NewsHead),
                                Abbr = StringExtensions.StripHtml(t.NewsAbbr),
                                Body = StringExtensions.StripHtml(t.NewsBody),
                                Source = StringExtensions.StripHtml(t.NewsSource),
                                ImgAlt = t.ImgAlt,
                                LanguageId = t.LangId
                            })
                            .FirstOrDefault()
                        : null,
                Languages = news
                    .NewsUnivTranslations.Select(t => new LanguageModel
                    {
                        Id = t.Language.Id,
                        Code = t.Language.LCID
                    })
                    .Distinct()
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (response.Result?.Languages != null)
        {
            foreach (var language in response.Result.Languages)
            {
                var exactLanguage = StaticLanguages.LanguageModels.FirstOrDefault(x =>
                    x.Code.Trim().ToLower() == language.Code.Trim().ToLower()
                );
                if (exactLanguage == null)
                {
                    continue;
                }

                language.Flag = exactLanguage.Flag;
                language.Name = exactLanguage.Name;
            }
        }

        return response;
    }

    public async Task<ResponseOf<List<NewsDto>>> SearchByOwnerAbbreviationAsync(
        string abbreviation,
        int languageId,
        CancellationToken cancellationToken = default
    )
    {
        var response = new ResponseOf<List<NewsDto>>();
        var ownerKeywords = new Dictionary<Guid, string[]>
        {
            {
                Guid.Parse("4afac4c7-cab4-4112-aed7-034ba2b541c3"),
                new[] { "wafiden", "وافدين" }
            },
            {
                Guid.Parse("ae7c7b7e-0343-4a44-87a3-1af0923d9a2f"),
                new[] { "cenev", "مركز CENEVA" }
            },
            {
                Guid.Parse("62d7ddf5-eee9-4aa3-b30d-57975e6a9ca8"),
                new[] { "educ", "قطاع التعليم" }
            },
            { Guid.Parse("016806d3-46c3-4131-baa0-035064ac119b"), new[] { "env", "شؤون البيئة" } },
            {
                Guid.Parse("81ad5631-aa92-4e75-94dd-f19cbcaba33a"),
                new[] { "env2", "إدارة شؤون البيئة" }
            },
            {
                Guid.Parse("b1222730-a569-4e21-94c9-5d29c9351213"),
                new[] { "nci", "المركز القومي للمعلومات" }
            },
            {
                Guid.Parse("333b4686-0ca7-41dd-8f9a-03d1ecc61627"),
                new[] { "postgrad", "الدراسات العليا" }
            },
            {
                Guid.Parse("c5ed9861-f000-472f-8b1a-8463a7e8a126"),
                new[] { "sadat", "جامعة السادات" }
            },
            {
                Guid.Parse("d2dfafec-0dd2-4960-a876-6579670a3f83"),
                new[] { "secr", "الأمانة العامة" }
            },
            {
                Guid.Parse("6d1eb652-e500-4e45-ae8c-b6f48ea2e927"),
                new[] { "tico", "مركز تكنولوجيا المعلومات" }
            },
            {
                Guid.Parse("b9c7a805-ed88-425c-8763-db283c4cc92b"),
                new[] { "univpres", "رئاسة الجامعة" }
            }
        };

        var abbreviationLower = abbreviation.Trim().ToLower();
        var ownerEntry = ownerKeywords.FirstOrDefault(kvp =>
            kvp.Value.Any(k => k.Trim().ToLower() == abbreviationLower)
        );
        if (ownerEntry.Equals(default(KeyValuePair<Guid, string[]>)))
        {
            response.SendBadRequest("Invalid owner abbreviation");
            return response;
        }

        var ownerId = ownerEntry.Key;
        var newsList = await _context
            .News.AsNoTracking()
            .Include(n => n.NewsTranslations)
            .ThenInclude(t => t.Language)
            .Where(n => n.OwnerId == ownerId && n.NewsTranslations.Any(t => t.LangId == languageId))
            .OrderByDescending(n => n.NewsDate)
            .ToListAsync(cancellationToken);

        if (!newsList.Any())
        {
            response.Result = new List<NewsDto>();
            response.SendSuccess("No news found");
            return response;
        }

        response.Result = newsList
            .Select(news =>
            {
                var translation = news.NewsTranslations.FirstOrDefault(t => t.LangId == languageId);
                if (translation == null)
                {
                    return null;
                }

                return new NewsDto
                {
                    Id = news.NewsId,
                    Date = news.NewsDate,
                    IsFeatured = news.IsFeatured,
                    NewsImg = StringExtensions.GetFullPath(news.OwnerId, news.NewsImg),
                    NewsDetails = new NewsTranslationDto
                    {
                        Id = translation.Id,
                        Head = StringExtensions.StripHtml(translation.NewsHead),
                        Abbr = StringExtensions.StripHtml(translation.NewsAbbr),
                        Body = StringExtensions.StripHtml(translation.NewsBody),
                        Source = StringExtensions.StripHtml(translation.NewsSource),
                        ImgAlt = translation.ImgAlt,
                        LanguageId = translation.LangId
                    },
                    Languages = news
                        .NewsTranslations.Select(l => new LanguageModel
                        {
                            Id = l.Language.Id,
                            Code = l.Language.LCID
                        })
                        .Distinct()
                        .ToList()
                };
            })
            .Where(x => x != null)
            .ToList();

        response.Success = true;
        return response;
    }
}
