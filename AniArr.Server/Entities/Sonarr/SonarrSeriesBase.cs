namespace AniArr.Server.Entities.Sonarr;

public abstract class SonarrSeriesBase
{
    public int tvdbId { get; set; }
    public List<Season> seasons { get; set; } = [];

    public class Season
    {
        public int seasonNumber { get; set; }
        public bool monitored { get; set; }

        public override string ToString()
        {
            return $"{{\"seasonNumber\":{seasonNumber}, \"monitored\":{monitored} }}";
        }
    }
}
