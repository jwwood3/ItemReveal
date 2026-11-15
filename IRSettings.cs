using MenuChanger.Attributes;
using Newtonsoft.Json;
using RandomizerCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemReveal;

public class IRSettings
{
    [JsonProperty]
    public bool EnableItemReveal { get; set; } = false;

    [JsonProperty]
    public bool RevealAllDuplicates { get; set; } = true;

    [JsonProperty]
    [MenuIgnore]
    public HashSet<string> RevealedItems { get; set; } = new();

    [JsonIgnore]
    internal bool Any =>
        EnableItemReveal;

    [JsonIgnore]
    internal HashSet<string> ExcludedGroups = new()
    {
        "GeoChests",
        "BossGeo",
        "GeoRocks",
        "BossEssence",
        "WhisperingRoots",
        "SoulTotems",
        "LoreTablets",
        "JournalEntries",
    };

    internal IRSettings Clone()
    {
        IRSettings clone =
            new()
            {
                EnableItemReveal = this.EnableItemReveal,
                RevealedItems = new HashSet<string>(this.RevealedItems),
            };
        return clone;
    }

    internal int GetSettingsHash()
    {
        if (!Any)
        {
            return 0;
        }

        return typeof(IRSettings)
            .GetProperties()
            .Where(p => p.Name != nameof(Any))
            .OrderBy(p => p.Name)
            .Select(p => $"{p.Name}: {FormatValue(p.GetValue(this))}".GetStableHashCode())
            .Aggregate(64327, (current, v) => (88169 * current) + v);
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            IEnumerable<string> items => string.Join(",", items.OrderBy(s => s, StringComparer.Ordinal)),
            _ => value?.ToString() ?? "null",
        };
    }
}