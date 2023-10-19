using System.Collections;
using UnityEngine;

public class CoroutineCaller : MonoBehaviour
{
    public void CallCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(Lifetime(coroutine));
    }

    private IEnumerator Lifetime(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);

        Destroy(gameObject);
    }
}
