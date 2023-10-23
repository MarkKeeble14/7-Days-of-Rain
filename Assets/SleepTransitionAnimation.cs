using System.Collections;
using UnityEngine;


public class SleepTransitionAnimation : TransitionAnimation
{
    [Header("Sleep Sequences")]
    [SerializeField] private RectTransform topLid;
    [SerializeField] private RectTransform bottomLid;
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
        while (topLid.anchoredPosition.y != topGoalPos.y)
        {
            topLid.anchoredPosition = Vector3.MoveTowards(topLid.anchoredPosition, topGoalPos, Time.deltaTime * speed);
            bottomLid.anchoredPosition = Vector3.MoveTowards(bottomLid.anchoredPosition, botGoalPos, Time.deltaTime * speed);
            yield return null;
        }
    }

    private IEnumerator Open(float speed)
    {
        while (topLid.anchoredPosition.y != topRestPos.y)
        {
            topLid.anchoredPosition = Vector3.MoveTowards(topLid.anchoredPosition, topRestPos, Time.deltaTime * speed);
            bottomLid.anchoredPosition = Vector3.MoveTowards(bottomLid.anchoredPosition, botRestPos, Time.deltaTime * speed);
            yield return null;
        }
    }

    public override void SetToOutState()
    {
        topLid.anchoredPosition = topGoalPos;
        bottomLid.anchoredPosition = botGoalPos;
    }

    public override void SetToInState()
    {
        topLid.anchoredPosition = topRestPos;
        bottomLid.anchoredPosition = botRestPos;
    }

    protected override IEnumerator Out()
    {
        ShowGameEventTriggerOpporotunity._Instance.Clear();

        yield return new WaitUntil(() => !GameManager._Instance.CameraController.Transitioning);

        // Blink Twice
        for (int i = 0; i < numBlinks; i++)
        {
            yield return StartCoroutine(Blink(blinkSpeed, blinkSpeed));
        }

        yield return new WaitForSeconds(timeAfterBlinkBeforeClose);

        // Close Eyes
        yield return StartCoroutine(Close(sleepEyesSpeed));
    }

    protected override IEnumerator In()
    {
        // Open Eyes
        yield return StartCoroutine(Open(wakeUpEyesSpeed));

        GameManager._Instance.CameraController.SetTransitionSettings(wakeUpPosChangeRate, wakeUpRotChangeRate);
        GameManager._Instance.CameraController.SetDefaultSubject();

        yield return new WaitUntil(() => !GameManager._Instance.CameraController.Transitioning);
    }

    public override void Enable()
    {
        topLid.gameObject.SetActive(true);
        bottomLid.gameObject.SetActive(true);
    }

    public override void Disable()
    {
        topLid.gameObject.SetActive(false);
        bottomLid.gameObject.SetActive(false);
    }
}
