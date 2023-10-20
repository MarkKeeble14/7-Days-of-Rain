using System.Collections;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GroundGenerator groundGenerator;
    [SerializeField] private PopulateWithFoliage populateWithFoliage;
    [SerializeField] private CropSpawner[] spawnCropsOnGenerate;

    private void Awake()
    {
        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        groundGenerator.GenerateGround();

        yield return new WaitForSeconds(0.05f);

        foreach (CropSpawner cropSpawner in spawnCropsOnGenerate)
        {
            cropSpawner.Generate();
        }

        yield return new WaitForSeconds(0.05f);
        populateWithFoliage.Populate();

        GameManager._Instance.LockTotalCropCount();
    }
}
