using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MeshData
{
    public Mesh Mesh;
    public Material Mat;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Crop : MonoBehaviour
{
    [SerializeField] private MeshData[] meshOfStages;
    private int currentStage;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public bool HasGrownToday { get; private set; }
    [SerializeField] private GameObject root;
    [SerializeField] private Vector2 chanceToDie;
    [SerializeField] private int numDaysLeftUntendedToDie;
    private int currentNumDaysUntended;
    public bool ReadyForHarvest => currentStage == meshOfStages.Length - 1;

    [SerializeField] private MeshData deadState;
    [SerializeField] private Transform rootTransform;
    [SerializeField] private Vector3 deadScale = Vector3.one;
    public bool IsDead { get; private set; }
    public static int GrowGoalToday { get; set; }
    public static int NumGrownToday { get; set; }
    public static int NumSpawned { get; set; }
    public static int NumHarvested { get; private set; }
    public static int NumDead { get; private set; }

    [SerializeField] private TemporaryAudioSourceSpawner tempAudioSource;
    [SerializeField] private RandomClipAudioClipContainer onHarvest;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        GameManager._Instance.OnEndOfDay += RunOnSleep;
        NumSpawned++;
        Set(meshOfStages[currentStage]);
    }

    private void OnDestroy()
    {
        GameManager._Instance.OnEndOfDay -= RunOnSleep;
    }

    private void RunOnSleep()
    {
        if (!HasGrownToday)
        {
            currentNumDaysUntended++;
            if (currentNumDaysUntended >= numDaysLeftUntendedToDie)
            {
                if (RandomHelper.EvaluateChanceTo(chanceToDie))
                {
                    Die();
                }
            }
        }
        else
        {
            currentNumDaysUntended = 0;
            HasGrownToday = false;
        }
    }

    private void Set(MeshData data)
    {
        meshFilter.mesh = data.Mesh;
        meshRenderer.material = data.Mat;
    }

    [ContextMenu("Grow")]
    public void Grow()
    {
        if (ReadyForHarvest) return;
        if (HasGrownToday) return;
        if (IsDead) return;

        // Tracking
        NumGrownToday++;
        HasGrownToday = true;
        if (NumGrownToday == GrowGoalToday)
        {
            GameManager._Instance.TendedToAllCrops();
            GameManager._Instance.CheckToDoItem("TendCrops");
        }

        Set(meshOfStages[++currentStage]);
    }

    [ContextMenu("Harvest")]
    public void Harvest()
    {
        if (!ReadyForHarvest) return;
        if (HasGrownToday) return;
        NumHarvested++;
        tempAudioSource.PlayOneShot(onHarvest);
        Destroy(gameObject);
    }


    [ContextMenu("Die")]
    public void Die()
    {
        if (IsDead) return;
        NumDead++;
        IsDead = true;
        rootTransform.localScale = deadScale;
        Set(deadState);
    }
}
