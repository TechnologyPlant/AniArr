using AniArr.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AniArr.Server.Endpoints;

public static class FribbListEndpoints
{
    public static IEndpointRouteBuilder MapFribbListGroup(this RouteGroupBuilder group)
    {
        group.MapPut("/", PutAsync);

        return group;
    }
   
    static async Task<IResult> PutAsync([FromServices] AniService aniService, [FromBody] HttpRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var form = await request.ReadFormAsync();
            var file = form.Files["file"];
            if (file is null)
                return Results.BadRequest("No file uploaded.");
            await aniService.StoreFribbItems(file);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }

        return Results.Ok();
    }
}
