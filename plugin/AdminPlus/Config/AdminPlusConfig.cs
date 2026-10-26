using System.Text.Json.Serialization;
using AdminPlus.Logic;
using CounterStrikeSharp.API.Core;

namespace AdminPlus;

public class AdminPlusConfig : BasePluginConfig
{
    [JsonPropertyName("ConfigDirectory")]
    [JsonConverter(typeof(TolerantStringConverter))]
    public string ConfigDirectory { get; set; } = "hstx";

    [JsonPropertyName("MenuType")]
    [JsonConverter(typeof(TolerantStringConverter))]
    public string MenuType { get; set; } = "chat";

    [JsonPropertyName("Cvars")]
    [JsonConverter(typeof(TolerantListConverter<CvarDefinition>))]
    public List<CvarDefinition> Cvars { get; set; } = DefaultCvars.Create();

    [JsonPropertyName("Maps")]
    [JsonConverter(typeof(TolerantListConverter<MapEntry>))]
    public List<MapEntry> Maps { get; set; } = DefaultCatalog.Maps();

    [JsonPropertyName("Modes")]
    [JsonConverter(typeof(TolerantListConverter<ModeDefinition>))]
    public List<ModeDefinition> Modes { get; set; } = DefaultCatalog.Modes();

    /// <summary>Entries the tolerant JSON converters could not read become markers; drop them with one log line each.</summary>
    private static void DropUnreadable<T>(List<T>? list, string what, List<string> problems) where T : class, INamedEntry
    {
        int dropped = list?.RemoveAll(e => e != null && e.Name == TolerantJson.InvalidName) ?? 0;
        for (int i = 0; i < dropped; i++)
            problems.Add($"{what}-Eintrag konnte nicht gelesen werden (falscher JSON-Typ oder unbekannter Wert), übersprungen");
    }

    public void Normalize(List<string> problems)
    {
        DropUnreadable(Cvars, "Cvar", problems);
        DropUnreadable(Maps, "Map", problems);
        DropUnreadable(Modes, "Modus", problems);

        Cvars = CvarWhitelist.Sanitize(Cvars, problems);

        Maps = (Maps ?? new List<MapEntry>()).Where(m => m != null).ToList();
        Maps.RemoveAll(m =>
        {
            var ok = MapCommands.TryBuild(m, out _);
            if (!ok) problems.Add($"Map '{m.Name}': ungültig");
            return !ok;
        });
        foreach (var m in Maps.Where(m => string.IsNullOrWhiteSpace(m.Label))) m.Label = m.Name;

        Modes = (Modes ?? new List<ModeDefinition>()).Where(m => m != null).ToList();
        Modes.RemoveAll(m =>
        {
            var ok = MapCommands.TryBuildMode(m, "de_dust2", out _);
            if (!ok) problems.Add($"Modus '{m.Name}': ungültig");
            return !ok;
        });
        foreach (var m in Modes.Where(m => string.IsNullOrWhiteSpace(m.Label))) m.Label = m.Name;

        if (!ConfigCatalog.IsValidFolder(ConfigDirectory))
        {
            problems.Add($"ConfigDirectory '{ConfigDirectory}' ungültig, verwende 'hstx'");
            ConfigDirectory = "hstx";
        }

        if (MenuType != "chat" && MenuType != "center")
        {
            problems.Add($"MenuType '{MenuType}' ungültig, verwende 'chat'");
            MenuType = "chat";
        }
    }
}
