using System.Text;
using System.Reflection;
using UnityEngine;
using TMPro;

public class UIStatDisplay : MonoBehaviour
{

    public PlayerStats player; // The player that this stat display is rendering stats for.
    TextMeshProUGUI statNames, statValues;
    public bool displayCurrentHealth = false;
    public bool updateInEditor = false;
    public bool turkifyNames = true;

    // Update this stat display whenever it is set to be active.
    void OnEnable()
    {
        UpdateStatFields();
    }
    void OnDrawGizmosSelected()
    {
        if (updateInEditor) UpdateStatFields();
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

        // Add the current health to the stat box.
        if (displayCurrentHealth)
        {
            if (turkifyNames)
                names.AppendLine("Mevcut Can");
            else
                names.AppendLine("Current Health");
            values.AppendLine((player.CurrentHealth).ToString("F2"));
        }

        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            // A) Ýstatistik adýný belirle
            string displayName;
            if (turkifyNames)
            {
                switch (field.Name)
                {
                    case "maxHealth":
                        displayName = "Maks. Can";
                        break;
                    case "recovery":
                        displayName = "Yenilenme";
                        break;
                    case "armor":
                        displayName = "Zýrh";
                        break;
                    case "moveSpeed":
                        displayName = "Hareket Hýzý";
                        break;
                    case "might":
                        displayName = "Kudret";
                        break;
                    case "area":
                        displayName = "Hasar Alaný";
                        break;
                    case "speed":
                        displayName = "Silah Hýzý";
                        break;
                    case "duration":
                        displayName = "Silah Süresi";
                        break;
                    case "amount":
                        displayName = "Silah Miktarý";
                        break;
                    case "cooldown":
                        displayName = "Bekleme Süresi";
                        break;
                    case "magnet":
                        displayName = "Mýknatýs";
                        break;
                    case "revival":
                        displayName = "Dirilme";
                        break;
                    case "luck":
                        displayName = "Þans";
                        break;
                    default:
                        // Eðer yeni bir alan eklediyseniz ve henüz Türkçesi yoksa,
                        // orijinal adý PrettifyNames ile gösterelim:
                        displayName = PrettifyNames(new StringBuilder(field.Name));
                        break;
                }
            }
            else
            {
                // Orijinal Ýngilizce alan adýný okunabilir hale getir:
                displayName = PrettifyNames(new StringBuilder(field.Name));
            }

            names.AppendLine(displayName);

            // B) Ýstatistik deðerini al
            object val = field.GetValue(player.Stats);
            float fval = val is int ? (int)val : (float)val;

            // Eðer [Range] veya [Min] attribute’u varsa yüzdelik göster
            PropertyAttribute attribute = (PropertyAttribute)field
                                          .GetCustomAttribute<RangeAttribute>()
                                          ?? field.GetCustomAttribute<MinAttribute>();

            if (attribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100 - 100);

                if (Mathf.Approximately(percentage, 0))
                {
                    values.Append('-').Append('\n');
                }
                else
                {
                    if (percentage > 0)
                        values.Append('+');
                    values.Append(percentage).Append('%').Append('\n');
                }
            }
            else
            {
                values.Append(fval).Append('\n');
            }
        }

        statNames.text = names.ToString();
        statValues.text = values.ToString();
    } 

    public static string PrettifyNames(StringBuilder input)
    {
        // Return an empty string if StringBuilder is empty.
        if (input.Length <= 0) return string.Empty;

        StringBuilder result = new StringBuilder();
        char last = '\0';
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // Check when to uppercase or add spaces to a character.
            if (last == '\0' || char.IsWhiteSpace(last))
            {
                c = char.ToUpper(c);
            }
            else if (char.IsUpper(c))
            {
                result.Append(' '); // Insert space before capital letter
            }
            result.Append(c);

            last = c;
        }

        return result.ToString();
    }

    void Reset()
    {
        player = Object.FindFirstObjectByType<PlayerStats>();
    }
}