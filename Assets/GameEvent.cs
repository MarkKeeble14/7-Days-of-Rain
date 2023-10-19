using System.Collections;
using UnityEngine;

public abstract class GameEvent : MonoBehaviour
{
    [SerializeField] private float delayOnActivation;

    public void CallActivate()
    {
        GameManager._Instance.StartCoroutine(Activation());
    }

    private IEnumerator Activation()
    {
        yield return new WaitForSeconds(delayOnActivation);

        Activate();
    }

    protected abstract void Activate();
}