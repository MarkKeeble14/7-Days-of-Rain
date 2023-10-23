using System.Collections;
using UnityEngine;

public class EyelidsTransitionAnimation : TransitionAnimation
{
    [Header("Sleep Sequences")]
    [SerializeField] private RectTransform topLid;
    [SerializeField] private RectTransform bottomLid;
    [SerializeField] private Vector3 topGoalPos;
    [SerializeField] private Vector3 topRestPos;
    [SerializeField] private Vector3 botGoalPos;
    [SerializeField] private Vector3 botRestPos;
    [SerializeField] private float blinkSpeed;

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
        yield return StartCoroutine(Close(blinkSpeed));
    }

    protected override IEnumerator In()
    {
        yield return StartCoroutine(Open(blinkSpeed));
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
