using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropSpawner : MonoBehaviour
{
    [SerializeField] private GameObject crop;
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private float distBetweenAnchors = 1;
    [SerializeField] private Vector2 minMaxRandomOffsetPerCrop = new Vector2(-.25f, .25f);

    public void Generate()
    {
        float startX = (-gridSize.x / 2) * distBetweenAnchors;
        if (gridSize.x % 2 == 1) startX += distBetweenAnchors / 2;
        float startZ = (-gridSize.y / 2) * distBetweenAnchors;
        if (gridSize.y % 2 == 1) startZ += distBetweenAnchors / 2;

        Vector3 spawnPos = new Vector3(startX, 0, startZ);
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int p = 0; p < gridSize.y; p++)
            {
                GameObject crop = Instantiate(this.crop, transform);
                crop.transform.localPosition = spawnPos + new Vector3(
                    RandomHelper.RandomFloat(minMaxRandomOffsetPerCrop),
                    0,
                    RandomHelper.RandomFloat(minMaxRandomOffsetPerCrop));
                spawnPos.x += distBetweenAnchors;
            }

            spawnPos.x = startX;
            spawnPos.z += distBetweenAnchors;
        }
    }
}
