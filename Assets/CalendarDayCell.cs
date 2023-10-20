using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalendarDayCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayNumber;
    [SerializeField] private GameObject cross;
    [SerializeField] private GameObject heart;

    public void CrossoutDay()
    {
        cross.SetActive(true);
    }
    public void MarkHeartOnDay()
    {
        heart.SetActive(true);
    }

    public void SetDayText(int i)
    {
        dayNumber.text = i.ToString();
    }
}
