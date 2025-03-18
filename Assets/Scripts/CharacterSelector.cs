using Unity.Collections;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterScriptableObjects characterData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + "DELETED");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static CharacterScriptableObjects GetData()
    {
        return instance.characterData;
    }

    public void SelectCharacter(CharacterScriptableObjects character)
    {
        characterData = character;
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
