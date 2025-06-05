using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UICharacterSelectionMenu : MonoBehaviour
{
    [Header("UI Ayarları")]
    public Transform gridContainer;
    public List<CharacterData> characterList;

    [Header("Seçili Karakter")]
    private CharacterData selectedCharacter;

    void Start()
    {
        AssignDataToExistingCards();
    }

    void AssignDataToExistingCards()
    {
        int cardCount = gridContainer.childCount;
        int dataCount = characterList.Count;
        int count = Mathf.Min(cardCount, dataCount);

        for (int i = 0; i < count; i++)
        {
            Transform card = gridContainer.GetChild(i);

            var iconImage = card.transform
                .Find("Character Option/Icon/Character Icon")
                .GetComponent<Image>();

            var nameText = card.transform
                .Find("Character Option/Name")
                .GetComponent<TextMeshProUGUI>();

            var descText = card.transform
                .Find("Character Option/Description")
                .GetComponent<TextMeshProUGUI>();

            var button = card.transform
                .Find("Character Option/Button")
                .GetComponent<Button>();

            var bgImage = card.transform
                .Find("Character Option")
                .GetComponent<Image>();

            var healthText = card.transform
                .Find("Character Option/Stats/Health/Health Value")
                .GetComponent<TextMeshProUGUI>();

            var armorText = card.transform
                .Find("Character Option/Stats/Armor/Armor Value")
                .GetComponent<TextMeshProUGUI>();

            var movespeedText = card.transform
                .Find("Character Option/Stats/Movespeed/Movespeed Value")
                .GetComponent<TextMeshProUGUI>();

            var mightText = card.transform
                .Find("Character Option/Stats/Might/Might Value")
                .GetComponent<TextMeshProUGUI>();

            CharacterData data = characterList[i];

            iconImage.sprite = data.Icon;
            nameText.text = data.Name;
            descText.text = data.Description;
            bgImage.color = Color.white;

            healthText.text = data.stats.maxHealth.ToString("0");
            armorText.text = data.stats.armor.ToString("0.0");
            movespeedText.text = data.stats.moveSpeed.ToString("0.0");
            mightText.text = data.stats.might.ToString("0.0");

        }
    }

    public void ConfirmSelection()
    {
        if (selectedCharacter != null)
        {
            Debug.Log("Seçilen karakter: " + selectedCharacter.Name);
            PlayerPrefs.SetString("LastCharacter", selectedCharacter.Name);
        }
        else
        {
            Debug.LogWarning("Hiç karakter seçilmedi!");
        }
    }
}
