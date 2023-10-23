using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseMeshOnSpawn : MonoBehaviour
{
    [SerializeField] private MeshData[] possibleData;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;

    private void Awake()
    {
        SetData();
    }

    private void SetData()
    {
        MeshData data = RandomHelper.GetRandomFromArray(possibleData);
        meshRenderer.material = data.Mat;
        meshFilter.mesh = data.Mesh;
    }
}
