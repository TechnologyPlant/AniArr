using AniArr.Server.Entities;
using AniArr.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AniArr.Server.Endpoints;

public static class SonarrConfigEndpoints
{
    public static IEndpointRouteBuilder MapSonarrConfigGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAsync);
        group.MapPut("/", PutAsync);
        group.MapPut("/ConnectionDetails", PutConnectionDetailsAsync);
        group.MapGet("/ExternalDetails", GetExternalDetails);

        return group;
    }


    static async Task<IResult> GetAsync([FromServices] SonarrService sonarrService)
    {
        try
        {
            var config = await sonarrService.GetSonarrConfig();
            return Results.Ok(config);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

    static async Task<IResult> PutAsync([FromServices] SonarrService sonarrService, [FromBody] SonarrConfig request)
    {
        try
        {
            await sonarrService.SaveSonarrConfig(request);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

    static async Task<IResult> PutConnectionDetailsAsync([FromServices] SonarrService sonarrService, [FromBody] SonarrConnectionDetails sonarrConnectionDetails)
    {
        try
        {
            var result = await sonarrService.UpdateConnectionDetails(sonarrConnectionDetails);
            if (!result) return Results.BadRequest();
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

    static async Task<IResult> GetExternalDetails([FromServices] SonarrService sonarrService)
    {
        try
        {
            var config = await sonarrService.GetSonarrConfig();

            config.SonarrTags = await sonarrService.GetSonarrTags(config.SonarrConnectionDetails);
            config.QualityProfiles = await sonarrService.LoadQualityProfiles(config.SonarrConnectionDetails);
            config.RootFolders = await sonarrService.LoadRootFolders(config.SonarrConnectionDetails);

            return Results.Ok(config);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }

}
