using System.Collections;
using UnityEngine;

public class FadeTransitionAnimation : TransitionAnimation
{
    [SerializeField] private CanvasGroup cv;
    [SerializeField] private float fadeInSpeed;
    [SerializeField] private float fadeOutSpeed;

    protected override IEnumerator Out()
    {
        yield return Utils.ChangeCanvasGroupAlpha(cv, 1, fadeInSpeed);
    }

    protected override IEnumerator In()
    {
        yield return Utils.ChangeCanvasGroupAlpha(cv, 0, fadeOutSpeed);
    }

    public override void SetToOutState()
    {
        cv.alpha = 1;
    }

    public override void SetToInState()
    {
        cv.alpha = 0;
    }

    public override void Enable()
    {
        cv.gameObject.SetActive(true);
    }

    public override void Disable()
    {
        cv.gameObject.SetActive(false);
    }
}