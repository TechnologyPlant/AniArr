using AniArr.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AniArr.Server.Endpoints;

public static class FribbListEndpoints
{
    public static IEndpointRouteBuilder MapFribbListGroup(this RouteGroupBuilder group)
    {
        group.MapPut("/", PutAsync)
            .DisableAntiforgery();

        return group;
    }
   
    static async Task<IResult> PutAsync([FromServices] AniService aniService, [FromForm] IFormFile formFile, CancellationToken cancellationToken = default)
    {
        try
        {
            if (formFile is null)
                return Results.BadRequest("No file uploaded.");
            await aniService.StoreFribbItems(formFile);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }

        return Results.Ok();
    }
}
