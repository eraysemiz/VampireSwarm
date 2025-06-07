using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public struct RewardInfo
{
    public Sprite icon;
    public string displayName;
    public string description;
    public string levelText;
}

public class UIChestRewardDisplay : MonoBehaviour
{
    public GameObject rewardSlotPrefab; // Reward Slot objesi
    public Transform rewardHolder;      // Rewards Holder
    public float revealDelay = 0.5f;

    public AudioSource rewardSound;
    public Button continueButton;

    private void Start()
    {
        // “Devam Et” butonunu devre dýþý býrak
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
    }

    public void ShowRewards(List<RewardInfo> rewards)
    {
        foreach (Transform child in rewardHolder)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(RevealRewards(rewards));
    }

    IEnumerator RevealRewards(List<RewardInfo> rewards)
    {

        yield return null; // hafif gecikme

        foreach (var info in rewards)
        {
            // Slot prefab'ýný ekle
            GameObject slot = Instantiate(rewardSlotPrefab, rewardHolder);
            slot.SetActive(true);

            // Icon
            var iconImg = slot.transform.Find("Icon/Item Icon").GetComponent<Image>();
            iconImg.sprite = info.icon;

            // Ýsmi
            var nameTxt = slot.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            nameTxt.text = info.displayName;

            // Açýklama
            var descTxt = slot.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            descTxt.text = info.description;

            // Seviye metni
            var lvlTxt = slot.transform.Find("Level").GetComponent<TextMeshProUGUI>();
            lvlTxt.text = info.levelText;

            // Animasyon tetikle
            var anim = slot.GetComponent<Animator>();
            if (anim) anim.SetTrigger("Show");

            // Unscaled bekle
            yield return new WaitForSecondsRealtime(revealDelay);
        }

        // Tüm ödüller eklendiðinde butonu aktifleþtir.
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        Debug.Log("Devam Butonu Deaktive edildi");
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
    }
}
