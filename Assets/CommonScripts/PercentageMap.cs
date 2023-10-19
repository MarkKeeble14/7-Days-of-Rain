using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PercentageMap<T>
{
    [SerializeField]
    private List<SerializableKeyValuePair<T, int>> percentageMap
        = new List<SerializableKeyValuePair<T, int>>();

    public int Count => percentageMap.Count;

    private SerializableKeyValuePair<T, int> GetFromDictionary()
    {
        // Say a list has 2 elements, 1 for SoldierEnemy with number 50, and 1 for DroneEnemy with number 50
        // That means it's a 50/50 which one of the enemies will be spawned
        int rand = RandomHelper.RandomIntExclusive(0, 100);
        float floor = 0;
        SerializableKeyValuePair<T, int> mostCommon = null;
        int largestWeight = 0;
        for (int i = 0; i < percentageMap.Count; i++)
        {
            SerializableKeyValuePair<T, int> current = percentageMap[i];
            // Debug.Log(current.Key + ", " + current.Value);

            // Keeping track of the highest weighted option to return in the case of a bogus error
            if (current.Value > largestWeight)
            {
                mostCommon = current;
                largestWeight = current.Value;
            }
            // Debug.Log("To Spawn: " + current.Key + ", rand must be greater than: "
            //    + floor + " and less than: " + (floor + current.Value) + ": rand = " + rand);
            if (rand > floor && rand <= floor + current.Value)
            {
                // Debug.Log(current.Key);
                return current;
            }
            floor += current.Value;
        }
        if (Count > 0)
        {
            // If there are any options, by default we return the most common
            // Debug.Log("Returning most common: " + mostCommon);
            return mostCommon;
        }
        else
        {
            // Otherwise, return default (can be null)
            // Debug.Log("Returning default: " + default(T));
            return null;
        }
    }

    public SerializableKeyValuePair<T, int> GetFullOption()
    {
        return GetFromDictionary();
    }

    public T GetOption()
    {
        return GetFullOption().Key;
    }

    public void AddOption(SerializableKeyValuePair<T, int> option)
    {
        // Debug.Log("Adding: Key: " + option.Key + ", Value: " + option.Value);
        percentageMap.Add(option);
    }

    public bool RemoveOption(SerializableKeyValuePair<T, int> option)
    {
        // Debug.Log("Removing: Key: " + option.Key + ", Value: " + option.Value);
        if (percentageMap.Contains(option))
        {
            percentageMap.Remove(option);
            return true;
        }
        return false;
    }

    public SerializableKeyValuePair<T, int> FindOption(T option)
    {
        foreach (SerializableKeyValuePair<T, int> kvp in percentageMap)
        {
            if (kvp.Key.Equals(option)) return kvp;
        }
        return null;
    }
}
