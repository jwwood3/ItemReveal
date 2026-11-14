using ItemChanger;
using ItemChanger.Extensions;
using MapChanger.MonoBehaviours;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace ItemReveal.Input;

internal class RevealButtonInput()
    : IRWorldMapInput("Pan to next Previewed Pin or Filter Pins to only Previewed Pins", () => InputHandler.Instance.inputActions.jump, 0f)
{
    private int _index = -1;
    private bool _filterOn;
    private MapObject _root;
    private const float HoldSeconds = 0.3f;
    private bool _tracking;
    public override void DoAction()
    {
        if (_tracking) return;
        GameManager.instance.StartCoroutine(TrackPress());
    }

    private IEnumerator TrackPress()
    {
        _tracking = true;
        var button = InputHandler.Instance.inputActions.jump;
        float start = Time.unscaledTime;
        bool wasHold = false;
        while (button.IsPressed)
        {
            if (!wasHold && Time.unscaledTime - start >= HoldSeconds)
            {
                wasHold = true;
                FilterPins();
            }
            yield return null;
        }

        if (!wasHold && ActiveCondition())
        {

            CyclePan();
        }
        _tracking = false;
    }

    private void CyclePan()
    {
        var rootGO = GameManager.instance.gameMap?.FindChild("RandoMapCore Pins");
        if (rootGO == null || rootGO.GetComponent<MapObject>() is not MapObject root) return;

        var revealed = ItemChangerMod.Modules.Get<IRModule>()?.RevealedPlacements;
        if (revealed is null) return;

        var pins = new List<MapObject>();

        foreach (var pin in root.Children)
        {
            if (pin.isActiveAndEnabled && revealed.Contains(pin.name))
            {
                pins.Add(pin);
            }
        }
        _index = _index + 1;
        if (_index >= pins.Count || _index < 0)
        {
            _index = 0;
        }
        MapPanner.PanTo(pins[_index].transform.position);
    }

    private void FilterPins()
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
