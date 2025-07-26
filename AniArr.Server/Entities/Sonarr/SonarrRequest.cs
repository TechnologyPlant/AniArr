namespace AniArr.Server.Entities.Sonarr;

public class SonarrRequest : SonarrSeriesBase
{
    public int QualityProfileId { get; set; }
    public int RootFolderPath { get; set; }
    public string ToPostRequestBody()
    {
        return $"{{" +
            $"\"tvdbId\":{tvdbId}," +
            $"\"qualityProfileId\":{QualityProfileId}," +
            $"\"rootFolderPath\":{RootFolderPath}," +
            $"\"monitored\":true," +
            $"\"seasons\":[{string.Join("", seasons.Select(x => x.ToString()))}]," +
            $"  \"addOptions\":{{ \"searchForMissingEpisodes\":true }}" +
            $"}}";
    }
}