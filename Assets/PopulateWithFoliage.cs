using System.Collections;
using UnityEngine;

[System.Serializable]
public struct FoliageSpawningData
{
    [Header("Spawning Data")]
    public PercentageMap<BindToLayer> prefabs;

    [Header("Spawning Positioning Data")]
    public float width;
    public float height;
    public float radius;
    public float minProximityToOrigin;

    [Header("Variety")]
    public Vector2 minMaxScaleMod;
}

public class PopulateWithFoliage : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private FoliageSpawningData[] foliageSpawningDatas;
    [SerializeField] private float spawnObjectsAtY;

    [Header("References")]
    [SerializeField] private Transform spawnOn;

    [SerializeField] private Vector2 chanceForFlowerToCluster;

    public void Populate()
    {
        foreach (FoliageSpawningData data in foliageSpawningDatas)
        {
            Populate(data);
        }
        StartCoroutine(ActivateFlowerClusters());
    }

    private IEnumerator ActivateFlowerClusters()
    {
        FlowerCluster[] flowerClusters = FindObjectsOfType<FlowerCluster>();
        foreach (FlowerCluster cluster in flowerClusters)
        {
            if (RandomHelper.EvaluateChanceTo(chanceForFlowerToCluster))
            {
                cluster.Cluster();
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void Populate(FoliageSpawningData data)
    {
        PoissonDiscSampler sampler = new PoissonDiscSampler(data.width, data.height, data.radius);
        foreach (Vector2 sample in sampler.Samples())
        {
            Vector3 spawnPos = new Vector3(sample.x, spawnObjectsAtY, sample.y);
            spawnPos.x -= data.width / 2;
            spawnPos.z -= data.height / 2;
            if (Vector3.Distance(spawnPos, Vector3.zero) < data.minProximityToOrigin) continue;

            BindToLayer spawned = Instantiate(data.prefabs.GetOption(), spawnOn);
            spawned.transform.position = spawnPos;
            spawned.transform.localScale += Vector3.one * RandomHelper.RandomFloat(data.minMaxScaleMod);
            spawned.TryBind();
        }
    }
}
