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

            CharacterData data = characterList[i];

            iconImage.sprite = data.Icon;
            nameText.text = data.Name;
            descText.text = data.Description;
            bgImage.color = Color.white;

            button.onClick.AddListener(() =>
            {
                for (int j = 0; j < count; j++)
                {
                    var otherBg = gridContainer.GetChild(j)
                        .Find("Character Option")
                        .GetComponent<Image>();
                    otherBg.color = Color.white;
                }

                bgImage.color = new Color(1f, 1f, 0.7f);
            });
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
