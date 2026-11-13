using Modding;
using RandomizerMod.Logging;
using RandomizerMod.RC;
using System.IO;
using ItemChanger;
using System.Collections.Generic;

namespace ItemReveal;

public class ItemReveal : Mod, IGlobalSettings<IRSettings>
{
    public ItemReveal()
    {
        Instance = this;
    }
    internal static ItemReveal Instance { get; private set; }
    internal static IRSettings GS { get; private set; } = new();

    public void OnLoadGlobal(IRSettings s)
    {
        GS = s;
    }

    public IRSettings OnSaveGlobal()
    {
        return GS;
    }

    public override string GetVersion()
    {
        return "0.0.1";
    }

    public override int LoadPriority()
    {
        return 10;
    }

    public override void Initialize()
    {
        LogDebug("Initializing");

        if (ModHooks.GetMod("Randomizer 4") is null || ModHooks.GetMod("RandoMapCoreMod") is null)
        {
            return;
        }

        IRMenuPage.Hook();
        RandoController.OnExportCompleted += OnExport;
        RandoController.OnCalculateHash += AdjustHash;
        SettingsLog.AfterLogSettings += LogIRSettings;

        if (ModHooks.GetMod("RandoSettingsManager") is not null)
        {
            RandoSettingsManagerInterop.Hook();
        }

        LogDebug("Initialized");
    }

    private static void OnExport(RandoController rc)
    {
        if (!GS.EnableItemReveal) return;
        var m = ItemChangerMod.Modules.GetOrAdd<IRModule>();
        m.RevealedItems = new HashSet<string>(GS.RevealedItems);
    }

    private static int AdjustHash(RandoController rc, int orig)
    {
        return GS.GetSettingsHash();
    }

    private static void LogIRSettings(LogArguments args, TextWriter tw)
    {
        tw.WriteLine("Logging ItemReveal settings");
        using Newtonsoft.Json.JsonTextWriter jtw = new(tw) { CloseOutput = false };
        RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, GS);
        tw.WriteLine();
    }
}