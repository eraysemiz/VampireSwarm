using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIGameHistoryScreen : MonoBehaviour
{
    public GameObject historySlotPrefab;
    public Transform contentHolder;
    public Button deleteButton;
    public bool developerMode = false;

    int selectedRunId = -1;

    void OnEnable()
    {
        Refresh();
        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(DeleteSelected);
        }
        UpdateDeleteButton();
    }

    public void SetDeveloperMode(bool value)
    {
        developerMode = value;
        UpdateDeleteButton();
    }

    void UpdateDeleteButton()
    {
        if (deleteButton)
            deleteButton.gameObject.SetActive(developerMode);
    }

    public void Refresh()
    {
        selectedRunId = -1;

        DatabaseManager db = FindObjectOfType<DatabaseManager>();
        if (db == null) return;

        List<DatabaseManager.GameHistoryEntry> entries = db.LoadGameHistory();
        for (int i = contentHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(contentHolder.GetChild(i).gameObject);
        }

        foreach (var e in entries)
        {
            GameObject slotObj = Instantiate(historySlotPrefab, contentHolder);
            TMP_Text text = slotObj.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = $"{e.playedAt}: {e.character} LV{e.level} - {e.minutes:F1}m - {(e.victory ? "Victory" : "Defeat")}";

            Button btn = slotObj.GetComponent<Button>();
            if (btn != null)
            {
                int id = e.runId;
                btn.onClick.AddListener(() => OnSlotSelected(id));
            }
        }
    }

    void OnSlotSelected(int runId)
    {
        selectedRunId = runId;
    }

    public void DeleteSelected()
    {
        if (selectedRunId < 0)
            return;

        DatabaseManager db = FindObjectOfType<DatabaseManager>();
        if (db != null)
        {
            db.DeleteGameHistoryEntry(selectedRunId);
        }
        selectedRunId = -1;
        Refresh();
    }
}