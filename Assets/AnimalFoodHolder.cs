using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFoodHolder : MonoBehaviour
{
    [SerializeField] private GameObject foodRepresentation;
    private bool isFilled;
    public bool IsFilled
    {
        get
        {
            return isFilled;
        }
        set
        {
            isFilled = value;
            foodRepresentation.SetActive(isFilled);
        }
    }

    private void Start()
    {
        GameManager._Instance.OnEndOfDay += () => isFilled = false;
    }
}
