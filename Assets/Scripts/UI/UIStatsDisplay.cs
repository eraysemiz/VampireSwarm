using System.Text;
using System.Reflection;
using UnityEngine;
using TMPro;

public class UIStatDisplay : MonoBehaviour
{

    public PlayerStats player; // The player that this stat display is rendering stats for.
    TextMeshProUGUI statNames, statValues;

    // Update this stat display whenever it is set to be active.
    void OnEnable()
    {
        UpdateStatFields();
    }

    public void UpdateStatFields()
    {
        if (!player) return;

        // Get a reference to both Text objects to render stat names and stat values.
        if (!statNames) statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!statValues) statValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // Render all stat names and values.
        // Use StringBuilders so that the string manipulation runs faster.
        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();
        FieldInfo[] fields = typeof(PlayerStats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            names.AppendLine(field.Name);

            object val = field.GetValue(player);
            float fval = val is int ? (int)val : (float)val;
            values.Append(fval).Append('\n');
        }
    }
}