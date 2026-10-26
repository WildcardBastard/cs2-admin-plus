using System.Text.Json;
using AdminPlus.Logic;
using Xunit;

namespace AdminPlus.Tests;

public class ConfigJsonTests
{
    private static AdminPlusConfig Load(string json) =>
        JsonSerializer.Deserialize<AdminPlusConfig>(json)!;

    [Fact]
    public void Default_config_round_trips_and_is_clean()
    {
        var json = JsonSerializer.Serialize(new AdminPlusConfig());
        var config = Load(json);

        var problems = new List<string>();
        config.Normalize(problems);

        Assert.Empty(problems);
        Assert.Equal(DefaultCvars.Create().Count, config.Cvars.Count);
        Assert.Equal("hstx", config.ConfigDirectory);
        Assert.Equal("chat", config.MenuType);
        Assert.Contains("\"Kind\":\"Choice\"", json);
    }

    [Fact]
    public void Empty_json_object_falls_back_to_the_defaults()
    {
        var config = Load("{}");
        var problems = new List<string>();
        config.Normalize(problems);

        Assert.Empty(problems);
        Assert.NotEmpty(config.Cvars);
        Assert.NotEmpty(config.Maps);
        Assert.NotEmpty(config.Modes);
    }

    [Fact]
    public void Null_lists_do_not_crash_normalization()
    {
        var config = Load("{\"Cvars\":null,\"Maps\":null,\"Modes\":null}");
        var problems = new List<string>();
        config.Normalize(problems);

        Assert.Empty(config.Cvars);
        Assert.Empty(config.Maps);
        Assert.Empty(config.Modes);
    }

    [Fact]
    public void Bad_entries_are_dropped_with_a_problem_each()
    {
        var config = Load("""
        {
          "ConfigDirectory": "../etc",
          "MenuType": "popup",
          "Cvars": [ { "Name": "sv_cheats", "Kind": "Bool" }, { "Name": "mp_maxrounds", "Kind": "Choice", "Values": ["12"] } ],
          "Maps": [ { "Name": "de_mirage;quit" }, { "Name": "de_nuke" }, null ],
          "Modes": [ { "Name": "x", "GameType": 42, "GameMode": 0 }, { "Name": "ok", "GameType": 0, "GameMode": 1 } ]
        }
        """);

        var problems = new List<string>();
        config.Normalize(problems);

        Assert.Equal("hstx", config.ConfigDirectory);
        Assert.Equal("chat", config.MenuType);
        Assert.Equal(new[] { "mp_maxrounds" }, config.Cvars.Select(c => c.Name));
        Assert.Equal(new[] { "de_nuke" }, config.Maps.Select(m => m.Name));
        Assert.Equal(new[] { "ok" }, config.Modes.Select(m => m.Name));
        Assert.Equal(5, problems.Count);
    }
}
