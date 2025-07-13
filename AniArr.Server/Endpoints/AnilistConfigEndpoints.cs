using AniArr.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AniArr.Server.Endpoints;

public static class AnilistConfigEndpoints
{
    public static IEndpointRouteBuilder MapAnilistConfigGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAsync);
        group.MapPut("/", PutAsync);

        return group;
    }
    static async Task<IResult> GetAsync([FromServices] AniService aniService, CancellationToken cancellationToken = default)
    {
        try
        {
            var config = await aniService.GetConfigAsync(cancellationToken);
            return Results.Ok(config);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

    static async Task<IResult> PutAsync([FromBody] string username, [FromServices] AniService aniService, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(username))
                return Results.BadRequest();

            await aniService.SetAniListUserNameAsync(username, cancellationToken);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }
}
