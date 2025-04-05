using UnityEngine;

public class WingsPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.moveSpeedMultiplier += passiveItemData.Multiplier / 100f;
        player.UpdateMoveSpeed();
    }


}
