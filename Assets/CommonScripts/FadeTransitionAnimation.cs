using System.Collections;
using UnityEngine;

public class FadeTransitionAnimation : TransitionAnimation
{
    [SerializeField] private CanvasGroup cv;
    [SerializeField] private float fadeInSpeed;
    [SerializeField] private float fadeOutSpeed;

    public override IEnumerator Out()
    {
        yield return Utils.ChangeCanvasGroupAlpha(cv, 1, fadeInSpeed);
    }

    public override IEnumerator In()
    {
        yield return Utils.ChangeCanvasGroupAlpha(cv, 0, fadeOutSpeed);
    }
}