using ItemChanger;
using ItemChanger.Modules;
using ItemChanger.Tags;
using System.Collections.Generic;
using System.Linq;

namespace ItemReveal;

public class IRModule : Module
{
    private const string Message = "RandoSupplementalMetadata";
    private const string Prop = "ForceEnablePreview";

    public HashSet<string> RevealedItems { get; set; } = new();

    public override void Initialize()
    {
        Apply();
    }
    public override void Unload()
    {
        
    }

    public void Apply()
    {
        foreach (var placement in ItemChanger.Internal.Ref.Settings.Placements.Values)
        {
            foreach (var item in placement.Items)
            {
                var tag = item.GetTags<InteropTag>().FirstOrDefault(t => t.Message == Message);
                if (tag is null)
                {
                    tag = new InteropTag { Message = Message, Properties = new() };
                    item.AddTag(tag);
                }
                tag.Properties[Prop] = RevealedItems.Contains(item.name);
            }
        }
    }
}
