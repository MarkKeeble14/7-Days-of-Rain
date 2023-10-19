using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeOfDayProgressDisplay : MonoBehaviour
{
    [SerializeField] private Image display;
    private void Update()
    {
        display.fillAmount = DayNightManager._Instance.PercentThroughDay;
    }
}
