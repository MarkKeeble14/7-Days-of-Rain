using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct ToDoListItemData
{
    public string Key;
    public string Text;
}

public class ToDoList : MonoBehaviour
{
    [SerializeField] private ToDoListItem itemPrefab;
    [SerializeField] private Transform spawnOn;
    private Dictionary<string, ToDoListItem> spawnedItems = new Dictionary<string, ToDoListItem>();

    public bool IsItemChecked(string key)
    {
        return spawnedItems[key].IsChecked;
    }

    public void AddListItems(ToDoListItemData[] data)
    {
        foreach (ToDoListItemData item in data)
        {
            AddListItem(item);
        }
    }

    public void AddListItem(ToDoListItemData data)
    {
        ToDoListItem spawned = Instantiate(itemPrefab, spawnOn);
        spawned.SetText(data.Text);
        spawnedItems.Add(data.Key, spawned);
    }

    public void Clear()
    {
        while (spawnedItems.Count > 0)
        {
            string key = spawnedItems.First().Key;
            RemoveListItem(key);
        }
    }

    private void RemoveListItem(string key)
    {
        ToDoListItem item = spawnedItems[key];
        spawnedItems.Remove(key);
        Destroy(item.gameObject);
    }

    public void CheckListItem(string key)
    {
        spawnedItems[key].Check();
    }
}
