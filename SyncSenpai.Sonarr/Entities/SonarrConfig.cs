namespace SyncSenpai.Sonarr.Entities;

public class SonarrConfig
{
    public int Id { get; init; } = 1;
    public string SonarrUrl { get; set; } = "";
    public string SonarrApiKey { get; set; } = "";
    public AnimeDistinguisher AnimeDistinguisher { get; set; } = AnimeDistinguisher.Undefined;

    public SonarrTag SonarrTag { get; set; } = new() { Id = -1, Name = "Select a tag" };
    public string? AnimeDirectory { get; set; }

}
