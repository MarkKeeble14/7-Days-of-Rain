using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct JournalEntry
{
    public string DayText { get; private set; }
    public string Text { get; private set; }
    public JournalEntry(string dayText, string text)
    {
        DayText = dayText;
        Text = text;
    }
}

public class Journal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI entryText;
    [SerializeField] private string dayLeadIn = "Day ";
    [SerializeField] private string defaultEntryText = "<Waiting for Typing...>";
    [SerializeField] private int maxNumChars;
    [SerializeField] private int minCharsToCheckOff = 10;
    private bool journalWasActiveLastFrame;
    private List<JournalEntry> journalEntries = new List<JournalEntry>();
    public bool IsJournalActive { get; set; }

    public void ResetJournalForDay(int day)
    {
        dayText.text = dayLeadIn + day.ToString();
        entryText.text = defaultEntryText;
    }

    private void Update()
    {
        if (IsJournalActive)
        {
            if (Input.GetKeyDown(KeyCode.Backspace) && entryText.text.Length > 0)
            {
                entryText.text = entryText.text.Substring(0, entryText.text.Length - 1);
            }

            if (entryText.text.Length >= maxNumChars) return;

            // Add characters to the text
            string s = Input.inputString;
            string toAdd = "";
            foreach (char c in s)
            {
                if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c)) toAdd += c;
            }
            if (toAdd.Length > 0 && entryText.text.Equals(defaultEntryText)) entryText.text = "";
            entryText.text += toAdd;
            if (entryText.text.Length > minCharsToCheckOff)
            {
                GameManager._Instance.CheckToDoItem("Journal");
            }
        }
        else if (journalWasActiveLastFrame)
        {
            entryText.text = entryText.text.Substring(0, entryText.text.Length - 1);
        }
        journalWasActiveLastFrame = IsJournalActive;
    }

    public void SaveJournalEntry(int day)
    {
        if (entryText.text.Equals(defaultEntryText))
        {
            SaveJournalEntry(new JournalEntry(dayLeadIn + day.ToString(), "No Entry"));
        }
        else
        {
            SaveJournalEntry(new JournalEntry(dayLeadIn + day.ToString(), entryText.text));
        }
    }

    private void SaveJournalEntry(JournalEntry entry)
    {
        journalEntries.Add(entry);
    }

    public List<JournalEntry> GetJournalEntries()
    {
        return journalEntries;
    }
}
