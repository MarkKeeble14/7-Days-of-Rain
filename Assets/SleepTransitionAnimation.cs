using System.Collections;
using UnityEngine;

public class SleepTransitionAnimation : TransitionAnimation
{
    [Header("Sleep Sequences")]
    [SerializeField] private RectTransform topBlackBar;
    [SerializeField] private RectTransform bottomBlackBar;
    [SerializeField] private Vector3 topGoalPos;
    [SerializeField] private Vector3 topRestPos;
    [SerializeField] private Vector3 botGoalPos;
    [SerializeField] private Vector3 botRestPos;
    [SerializeField] private float blinkSpeed;
    [SerializeField] private float sleepEyesSpeed;
    [SerializeField] private float wakeUpEyesSpeed;
    [SerializeField] private float wakeUpPosChangeRate;
    [SerializeField] private float wakeUpRotChangeRate;
    [SerializeField] private int numBlinks = 2;

    [SerializeField] private float timeAfterBlinkBeforeClose = .25f;

    [Header("References")]
    [SerializeField] private PlayerInput p_Input;

    private IEnumerator Blink(float closeSpeed, float openSpeed)
    {
        yield return StartCoroutine(Close(closeSpeed));
        yield return StartCoroutine(Open(openSpeed));
    }

    private IEnumerator Close(float speed)
    {
        while (topBlackBar.anchoredPosition.y != topGoalPos.y)
        {
            topBlackBar.anchoredPosition = Vector3.MoveTowards(topBlackBar.anchoredPosition, topGoalPos, Time.deltaTime * speed);
            bottomBlackBar.anchoredPosition = Vector3.MoveTowards(bottomBlackBar.anchoredPosition, botGoalPos, Time.deltaTime * speed);
            yield return null;
        }
    }

    private IEnumerator Open(float speed)
    {
        while (topBlackBar.anchoredPosition.y != topRestPos.y)
        {
            topBlackBar.anchoredPosition = Vector3.MoveTowards(topBlackBar.anchoredPosition, topRestPos, Time.deltaTime * speed);
            bottomBlackBar.anchoredPosition = Vector3.MoveTowards(bottomBlackBar.anchoredPosition, botRestPos, Time.deltaTime * speed);
            yield return null;
        }
    }

    public override IEnumerator Out()
    {
        ShowGameEventTriggerOpporotunity._Instance.Clear();

        yield return new WaitUntil(() => !GameManager._Instance.ActiveCamera.Transitioning);

        topBlackBar.gameObject.SetActive(true);
        bottomBlackBar.gameObject.SetActive(true);

        // Blink Twice
        for (int i = 0; i < numBlinks; i++)
        {
            yield return StartCoroutine(Blink(blinkSpeed, blinkSpeed));
        }

        yield return new WaitForSeconds(timeAfterBlinkBeforeClose);

        // Close Eyes
        yield return StartCoroutine(Close(sleepEyesSpeed));
    }

    public override IEnumerator In()
    {
        // Open Eyes
        yield return StartCoroutine(Open(wakeUpEyesSpeed));

        GameManager._Instance.ActiveCamera.SetTransitionSettings(true, wakeUpPosChangeRate, wakeUpRotChangeRate);
        GameManager._Instance.ActiveCamera.SetDefaultSubject();

        yield return new WaitUntil(() => !GameManager._Instance.ActiveCamera.Transitioning);

        topBlackBar.gameObject.SetActive(false);
        bottomBlackBar.gameObject.SetActive(false);
    }
}
