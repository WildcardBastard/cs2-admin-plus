using AdminPlus.Logic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AdminPlus;

public partial class AdminPlus
{
    internal void OpenPlayers(CCSPlayerController admin)
    {
        var menu = NewMenu("Spieler wählen");

        foreach (var target in AdminActions.HumanPlayers())
        {
            int userId = target.UserId!.Value;
            string name = target.PlayerName;
            menu.AddMenuOption(name, (p, _) => OpenPlayerActions(p, userId, name));
        }

        menu.AddMenuOption("Zurück", (p, _) => OpenMain(p));
        menu.Open(admin);
    }

    private void OpenPlayerActions(CCSPlayerController admin, int userId, string name)
    {
        var menu = NewMenu(name);

        void Add(string label, MenuAction action, string detail, Func<ActionResult> run, Action<CCSPlayerController> next)
        {
            if (!Can(admin, action)) return;
            menu.AddMenuOption(label, (p, _) =>
            {
                Execute(p, action, name, detail, run);
                next(p);
            });
        }

        void Stay(CCSPlayerController p) => OpenPlayerActions(p, userId, name);

        ActionResult Guarded(CCSPlayerController p, Func<ActionResult> run)
        {
            var check = CheckTarget(p, userId);
            return check.Result == Audit.Results.Ok ? run() : check;
        }

        Add("Kicken", MenuAction.Kick, "", () => Guarded(admin, () => AdminActions.Kick(userId)), OpenPlayers);
        if (Can(admin, MenuAction.Ban))
            menu.AddMenuOption("Bannen ...", (p, _) => OpenBanDurations(p, userId, name));
        Add("Muten / Entmuten", MenuAction.Mute, "", () => AdminActions.ToggleMute(userId), Stay);
        Add("Team: CT", MenuAction.SwitchTeam, "ct", () => AdminActions.SwitchTeam(userId, CsTeam.CounterTerrorist, "CT"), Stay);
        Add("Team: T", MenuAction.SwitchTeam, "t", () => AdminActions.SwitchTeam(userId, CsTeam.Terrorist, "T"), Stay);
        Add("Team: Zuschauer", MenuAction.SwitchTeam, "spec", () => AdminActions.SwitchTeam(userId, CsTeam.Spectator, "Zuschauer"), Stay);
        Add("Töten", MenuAction.Slay, "", () => AdminActions.Slay(userId), Stay);
        Add("Respawn", MenuAction.Respawn, "", () => AdminActions.Respawn(userId), Stay);

        menu.AddMenuOption("Zurück", (p, _) => OpenPlayers(p));
        menu.Open(admin);
    }

    private void OpenBanDurations(CCSPlayerController admin, int userId, string name)
    {
        var menu = NewMenu($"Bann: {name}");

        foreach (var (label, minutes) in new[] { ("1 Stunde", 60), ("1 Tag", 1440), ("7 Tage", 10080), ("Permanent", 0) })
        {
            menu.AddMenuOption(label, (p, _) =>
            {
                Execute(p, MenuAction.Ban, name, $"{minutes} min", () =>
                {
                    var check = CheckTarget(p, userId);
                    return check.Result == Audit.Results.Ok ? AdminActions.Ban(userId, minutes) : check;
                });
                OpenPlayers(p);
            });
        }

        menu.AddMenuOption("Zurück", (p, _) => OpenPlayerActions(p, userId, name));
        menu.Open(admin);
    }
}
