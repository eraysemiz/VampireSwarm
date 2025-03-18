using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public string playerInput;

    void Start()
    {  
        usernameInput.onEndEdit.AddListener(OnInputEndEdit);
    }
    
    public void OnInputEndEdit(string input)
    {
        playerInput = input;
        PlayerStats.PlayerData.Username = playerInput;
    }

}
