using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ShowGameEventTriggerOpporotunity : MonoBehaviour
{
    public static ShowGameEventTriggerOpporotunity _Instance { get; private set; }

    private void Awake()
    {
        _Instance = this;
    }

    [SerializeField] private Transform spawnOn;
    private Dictionary<GameEventTriggerOnKeyPress, TextMeshProUGUI> spawnedOpporotunities = new Dictionary<GameEventTriggerOnKeyPress, TextMeshProUGUI>();

    private Dictionary<GameEventTriggerOnKeyPress, KeyCode> spawnedTriggers = new Dictionary<GameEventTriggerOnKeyPress, KeyCode>();
    [SerializeField] private SerializableDictionary<KeyCode, bool> keyCodeInUseMap = new SerializableDictionary<KeyCode, bool>();
    private GameEventTriggerOnKeyPress triggeredThisKeyPress;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI textPrefab;

    public bool Enable { get; set; }

    private void Update()
    {
        if (!Enable)
        {
            Clear();
            return;
        }

        foreach (KeyValuePair<GameEventTriggerOnKeyPress, KeyCode> kvp in spawnedTriggers)
        {
            if (triggeredThisKeyPress != null) break;
            if (!Input.GetKeyDown(kvp.Value)) continue;
            triggeredThisKeyPress = kvp.Key;
        }

        if (triggeredThisKeyPress != null)
        {
            triggeredThisKeyPress.Trigger();
            if (triggeredThisKeyPress.ShouldDisableOpperotunityOnTrigger)
            {
                TryRemoveTrigger(triggeredThisKeyPress);
            }
            triggeredThisKeyPress = null;
        }
    }

    public void TryAddTrigger(GameEventTriggerOnKeyPress trigger)
    {
        if (!Enable) return;
        if (GameManager._Instance.BeingForcedInsideForBeingTooCold || GameManager._Instance.BeingForcedToSleep)
        {
            return;
        }

        if (!spawnedTriggers.ContainsKey(trigger))
        {
            // Get Key Code
            KeyCode key = GetUnusedKeyCode();
            // No available key codes
            if (key == KeyCode.None) return;
            keyCodeInUseMap[key] = true;

            spawnedTriggers.Add(trigger, key);
            Spawn(trigger, key);
        }
    }

    public void TryRemoveTrigger(GameEventTriggerOnKeyPress trigger)
    {
        if (!spawnedTriggers.ContainsKey(trigger)) return;

        ReleaseKeyCode(spawnedTriggers[trigger]);
        spawnedTriggers.Remove(trigger);

        // Destroy Object
        Destroy(trigger);

        RedoKeyCodes();
    }

    public void Spawn(GameEventTriggerOnKeyPress spawningFor, KeyCode mapping)
    {
        // Spawn
        TextMeshProUGUI spawned = Instantiate(textPrefab, spawnOn);
        spawned.text = "'" + mapping.ToString()[mapping.ToString().Length - 1] + "' to " + spawningFor.ActivationText;
        spawnedOpporotunities.Add(spawningFor, spawned);
    }

    public void Destroy(GameEventTriggerOnKeyPress destroyingFor)
    {
        // Destroy
        TextMeshProUGUI spawned = spawnedOpporotunities[destroyingFor];
        spawnedOpporotunities.Remove(destroyingFor);
        Destroy(spawned.gameObject);
    }

    public void Clear()
    {
        while (spawnedTriggers.Count > 0)
        {
            TryRemoveTrigger(spawnedTriggers.First().Key);
        }
    }

    private KeyCode GetUnusedKeyCode()
    {
        foreach (KeyCode key in keyCodeInUseMap.Keys())
        {
            if (!keyCodeInUseMap[key])
            {
                return key;
            }
        }
        return KeyCode.None;
    }

    private void RedoKeyCodes()
    {
        List<GameEventTriggerOnKeyPress> events = new List<GameEventTriggerOnKeyPress>();
        foreach (KeyValuePair<GameEventTriggerOnKeyPress, KeyCode> kvp in spawnedTriggers)
        {
            events.Add(kvp.Key);
            ReleaseKeyCode(kvp.Value);
        }

        foreach (GameEventTriggerOnKeyPress gameEvent in events)
        {
            KeyCode key = GetUnusedKeyCode();
            Destroy(gameEvent);
            keyCodeInUseMap[key] = true;
            spawnedTriggers[gameEvent] = key;
            Spawn(gameEvent, key);
        }
    }

    private void ReleaseKeyCode(KeyCode key)
    {
        keyCodeInUseMap[key] = false;
    }
}
