using System.Text.Json.Serialization;

namespace AdminPlus.Logic;

public sealed class MapEntry
{
    [JsonPropertyName("Name")] public string Name { get; set; } = "";
    [JsonPropertyName("Label")] public string Label { get; set; } = "";
    [JsonPropertyName("WorkshopId")] public string WorkshopId { get; set; } = "";
}

public sealed class ModeDefinition
{
    [JsonPropertyName("Name")] public string Name { get; set; } = "";
    [JsonPropertyName("Label")] public string Label { get; set; } = "";
    [JsonPropertyName("GameType")] public int GameType { get; set; }
    [JsonPropertyName("GameMode")] public int GameMode { get; set; }
}

public static class DefaultCatalog
{
    private static MapEntry Map(string name, string label) => new() { Name = name, Label = label };

    public static List<MapEntry> Maps() => new()
    {
        Map("de_mirage", "Mirage"), Map("de_inferno", "Inferno"), Map("de_ancient", "Ancient"),
        Map("de_anubis", "Anubis"), Map("de_dust2", "Dust 2"), Map("de_nuke", "Nuke"),
        Map("de_overpass", "Overpass"), Map("de_vertigo", "Vertigo"),
    };

    public static List<ModeDefinition> Modes() => new()
    {
        new() { Name = "competitive", Label = "Competitive", GameType = 0, GameMode = 1 },
        new() { Name = "wingman", Label = "Wingman", GameType = 0, GameMode = 2 },
        new() { Name = "casual", Label = "Casual", GameType = 0, GameMode = 0 },
        new() { Name = "deathmatch", Label = "Deathmatch", GameType = 1, GameMode = 2 },
    };
}
