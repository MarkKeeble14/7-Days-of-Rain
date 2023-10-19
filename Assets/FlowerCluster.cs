using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerCluster : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxAxisChange;
    [SerializeField] private Vector2 chanceToRecluster;
    [SerializeField] private float reduceChanceToClusterOnCluster;

    public void Cluster()
    {
        if (chanceToRecluster.x <= 0) return;
        try
        {
            Vector3 spawnPos = transform.position + new Vector3(RandomHelper.RandomFloat(minMaxAxisChange), 0, RandomHelper.RandomFloat(minMaxAxisChange));
            FlowerCluster spawned = Instantiate(this, spawnPos, Quaternion.identity);
            spawned.transform.SetParent(transform, true);
            spawned.chanceToRecluster.x -= reduceChanceToClusterOnCluster;
            spawned.Cluster();
        }
        catch
        {
            // Doesn't really matter if this throws an error or not so who cares
        }
    }
}
