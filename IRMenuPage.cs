using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;

using System.Collections.Generic;
using System.Linq;

using RandomizerMod.RandomizerData;

namespace ItemReveal;

public class IRMenuPage
{

    private readonly MenuPage _itemPage;
    private readonly VerticalItemPanel _vip;
    private readonly MultiGridItemPanel _mgip;
    private readonly SmallButton _jumpButton;
    public Dictionary<string, ToggleButton> itemButtons;
    private Dictionary<string, MenuPage> _groupPages;

    private IRMenuPage(MenuPage landingPage)
    {
        _itemPage = new MenuPage("ItemReveal Selector", landingPage);
        Mef = new(_itemPage, ItemReveal.GS);
        _vip = new(_itemPage, new(0, 350), 75f, true, Mef.Elements);
        itemButtons = new Dictionary<string, ToggleButton>();
        _groupPages = new Dictionary<string, MenuPage>();
        var itemGroups = ItemGroups();
        var groupButtons = new List<SmallButton>();
        foreach (string group in itemGroups.Keys)
        {
            var groupPage = new MenuPage(group, _itemPage);
            _groupPages[group] = groupPage;
            var buttons = itemGroups[group].Select(name =>
            {
                var b = new ToggleButton(groupPage, name.Replace('_', ' '));
                b.SetValue(ItemReveal.GS.RevealedItems.Contains(name));
                b.ValueChanged += on =>
                {
                    if (on) ItemReveal.GS.RevealedItems.Add(name);
                    else ItemReveal.GS.RevealedItems.Remove(name);
                };
                itemButtons.Add(name, b);
                return b;
            }).ToArray();
            new MultiGridItemPanel(groupPage, 5, 3, 60f, 650f, new UnityEngine.Vector2(0, 300), buttons);
            var groupPageButton = new SmallButton(_itemPage, group);
            groupPageButton.AddHideAndShowEvent(_itemPage, groupPage);
            groupButtons.Add(groupPageButton);
        }
        _jumpButton = new SmallButton(landingPage, "ItemReveal");
        _jumpButton.AddHideAndShowEvent(landingPage, _itemPage);
        _mgip = new MultiGridItemPanel(_itemPage, 5, 3, 60f, 650f, new UnityEngine.Vector2(0, 250), groupButtons.ToArray());
    }

    private Dictionary<string, List<string>> ItemGroups()
    {
        var groupItems = new Dictionary<string, List<string>>();
        var seen = new HashSet<string>();

        foreach (var entry in Data.PoolList)
        {
            if (ItemReveal.GS.ExcludedGroups.Contains(entry.Group)) continue;
            foreach (var item in entry.IncludeItems)
            {
                if (!seen.Add(item)) continue;
                if (!groupItems.TryGetValue(entry.Group, out var list))
                {
                    list = new List<string>();
                    groupItems[entry.Group] = list;
                }
                list.Add(item);
            }
        }

        return groupItems;
    }

    internal static IRMenuPage Instance { get; private set; }

    internal MenuElementFactory<IRSettings> Mef { get; private set; }

    internal static void Hook()
    {
        RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
        MenuChangerMod.OnExitMainMenu += OnExitMenu;
    }

    private static void OnExitMenu()
    {
        Instance = null;
    }

    private static void ConstructMenu(MenuPage landingPage)
    {
        Instance = new(landingPage);
    }

    private static bool HandleButton(MenuPage landingPage, out SmallButton button)
    {
        button = Instance?._jumpButton;
        return button is not null;
    }

}
