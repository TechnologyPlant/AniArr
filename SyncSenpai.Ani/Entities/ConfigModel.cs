namespace SyncSenpai.Ani.Entities;

public class ConfigModel
{
    public int Id { get; init; } = 1;
    public string UserName { get; set; } = "";

    public DateTime? LookupLastUpdated { get; set; }
}
