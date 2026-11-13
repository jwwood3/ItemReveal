using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using System.Collections.Generic;

namespace ItemReveal;

internal static class RandoSettingsManagerInterop
{
    public static void Hook()
    {
        RandoSettingsManagerMod.Instance.RegisterConnection(new IRSettingsProxy());
    }
}

internal class IRSettingsProxy : RandoSettingsProxy<IRSettings, string>
{
    public override string ModKey => nameof(ItemReveal);
    public override VersioningPolicy<string> VersioningPolicy { get; } =
        new EqualityVersioningPolicy<string>(ItemReveal.Instance.GetVersion());
    public override void ReceiveSettings(IRSettings settings)
    {
        settings ??= new();
        IRMenuPage.Instance.Mef.SetMenuValues(settings);
        ItemReveal.GS.RevealedItems = new HashSet<string>(settings.RevealedItems);
        foreach (var item in IRMenuPage.Instance.itemButtons.Keys)
        {
            IRMenuPage.Instance.itemButtons[item].SetValue(settings.RevealedItems.Contains(item));
        }
    }

    public override bool TryProvideSettings(out IRSettings settings)
    {
        settings = ItemReveal.GS;
        return settings.Any;
    }
}