using System.Collections;
using UnityEngine;

public abstract class TransitionAnimation : MonoBehaviour
{
    public IEnumerator CallOut()
    {
        Enable();
        yield return StartCoroutine(Out());
    }

    public IEnumerator CallIn()
    {
        Enable();
        yield return StartCoroutine(In());
    }

    protected abstract IEnumerator Out();
    protected abstract IEnumerator In();

    public abstract void SetToInState();
    public abstract void SetToOutState();

    public abstract void Enable();
    public abstract void Disable();
}
