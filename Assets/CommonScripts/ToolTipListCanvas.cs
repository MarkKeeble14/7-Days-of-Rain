using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ToolTipListCanvas : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup listPrefab;
    [SerializeField] private RectTransform rect;

    private List<VerticalLayoutGroup> spawnedVLayouts = new List<VerticalLayoutGroup>();

    [SerializeField] private HorizontalLayoutGroup hLayout;

    [SerializeField] private Canvas canvas;

    private Action onDestroyAction;

    public void Set(int sortOrder, Action a)
    {
        canvas.sortingOrder = sortOrder;
        onDestroyAction += a;
    }

    private void OnDestroy()
    {
        onDestroyAction?.Invoke();
    }


    public VerticalLayoutGroup SpawnList()
    {
        VerticalLayoutGroup spawned = Instantiate(listPrefab, transform);
        spawnedVLayouts.Add(spawned);
        return spawned;
    }

    public RectTransform GetRect()
    {
        return rect;
    }

    public VerticalLayoutGroup GetVerticalLayoutGroup(int index)
    {
        return spawnedVLayouts[index];
    }

    public HorizontalLayoutGroup GetHorizontalLayoutGroup()
    {
        return hLayout;
    }
}