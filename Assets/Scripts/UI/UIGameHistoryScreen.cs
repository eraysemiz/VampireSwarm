using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIGameHistoryScreen : MonoBehaviour
{
    [Header("UI Ayarlar�")]
    public GameObject historySlotPrefab;   // Slot prefab��
    public Transform contentHolder;        // Slot�lar�n eklenece�i ScrollView i�i
    public Button deleteButton;            // Sil butonu


    // public bool developerMode = false;     // Sil butonunu g�r�n�r k�lmak i�in
    public Toggle developerModeToggle;

    int selectedRunId = -1;                // �u an se�ili kay�t ID�si

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

        // Mevcut slotlar� kald�r
        foreach (Transform child in contentHolder)
            Destroy(child.gameObject);

        // Yeni slotlar� ekle
        foreach (var e in entries)
        {
            GameObject slotObj = Instantiate(historySlotPrefab, contentHolder);

            // Slot �zerindeki TMP_Text�i bul ve metni ayarla
            TMP_Text text = slotObj.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = $"{e.playedAt}: {e.character} Level {e.level} - {e.minutes:F1}m - {(e.victory ? "Victory" : "Defeat")}";

            // Slot�a t�klama olay� ekle
            Button btn = slotObj.GetComponentInChildren<Button>();
            if (btn != null)
            {
                int id = e.runId;  // closure sorununu �nlemek i�in yerel de�i�ken
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
    /// Se�ili kayd� veritaban�ndan siler ve paneli yeniler
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
