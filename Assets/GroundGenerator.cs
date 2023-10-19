using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] groundTiles;
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private float distBetweenAnchors;
    [SerializeField] private Transform spawnTilesOn;

    public void GenerateGround()
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
                GameObject tile = Instantiate(RandomHelper.GetRandomFromArray(groundTiles), spawnTilesOn);
                tile.transform.position = spawnPos;
                tile.transform.eulerAngles = new Vector3(0, RandomHelper.RandomIntInclusive(0, 3) * 90, 0);
                spawnPos.x += distBetweenAnchors;
            }

            spawnPos.x = startX;
            spawnPos.z += distBetweenAnchors;
        }
    }
}
