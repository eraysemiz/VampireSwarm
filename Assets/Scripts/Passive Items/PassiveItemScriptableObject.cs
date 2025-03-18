using UnityEngine;


[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "Scriptable Objects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value; }

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
