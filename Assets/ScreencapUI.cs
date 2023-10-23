using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreencapUI : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            gameObject.SetActive(false);
        }
    }
}
