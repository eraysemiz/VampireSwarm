using UnityEngine;

[CreateAssetMenu (fileName = "WeaponEvolutionBlueprint", menuName = "Scriptable Objects/WeaponEvolutionBlueprint")]

public class WeaponEvolutionBlueprint : ScriptableObject
{
    public WeaponScriptableObject baseWeaponData;
    public PassiveItemScriptableObject catalystPassiveItemData;
    public WeaponScriptableObject evolvedWeaponData;
    public GameObject evolvedWeapon;

}
