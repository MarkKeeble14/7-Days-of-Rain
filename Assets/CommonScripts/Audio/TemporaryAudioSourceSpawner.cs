using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryAudioSourceSpawner : MonoBehaviour
{
    public void PlayOneShot(AudioClipContainer toPlay)
    {
        TemporaryAudioSource tempSorce = Instantiate(Resources.Load<TemporaryAudioSource>("Audio/TemporaryAudioSource"), transform.position, Quaternion.identity);
        tempSorce.Play(toPlay);
    }
}
