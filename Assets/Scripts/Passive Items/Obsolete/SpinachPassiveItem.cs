using UnityEngine;
[System.Obsolete]
public class SpinachPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMight += passiveItemData.Multiplier / 100f;
    }
}
