using System.Collections;
using UnityEngine;

public abstract class TransitionAnimation : MonoBehaviour
{
    public abstract IEnumerator Out();
    public abstract IEnumerator In();
}
