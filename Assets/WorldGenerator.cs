using System.Collections;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GroundGenerator groundGenerator;
    [SerializeField] private PopulateWithFoliage populateWithFoliage;

    private void Awake()
    {
        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        groundGenerator.GenerateGround();

        yield return new WaitForSeconds(0.05f);

        foreach (CropSpawner cropSpawner in GetComponentsInChildren<CropSpawner>())
        {
            cropSpawner.Generate();
        }

        yield return new WaitForSeconds(0.05f);
        populateWithFoliage.Populate();
    }
}
