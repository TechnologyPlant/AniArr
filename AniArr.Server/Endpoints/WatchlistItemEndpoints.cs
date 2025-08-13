using AniArr.Server.Entities;
using AniArr.Server.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;

namespace AniArr.Server.Endpoints;

public static class WatchlistItemEndpoints
{
    public static IEndpointRouteBuilder MapWatchlistGroup(this RouteGroupBuilder group)
    {
        group.MapPost("/Refresh", RefreshWatchListItems);
        group.MapGet("/", GetWatchlistItems);
        group.MapDelete("/", DeleteWatchlistItems);
        group.MapPut("/", PutWatchlistItem);

        return group;
    }

    static async Task<IResult> RefreshWatchListItems([FromServices] AniService aniService, CancellationToken cancellationToken = default)
    {
        try
        {
            await aniService.GetUpdatedWatchlistEntries(cancellationToken);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

    static async Task<IResult> GetWatchlistItems([FromServices] AniService aniService, [FromQuery] bool managed, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await aniService.GetWatchlistEntries()
                .Where(x => x.AniListItems.Any(y => y.Managed == managed))
                .Skip(--page * pageSize)
                .Take(pageSize)
                .Select(x => new WatchlistItem()
                {
                    Title = x.Title,
                    TvdbId = x.TvdbId,
                    AniListItems = x.AniListItems.Where(x => x.Managed == managed).ToList(),
                })
                .ToListAsync(cancellationToken);

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
    static async Task<IResult> PutWatchlistItem([FromServices] AniService aniService, [FromBody] WatchlistItem watchlistItem, CancellationToken cancellationToken = default)
    {
        try
        {
            await aniService.PutWatchlistItem(watchlistItem, cancellationToken);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

}
