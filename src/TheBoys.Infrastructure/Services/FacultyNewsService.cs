using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Globalization;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Common.Requests;
using TheBoys.Application.Common.Responses;
using TheBoys.Application.Dtos;
using TheBoys.Application.Extensions;
using TheBoys.Application.FacultyNews;
using TheBoys.Application.Misc;
using TheBoys.Domain.Entities;
using TheBoys.Infrastructure.Persistence;

namespace TheBoys.Infrastructure.Services;

public sealed class FacultyNewsService : IFacultyNewsService
{
    private const int MaxPageSize = 10;
    private const string SchemaName = "dbo";
    private readonly ApplicationDbContext _context;

    public FacultyNewsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResponse<List<FacultyNewsItemDto>>> GetPublishedAsync(
        int publicFacultyCode,
        int langId,
        PaginateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        request ??= new PaginateRequest();
        NormalizePagination(request);

        var response = new PaginationResponse<List<FacultyNewsItemDto>>
        {
            Result = new List<FacultyNewsItemDto>(),
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };

        if (!FacultyNewsDictionary.TryGetFaculty(publicFacultyCode, out var config))
        {
            response.SendBadRequest("Requested resource was not found.");
            return response;
        }

        var query = BuildListQuery(config);
        var countQuery = BuildCountQuery(config);

        response.TotalCount = await ExecuteScalarAsync<int>(
            countQuery,
            command =>
            {
                AddParameter(command, "@LangId", langId, DbType.Int32);
                AddSearchParameter(command, request.Search);
            },
            cancellationToken
        );

        await ExecuteReaderAsync(
            query,
            command =>
            {
                AddParameter(command, "@LangId", langId, DbType.Int32);
                AddSearchParameter(command, request.Search);
                AddParameter(command, "@Offset", (request.PageIndex - 1) * request.PageSize, DbType.Int32);
                AddParameter(command, "@PageSize", request.PageSize, DbType.Int32);
            },
            reader =>
            {
                response.Result.Add(ReadItem(reader, config));
            },
            cancellationToken
        );

        response.Count = response.Result.Count;
        response.Success = true;
        return response;
    }

    public async Task<ResponseOf<FacultyNewsDetailsDto>> GetDetailsAsync(
        int publicFacultyCode,
        int id,
        int langId,
        CancellationToken cancellationToken = default
    )
    {
        var response = new ResponseOf<FacultyNewsDetailsDto>();

        if (!FacultyNewsDictionary.TryGetFaculty(publicFacultyCode, out var config))
        {
            response.SendBadRequest("Requested resource was not found.");
            return response;
        }

        var query = BuildDetailsQuery(config);
        FacultyNewsDetailsDto result = null;

        await ExecuteReaderAsync(
            query,
            command =>
            {
                AddParameter(command, "@Id", id, DbType.Int32);
                AddParameter(command, "@LangId", langId, DbType.Int32);
            },
            reader =>
            {
                result = ReadDetails(reader, config);
            },
            cancellationToken
        );

        if (result is null)
        {
            response.SendBadRequest("Requested resource was not found.");
            return response;
        }

        response.Result = result;
        response.Success = true;
        return response;
    }

