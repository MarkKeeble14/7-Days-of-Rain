using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToDoListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private GameObject checkMark;
    [SerializeField] private string leadIn = "• ";
    public bool IsChecked { get; private set; }

    public void Check()
    {
        IsChecked = true;
        checkMark.SetActive(true);
    }

    public void SetText(string str)
    {
        itemText.text = leadIn + str;
    }
}
