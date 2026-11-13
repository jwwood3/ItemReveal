using InControl;
using ItemChanger;
using MapChanger;
using MapChanger.Input;
using MapChanger.MonoBehaviours;
using System;

namespace ItemReveal.Input;

internal abstract class IRWorldMapInput(string name, Func<PlayerAction> getPlayerAction, float holdMilliseconds = 0)
    : WorldMapInput(name, "ItemReveal", getPlayerAction, holdMilliseconds)
{
    public override bool UseCondition()
    {
        return ItemChangerMod.Modules?.Get<IRModule>() is not null;
    }

    public override bool ActiveCondition()
    {
        return base.ActiveCondition()
            && MapChangerMod.IsEnabled()
            && ModeManager.CurrentMode()?.Mod == "RandoMapMod";
    }
}