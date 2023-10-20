using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSprintDisplayController : MonoBehaviour
{
    public static CanSprintDisplayController _Instance { get; private set; }
    private void Awake()
    {
        _Instance = this;

        CanSprint = false;
    }

    public bool CanSprint { get; set; }
    [SerializeField] private GameObject display;

    private void Update()
    {
        display.SetActive(CanSprint);
    }
}
