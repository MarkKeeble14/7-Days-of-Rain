using TMPro;
using UnityEngine;

public class JournalEntryDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    public void Set(JournalEntry entry)
    {
        text.text = entry.DayText + ":\n" + entry.Text;
    }
}