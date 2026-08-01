using KodiakPlugBank.Application.Common;

namespace KodiakPlugBank.Api;

public static class ApiResponse
{
    public static IResult From(Result result) =>
        result.IsSuccess ? Results.Ok() : Error(result);

    public static IResult From<T>(Result<T> result) =>
        result.IsSuccess ? Results.Ok(result.Value) : Error(result);

    private static IResult Error(Result result) =>
        Results.Json(
            new { code = result.StatusCode, message = result.Error },
            statusCode: result.StatusCode ?? StatusCodes.Status400BadRequest);
}
