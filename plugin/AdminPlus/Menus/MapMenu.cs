using AdminPlus.Logic;
using CounterStrikeSharp.API.Core;

namespace AdminPlus;

public partial class AdminPlus
{
    internal void OpenMaps(CCSPlayerController admin)
    {
        var menu = NewMenu("Map und Modus");

        if (Can(admin, MenuAction.ChangeMap))
        {
            foreach (var map in Config.Maps)
            {
                var entry = map;
                menu.AddMenuOption(entry.Label, (p, _) =>
                    Execute(p, MenuAction.ChangeMap, string.IsNullOrEmpty(entry.WorkshopId) ? entry.Name : entry.WorkshopId, "",
                        () => AdminActions.ChangeMap(entry)));
            }
        }

        if (Can(admin, MenuAction.ChangeMode))
        {
            foreach (var mode in Config.Modes)
            {
                var entry = mode;
                menu.AddMenuOption($"Modus: {entry.Label}", (p, _) =>
                    Execute(p, MenuAction.ChangeMode, entry.Name, "", () => AdminActions.ChangeMode(entry)));
            }
        }

        menu.AddMenuOption("Zurück", (p, _) => OpenMain(p));
        menu.Open(admin);
    }
}
