using AdminPlus.Logic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;

namespace AdminPlus;

public partial class AdminPlus
{
    private IMenu NewMenu(string title) =>
        Config.MenuType == "center" ? new CenterHtmlMenu(title, this) : new ChatMenu(title);

    private void OnAdminCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null)
        {
            info.ReplyToCommand("[AdminPlus] !admin geht nur im Spiel.");
            return;
        }

        if (!Permissions.CanOpenMenu(flag => AdminManager.PlayerHasPermissions(player, flag)))
        {
            player.PrintToChat($" {ChatColors.Red}[Admin]{ChatColors.Default} Kein Zugriff.");
            return;
        }

        OpenMain(player);
    }

    internal void OpenMain(CCSPlayerController admin)
    {
        var menu = NewMenu("HSTX Admin");

        if (Can(admin, MenuAction.Kick) || Can(admin, MenuAction.Ban))
            menu.AddMenuOption("Spieler", (p, _) => OpenPlayers(p));
        if (Can(admin, MenuAction.ChangeMap) || Can(admin, MenuAction.ChangeMode))
            menu.AddMenuOption("Map und Modus", (p, _) => OpenMaps(p));
        if (Can(admin, MenuAction.ChangeCvar) || Can(admin, MenuAction.LoadConfig))
            menu.AddMenuOption("Einstellungen", (p, _) => OpenSettings(p));
        if (Can(admin, MenuAction.MatchPause) || Can(admin, MenuAction.MatchRestart))
            menu.AddMenuOption("Match", (p, _) => OpenMatch(p));

        menu.Open(admin);
    }
}
