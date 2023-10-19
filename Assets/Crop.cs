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

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        Set();
    }

    private void Start()
    {
        GameManager._Instance.OnSleep += () => HasGrownToday = false;
    }

    private void Set()
    {
        meshFilter.mesh = meshOfStages[currentStage].Mesh;
        meshRenderer.material = meshOfStages[currentStage].Mat;
    }

    [ContextMenu("Grow")]
    public void Grow()
    {
        currentStage++;
        if (currentStage > meshOfStages.Length) return;
        Set();
        HasGrownToday = true;
    }
}
