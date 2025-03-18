using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "Scriptable Objects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField] // ??
    GameObject weapon;
    public GameObject Weapon { get => weapon;  set => weapon = value; }

    [SerializeField]
    float damage;
    public float Damage { get => damage;  set => damage = value; }
    [SerializeField]
    float speed;
    public float Speed { get => speed;  set => speed = value; }
    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; set => cooldownDuration = value; }
    [SerializeField]
    int durability;
    public int Durability { get => durability; private set => durability = value; }

    [SerializeField]
    int level;
    public int Level { get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }
}