    private async Task ExecuteReaderAsync(
        string query,
        Action<DbCommand> configureCommand,
        Action<DbDataReader> readRow,
        CancellationToken cancellationToken
    )
    {
        var connection = _context.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            configureCommand(command);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                readRow(reader);
            }
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private async Task<T> ExecuteScalarAsync<T>(
        string query,
        Action<DbCommand> configureCommand,
        CancellationToken cancellationToken
    )
    {
        var connection = _context.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            configureCommand(command);

            var value = await command.ExecuteScalarAsync(cancellationToken);
            return value == null || value == DBNull.Value
                ? default
                : (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static string BuildListQuery(FacultyNewsConfig config)
    {
        var newsTable = GetQualifiedTableName(config.NewsTable);
        var translationTable = GetQualifiedTableName(config.TranslationTable);

        return $"""
            SELECT
                n.[News_Id] AS [Id],
                n.[News_date] AS [Date],
                n.[currentNews_date] AS [CurrentDate],
                n.[News_img] AS [Image],
                t.[News_Head] AS [Title],
                t.[News_Source] AS [Source],
                t.[Img_alt] AS [ImageAlt]
            FROM {newsTable} AS n
            INNER JOIN {translationTable} AS t
                ON n.[News_Id] = t.[News_Id]
            WHERE n.[Published] = 1
                AND t.[Lang_Id] = @LangId
                AND (@Search IS NULL OR t.[News_Head] LIKE @Search)
            ORDER BY n.[News_date] DESC, n.[News_Id] DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;
    }

    private static string BuildCountQuery(FacultyNewsConfig config)
    {
        var newsTable = GetQualifiedTableName(config.NewsTable);
        var translationTable = GetQualifiedTableName(config.TranslationTable);

        return $"""
            SELECT COUNT(1)
            FROM {newsTable} AS n
            INNER JOIN {translationTable} AS t
                ON n.[News_Id] = t.[News_Id]
            WHERE n.[Published] = 1
                AND t.[Lang_Id] = @LangId
                AND (@Search IS NULL OR t.[News_Head] LIKE @Search);
            """;
    }

    private static string BuildDetailsQuery(FacultyNewsConfig config)
    {
        var newsTable = GetQualifiedTableName(config.NewsTable);
        var translationTable = GetQualifiedTableName(config.TranslationTable);

        return $"""
        SELECT
            n.[News_Id] AS [Id],
            n.[News_date] AS [Date],
            n.[currentNews_date] AS [CurrentDate],
            n.[News_img] AS [Image],
            t.[News_Head] AS [Title],
            t.[News_Source] AS [Source],
            t.[Img_alt] AS [ImageAlt],
            t.[News_Body] AS [Body],
            -- ????? ?? ?? ???? ????? ??? ???? ????? ?????? ?????? (?????: "1,2")
            (SELECT STRING_AGG(CAST(tr.[Lang_Id] AS VARCHAR), ',') 
             FROM {translationTable} AS tr 
             WHERE tr.[News_Id] = n.[News_Id]) AS [AvailableLanguages]
        FROM {newsTable} AS n
        INNER JOIN {translationTable} AS t
            ON n.[News_Id] = t.[News_Id]
        WHERE n.[Published] = 1
            AND n.[News_Id] = @Id
            AND t.[Lang_Id] = @LangId;
        """;
    }

    private static string GetQualifiedTableName(string tableName)
    {
        if (
            string.IsNullOrWhiteSpace(tableName)
            || tableName.Any(c => !char.IsLetterOrDigit(c) && c != '_')
        )
        {
            throw new InvalidOperationException("Invalid faculty news configuration.");
        }

        return $"[{SchemaName}].[{tableName}]";
    }

    private static void AddParameter(DbCommand command, string name, object value, DbType dbType)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = dbType;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static void AddSearchParameter(DbCommand command, string search)
    {
        AddParameter(
            command,
            "@Search",
            search.HasValue() ? $"{search.Trim()}%" : DBNull.Value,
            DbType.String
        );
    }

    private static FacultyNewsItemDto ReadItem(DbDataReader reader, FacultyNewsConfig config)
    {
        return new FacultyNewsItemDto
        {
            Id = GetInt32(reader, "Id"),
            Title = StringExtensions.StripHtml(GetString(reader, "Title")),
            Date = GetNullableDateTime(reader, "Date"),
            CurrentDate = GetNullableDateTime(reader, "CurrentDate"),
            Image = GetFacultyImagePath(config, GetString(reader, "Image")),
            Source = StringExtensions.StripHtml(GetString(reader, "Source")),
            ImageAlt = GetFacultyImagePath(config, GetString(reader, "ImageAlt"))
        };
    }

    private static FacultyNewsDetailsDto ReadDetails(DbDataReader reader, FacultyNewsConfig config)
    {
        var details = new FacultyNewsDetailsDto
        {
            Id = GetInt32(reader, "Id"),
            Title = StringExtensions.StripHtml(GetString(reader, "Title")),
            Date = GetNullableDateTime(reader, "Date"),
            CurrentDate = GetNullableDateTime(reader, "CurrentDate"),
            Image = GetFacultyImagePath(config, GetString(reader, "Image")),
            Source = StringExtensions.StripHtml(GetString(reader, "Source")),
            ImageAlt = GetFacultyImagePath(config, GetString(reader, "ImageAlt")),
            Body = StringExtensions.StripHtml(GetString(reader, "Body")),
            Languages = new List<LanguageModel>() // ????? ???????
        };

        // ????? ?????? ???? ???? ?? ??? SQL (?? "1,2")
        var langIdsString = GetString(reader, "AvailableLanguages");

        if (!string.IsNullOrEmpty(langIdsString))
        {
            // ????? ???? ?????? (1 ? 2)
            var langIds = langIdsString.Split(',').Select(int.Parse).Distinct();

            foreach (var langId in langIds)
            {
                // ????? ???? ?? ????? ??????? ??????? ???? ???? ?? ???????
                var staticLang = StaticLanguages.LanguageModels.FirstOrDefault(x => x.Id == langId);

                details.Languages.Add(new LanguageModel
                {
                    Id = langId,
                    Code = staticLang?.Code ?? (langId == 1 ? "ar-EG" : "en-US"),
                    Name = staticLang?.Name ?? (langId == 1 ? "Arabic" : "English"),
                    Flag = staticLang?.Flag // ??? ????? ?????!
                });
            }
        }

        return details;
    }

    private static string GetFacultyImagePath(FacultyNewsConfig config, string imageName)
    {
        return StringExtensions.GetFullPath(
            Guid.Empty,
            imageName,
            GetFacultyImageBasePath(config)
        );
    }

    private static string GetFacultyImageBasePath(FacultyNewsConfig config)
    {
        return $"https://mu.menofia.edu.eg/PrtlFiles/Faculties/{config.Abbr}/Portal/Images/";
    }

    private static void NormalizePagination(PaginateRequest request)
    {
        if (request.PageIndex < 1)
        {
            request.PageIndex = 1;
        }

        if (request.PageSize < 1)
        {
            request.PageSize = 1;
        }

        if (request.PageSize > MaxPageSize)
        {
            request.PageSize = MaxPageSize;
        }
    }

    private static int GetInt32(DbDataReader reader, string name)
    {
        var value = reader[name];
        return value == DBNull.Value ? 0 : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static DateTime? GetNullableDateTime(DbDataReader reader, string name)
    {
        var value = reader[name];
        return value == DBNull.Value
            ? null
            : Convert.ToDateTime(value, CultureInfo.InvariantCulture);
    }

    private static string GetString(DbDataReader reader, string name)
    {
        var value = reader[name];
        return value == DBNull.Value
            ? string.Empty
            : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
}
