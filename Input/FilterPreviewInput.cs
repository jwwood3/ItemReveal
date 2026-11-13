using ItemChanger;
using ItemChanger.Extensions;
using MapChanger.MonoBehaviours;

namespace ItemReveal.Input;

internal class FilterPreviewInput()
    : IRWorldMapInput("Filter to Previewed Pins", () => InputHandler.Instance.inputActions.jump, 300f)
{
    private bool _filterOn;
    private MapObject _root;

    public override void DoAction()
    {
        var rootGO = GameManager.instance.gameMap?.FindChild("RandoMapCore Pins");
        if (rootGO == null || rootGO.GetComponent<MapObject>() is not MapObject root) return;

        var revealed = ItemChangerMod.Modules.Get<IRModule>()?.RevealedPlacements;
        if (revealed is null) return;

        if (_root != root)
        {
            foreach (var pin in root.Children)
            {
                var name = pin.name;
                pin.ActiveModifiers.Add(() => !_filterOn || revealed.Contains(name));
            }
            _root = root;
        }

        _filterOn = !_filterOn;
        root.MainUpdate();
    }
}
