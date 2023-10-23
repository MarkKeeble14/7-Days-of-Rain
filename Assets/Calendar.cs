using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : MonoBehaviour
{
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private Vector3 firstSpawnPos;
    [SerializeField] private float vertHeightBetweenRows;
    [SerializeField] private float horizontalSpaceBetweenCells;
    [SerializeField] private CalendarDayCell dayCellPrefab;
    [SerializeField] private int[] numDaysPerWeekOverride;

    private CalendarDayCell[] calendarCells;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    private void Generate()
    {
        calendarCells = new CalendarDayCell[(int)(gridSize.x * gridSize.y)];
        Vector3 spawnPos = firstSpawnPos;
        int numDays = 1;
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int p = 0; p < gridSize.y; p++)
            {
                if (p >= numDaysPerWeekOverride[i])
                {
                    continue;
                }
                CalendarDayCell spawned = Instantiate(dayCellPrefab, transform);
                calendarCells[numDays - 1] = spawned;
                spawned.SetDayText(numDays++);
                spawned.transform.localPosition = spawnPos;

                spawnPos.x += horizontalSpaceBetweenCells;
            }
            spawnPos.y -= vertHeightBetweenRows;
            spawnPos.x = firstSpawnPos.x;
        }

        CrossoutUpTo(DayNightManager._Instance.StartDay - 1);
        // MarkHeartOnDay(DayNightManager._Instance.EndDay);
    }

    public void CrossoutDay(int i)
    {
        calendarCells[i].CrossoutDay();
    }

    public void CrossoutUpTo(int i)
    {
        for (int p = 0; p <= i; p++)
        {
            CrossoutDay(p);
        }
    }

    public void MarkHeartOnDay(int i)
    {
        calendarCells[i].MarkHeartOnDay();
    }
}
