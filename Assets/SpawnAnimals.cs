using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimals : MonoBehaviour
{
    [SerializeField] private AnimalData animalType;
    [SerializeField] private Transform spawnOn;
    [SerializeField] private Vector2 minMaxAnimals;
    [SerializeField] private Vector2 xBound;
    [SerializeField] private Vector2 zBound;
    [SerializeField] private Vector2 minMaxSpawnOffsetX;
    [SerializeField] private Vector2 minMaxSpawnOffsetZ;

    public void Spawn()
    {
        for (int i = 0; i < RandomHelper.RandomIntExclusive(minMaxAnimals); i++)
        {
            AnimalData animal = Instantiate(animalType, spawnOn);
            animal.Movement.SetBounds(xBound, zBound);
            animal.transform.position += new Vector3(
                RandomHelper.RandomFloat(minMaxSpawnOffsetX),
                0,
                RandomHelper.RandomFloat(minMaxSpawnOffsetZ));
        }
    }
}
