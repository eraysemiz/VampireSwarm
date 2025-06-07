using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIGameHistoryScreen : MonoBehaviour
{
    [Header("UI Ayarlarý")]
    public GameObject historySlotPrefab;   // Slot prefab’ý
    public Transform contentHolder;        // Slot’larýn ekleneceði ScrollView içi
    public Button deleteButton;            // Sil butonu


    // public bool developerMode = false;     // Sil butonunu görünür kýlmak için
    public Toggle developerModeToggle;

    int selectedRunId = -1;                // Þu an seçili kayýt ID’si

    void OnEnable()
    {
        Refresh();
        if (developerModeToggle != null)
            UpdateDeleteButtonVisibility(developerModeToggle.isOn);
    }

    private void Awake()
    {
        if (developerModeToggle != null)
            developerModeToggle.onValueChanged.AddListener(OnDeveloperModeChanged);

    }

    private void OnDeveloperModeChanged(bool isOn)
    {
        UpdateDeleteButtonVisibility(isOn);
    }

    private void UpdateDeleteButtonVisibility(bool visible)
    {
        if (deleteButton != null)
            deleteButton.gameObject.SetActive(visible);
    }


    public void Refresh()
    {
        selectedRunId = -1;

        DatabaseManager db = Object.FindFirstObjectByType<DatabaseManager>();
        if (db == null) return;

        List<DatabaseManager.GameHistoryEntry> entries = db.LoadGameHistory();

        // Mevcut slotlarý kaldýr
        foreach (Transform child in contentHolder)
            Destroy(child.gameObject);

        // Yeni slotlarý ekle
        foreach (var e in entries)
        {
            GameObject slotObj = Instantiate(historySlotPrefab, contentHolder);

            // Slot üzerindeki TMP_Text’i bul ve metni ayarla
            TMP_Text text = slotObj.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = $"{e.playedAt}: {e.character} Level {e.level} - {e.minutes:F1}m - {(e.victory ? "Victory" : "Defeat")}";

            // Slot’a týklama olayý ekle
            Button btn = slotObj.GetComponentInChildren<Button>();
            if (btn != null)
            {
                int id = e.runId;  // closure sorununu önlemek için yerel deðiþken
                btn.onClick.AddListener(() => OnSlotSelected(id));
            }

            // Force layout rebuild so the list updates immediately
            var rect = contentHolder as RectTransform;
            if (rect)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }

    public void OnSlotSelected(int runId)
    {
        selectedRunId = runId;
    }

    /// <summary>
    /// Seçili kaydý veritabanýndan siler ve paneli yeniler
    /// </summary>
    public void DeleteSelected()
    {
        if (selectedRunId < 0)
            return;

        DatabaseManager db = Object.FindFirstObjectByType<DatabaseManager>();
        if (db != null)
            db.DeleteGameHistoryEntry(selectedRunId);

        Refresh();
    }

    void OnDestroy()
    {
        // Dinleyiciyi temizleyelim
        if (developerModeToggle != null)
            developerModeToggle.onValueChanged.RemoveListener(OnDeveloperModeChanged);
    }

}
