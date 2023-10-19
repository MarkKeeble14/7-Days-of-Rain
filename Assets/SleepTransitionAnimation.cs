using System.Collections;
using UnityEngine;

public class SleepTransitionAnimation : TransitionAnimation
{
    [Header("Sleep Sequences")]
    [SerializeField] private RectTransform topBlackBar;
    [SerializeField] private RectTransform bottomBlackBar;
    [SerializeField] private float blinkSpeed;
    [SerializeField] private float closeEyesSpeed;
    [SerializeField] private float wakeUpPosChangeRate;
    [SerializeField] private float wakeUpRotChangeRate;

    [SerializeField] private float timeAfterBlinkBeforeClose = .25f;

    [Header("References")]
    [SerializeField] private PlayerInput p_Input;

    public override IEnumerator Out()
    {
        ShowGameEventTriggerOpporotunity._Instance.ClearCurrent();

        yield return new WaitUntil(() => !GameManager._Instance.ActiveCamera.Transitioning);

        topBlackBar.gameObject.SetActive(true);
        bottomBlackBar.gameObject.SetActive(true);

        Vector3 topBarStartPos = topBlackBar.transform.position;
        float topBarGoalHeight = topBlackBar.transform.position.y - (Screen.height / 2) * 1.25f;
        Vector3 topBarGoalPos = new Vector3(topBlackBar.transform.position.x, topBarGoalHeight, topBlackBar.transform.position.z);

        Vector3 bottomBarStartPos = bottomBlackBar.transform.position;
        float bottomBarGoalHeight = (Screen.height / 2) * 1.25f;
        Vector3 bottomBarGoalPos = new Vector3(bottomBlackBar.transform.position.x, bottomBarGoalHeight, bottomBlackBar.transform.position.z);

        // Blink Twice
        for (int i = 0; i < 2; i++)
        {
            while (topBlackBar.transform.position.y > topBarGoalHeight)
            {
                topBlackBar.transform.position = Vector3.MoveTowards(topBlackBar.transform.position, topBarGoalPos, Time.deltaTime * blinkSpeed);
                bottomBlackBar.transform.position = Vector3.MoveTowards(bottomBlackBar.transform.position, bottomBarGoalPos, Time.deltaTime * blinkSpeed);
                yield return null;
            }
            while (topBlackBar.transform.position.y < topBarStartPos.y)
            {
                topBlackBar.transform.position = Vector3.MoveTowards(topBlackBar.transform.position, topBarStartPos, Time.deltaTime * blinkSpeed);
                bottomBlackBar.transform.position = Vector3.MoveTowards(bottomBlackBar.transform.position, bottomBarStartPos, Time.deltaTime * blinkSpeed);
                yield return null;
            }
        }

        yield return new WaitForSeconds(timeAfterBlinkBeforeClose);

        // Close Eyes
        while (topBlackBar.transform.position != topBarGoalPos)
        {
            topBlackBar.transform.position = Vector3.MoveTowards(topBlackBar.transform.position, topBarGoalPos, Time.deltaTime * closeEyesSpeed);
            bottomBlackBar.transform.position = Vector3.MoveTowards(bottomBlackBar.transform.position, bottomBarGoalPos, Time.deltaTime * closeEyesSpeed);
            yield return null;
        }
    }

    public override IEnumerator In()
    {
        // Open Eyes
        float topBarGoalHeight = Screen.height + Screen.height / 2;
        Vector3 topBarGoalPos = new Vector3(topBlackBar.transform.position.x, topBarGoalHeight, topBlackBar.transform.position.z);

        float bottomBarGoalHeight = -Screen.height / 2;
        Vector3 bottomBarGoalPos = new Vector3(bottomBlackBar.transform.position.x, bottomBarGoalHeight, bottomBlackBar.transform.position.z);

        while (topBlackBar.transform.position != topBarGoalPos)
        {
            topBlackBar.transform.position = Vector3.MoveTowards(topBlackBar.transform.position, topBarGoalPos, Time.deltaTime * closeEyesSpeed);
            bottomBlackBar.transform.position = Vector3.MoveTowards(bottomBlackBar.transform.position, bottomBarGoalPos, Time.deltaTime * closeEyesSpeed);
            yield return null;
        }

        GameManager._Instance.ActiveCamera.SetTransitionSettings(true, wakeUpPosChangeRate, wakeUpRotChangeRate);
        GameManager._Instance.ActiveCamera.SetDefaultSubject();

        yield return new WaitUntil(() => !GameManager._Instance.ActiveCamera.Transitioning);

        topBlackBar.gameObject.SetActive(false);
        bottomBlackBar.gameObject.SetActive(false);
    }
}
