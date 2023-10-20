using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager _Instance { get; private set; }

    [SerializeField] private SerializableDictionary<string, TransitionAnimation> animationMap;

    [SerializeField] private PlayerInput p_Input;
    [SerializeField] private string playOnStart = "";
    [SerializeField] private float delayOnStartTransitionBy;

    [SerializeField] private CanvasGroup cv;

    public bool Transitioning { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    private void Start()
    {
        if (playOnStart.Length > 0)
        {
            StartCoroutine(Transition(PlayIn(playOnStart, delayOnStartTransitionBy)));
        }
    }

    private void Update()
    {
        cv.blocksRaycasts = Transitioning;
    }

    public IEnumerator Transition(IEnumerator enumerator, bool lockInput = false)
    {
        Transitioning = true;
        p_Input.LockInput = lockInput;

        yield return StartCoroutine(enumerator);

        p_Input.LockInput = false;
        Transitioning = false;
    }

    private IEnumerator PlayIn(string key, float delay, Action onStart = null, Action onEnd = null)
    {
        yield return new WaitForSeconds(delay);

        onStart?.Invoke();
        if (animationMap.ContainsKey(key))
        {
            yield return StartCoroutine(animationMap[key].In());
        }
        onEnd?.Invoke();
    }

    private IEnumerator PlayOut(string key, float delay, Action onStart = null, Action onEnd = null)
    {
        onStart?.Invoke();
        if (animationMap.ContainsKey(key))
        {
            yield return StartCoroutine(animationMap[key].Out());
        }
        onEnd?.Invoke();
    }

    public IEnumerator PlayAnimationWithActionsInBetween(List<AnimationActionSequenceEntry> animationSequenceData)
    {
        yield return PlayAnimationWithActionsInBetween(animationSequenceData.ToArray());
    }

    public IEnumerator PlayAnimationWithActionsInBetween(AnimationActionSequenceEntry[] animationSequenceData)
    {
        for (int i = 0; i < animationSequenceData.Length; i++)
        {
            AnimationActionSequenceEntry data = animationSequenceData[i];

            if (data.IsIn)
            {
                yield return StartCoroutine(Transition(PlayIn(data.AnimationKey, 0, data.OnStart, data.OnEnd), data.LockInput));
            }
            else
            {
                yield return StartCoroutine(Transition(PlayOut(data.AnimationKey, 0, data.OnStart, data.OnEnd), data.LockInput));
            }

            yield return new WaitForSeconds(data.TimeAfter);
        }
    }
}

[System.Serializable]
public struct AnimationActionSequenceEntry
{
    public string AnimationKey;
    public Action OnStart;
    public Action OnEnd;
    public float TimeAfter;
    public bool IsIn;
    public bool LockInput;

    public AnimationActionSequenceEntry(string animationKey, Action onStart, Action onEnd, float timeBetween, bool isIn, bool lockInput)
    {
        AnimationKey = animationKey;
        OnStart = onStart;
        OnEnd = onEnd;
        TimeAfter = timeBetween;
        IsIn = isIn;
        LockInput = lockInput;
    }
}
