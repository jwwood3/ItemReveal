using ItemChanger;
using ItemChanger.Modules;
using ItemChanger.Tags;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ItemReveal;

public class IRModule : Module
{
    private const string Message = "RandoSupplementalMetadata";
    private const string Prop = "ForceEnablePreview";

    public HashSet<string> RevealedItems { get; set; } = new();

    [JsonIgnore]
    public HashSet<string> RevealedPlacements { get; } = new();

    public override void Initialize()
    {
        Apply();
    }
    public override void Unload()
    {
        
    }

    public void Apply()
    {
        RevealedPlacements.Clear();
        foreach (var placement in ItemChanger.Internal.Ref.Settings.Placements.Values)
        {
            foreach (var item in placement.Items)
            {
                bool reveal = RevealedItems.Contains(item.name);
                var tag = item.GetTags<InteropTag>().FirstOrDefault(t => t.Message == Message);
                if (tag is null)
                {
                    tag = new InteropTag { Message = Message, Properties = new() };
                    item.AddTag(tag);
                }
                tag.Properties[Prop] = reveal;
                if (reveal)
                {
                    RevealedPlacements.Add(placement.Name);
                }
            }
        }
    }
}
