using System.Text.Json.Serialization;
using AdminPlus.Logic;
using CounterStrikeSharp.API.Core;

namespace AdminPlus;

public class AdminPlusConfig : BasePluginConfig
{
    [JsonPropertyName("ConfigDirectory")] public string ConfigDirectory { get; set; } = "hstx";
    [JsonPropertyName("MenuType")] public string MenuType { get; set; } = "chat";
    [JsonPropertyName("Cvars")] public List<CvarDefinition> Cvars { get; set; } = DefaultCvars.Create();
    [JsonPropertyName("Maps")] public List<MapEntry> Maps { get; set; } = DefaultCatalog.Maps();
    [JsonPropertyName("Modes")] public List<ModeDefinition> Modes { get; set; } = DefaultCatalog.Modes();

    public void Normalize(List<string> problems)
    {
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
