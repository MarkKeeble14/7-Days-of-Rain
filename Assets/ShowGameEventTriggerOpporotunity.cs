using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShowGameEventTriggerOpporotunity : MonoBehaviour
{
    public static ShowGameEventTriggerOpporotunity _Instance { get; private set; }

    private void Awake()
    {
        _Instance = this;
    }

    [SerializeField] private Transform spawnOn;
    private Dictionary<GameEventTriggerOnKeyPress, TextMeshProUGUI> spawnedOpporotunities = new Dictionary<GameEventTriggerOnKeyPress, TextMeshProUGUI>();
    private GameEventTriggerOnKeyPress currentlyDisplayedEventTrigger;

    private List<GameEventTriggerOnKeyPress> availableTriggers = new List<GameEventTriggerOnKeyPress>();

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI textPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(GameManager._Instance.PlayerInteractKey) && currentlyDisplayedEventTrigger != null)
        {
            currentlyDisplayedEventTrigger.Trigger();
            RemoveTrigger(currentlyDisplayedEventTrigger);
            Destroy(currentlyDisplayedEventTrigger);
        }
    }

    public void TryAddTrigger(GameEventTriggerOnKeyPress trigger)
    {
        if (!availableTriggers.Contains(trigger))
        {
            availableTriggers.Add(trigger);
        }
        UpdateShownTrigger();
    }

    public void RemoveTrigger(GameEventTriggerOnKeyPress trigger)
    {
        availableTriggers.Remove(trigger);
        UpdateShownTrigger();
    }

    private void UpdateShownTrigger()
    {
        GameEventTriggerOnKeyPress toShow = GetClosestAvailableTrigger();
        if (availableTriggers.Count == 0 || toShow == null)
        {
            Destroy(currentlyDisplayedEventTrigger);
            return;
        }
        Spawn(toShow);
    }

    private GameEventTriggerOnKeyPress GetClosestAvailableTrigger()
    {
        if (availableTriggers.Count == 0)
        {
            return null;
        }
        else if (availableTriggers.Count == 1)
        {
            if (availableTriggers[0].PassesAdditionalConditions)
            {
                return availableTriggers[0];
            }
            else
            {
                return null;
            }
        }
        else
        {
            List<GameEventTriggerOnKeyPress> passingTriggers = new List<GameEventTriggerOnKeyPress>();
            foreach (GameEventTriggerOnKeyPress trigger in availableTriggers)
            {
                if (trigger.PassesAdditionalConditions)
                {
                    passingTriggers.Add(trigger);
                }
            }

            if (passingTriggers.Count == 0)
            {
                return null;
            }
            else if (passingTriggers.Count == 1)
            {
                return passingTriggers[0];
            }
            else
            {
                GameEventTriggerOnKeyPress closestTrigger = passingTriggers[0];
                float closestDistance = Vector3.Distance(closestTrigger.transform.position, player.position);
                for (int i = 1; i < passingTriggers.Count; i++)
                {
                    GameEventTriggerOnKeyPress currentTrigger = passingTriggers[i];
                    float currentDistance = Vector3.Distance(currentTrigger.transform.position, player.position);
                    if (currentDistance < closestDistance)
                    {
                        closestTrigger = currentTrigger;
                        closestDistance = currentDistance;
                    }
                }
                return closestTrigger;
            }

        }
    }

    public void Spawn(GameEventTriggerOnKeyPress spawningFor)
    {
        if (currentlyDisplayedEventTrigger == spawningFor) return;
        if (currentlyDisplayedEventTrigger != null) Destroy(currentlyDisplayedEventTrigger);
        currentlyDisplayedEventTrigger = spawningFor;

        TextMeshProUGUI spawned = Instantiate(textPrefab, spawnOn);
        spawned.text = spawningFor.ActivationText;
        spawnedOpporotunities.Add(spawningFor, spawned);
    }

    public void Destroy(GameEventTriggerOnKeyPress destroyingFor)
    {
        if (destroyingFor == null) return;

        // Check to make sure event exists in map
        if (!spawnedOpporotunities.ContainsKey(destroyingFor)) return;

        // Destroy
        TextMeshProUGUI spawned = spawnedOpporotunities[destroyingFor];
        spawnedOpporotunities.Remove(destroyingFor);
        Destroy(spawned.gameObject);

        currentlyDisplayedEventTrigger = null;
    }

    public void ClearCurrent()
    {
        if (currentlyDisplayedEventTrigger != null)
        {
            Destroy(currentlyDisplayedEventTrigger);
        }
    }
}
