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

    [SerializeField] private MeshData deadState;
    [SerializeField] private Transform rootTransform;
    [SerializeField] private Vector3 deadScale = Vector3.one;
    public bool IsDead { get; private set; }
    public static int GrowGoalToday { get; set; }
    public static int NumGrownToday { get; set; }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        GameManager._Instance.CropCount++;
        GameManager._Instance.OnEndOfDay += RunOnSleep;
        Set(meshOfStages[currentStage]);
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
        if (IsDead) return;

        HasGrownToday = true;
        currentStage++;
        if (currentStage <= meshOfStages.Length)
        {
            Set(meshOfStages[currentStage]);
        }

        NumGrownToday++;
        if (NumGrownToday >= GrowGoalToday)
        {
            GameManager._Instance.CheckToDoItem("TendCrops");
        }
    }

    [ContextMenu("Die")]
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Set(deadState);
        rootTransform.localScale = deadScale;
        GameManager._Instance.CropCount--;
    }
}
