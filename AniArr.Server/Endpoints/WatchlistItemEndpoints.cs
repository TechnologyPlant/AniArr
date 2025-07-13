using AniArr.Server.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;

namespace AniArr.Server.Endpoints;

public static class WatchlistItemEndpoints
{
    public static IEndpointRouteBuilder MapWatchlistGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/new", GetNewWatchlistItems);
        group.MapGet("/", GetWatchlistItems);
        group.MapDelete("/", DeleteWatchlistItems);

        return group;
    }

    static async Task<IResult> GetNewWatchlistItems([FromServices] AniService aniService, CancellationToken cancellationToken = default)
    {
        try
        {
            return Results.Ok(await aniService.GetUpdatedWatchlistEntries(cancellationToken));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

    static async Task<IResult> GetWatchlistItems([FromServices] AniService aniService, [FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await aniService.GetWatchlistEntries().Skip(skip).Take(take).ToListAsync(cancellationToken);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }
    static async Task<IResult> DeleteWatchlistItems([FromServices] AniService aniService, CancellationToken cancellationToken = default)
    {
        try
        {
            await aniService.DeleteAllWatchListItem(cancellationToken);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

}
