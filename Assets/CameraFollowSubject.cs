using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraFollowStyle
{
    LOOKAT_WITH_OFFSET,
    MATCH,
    NONE,
    PURE_LOOKAT
}

[System.Serializable]
public struct CameraFollowSubjectData
{
    public Transform Subject;
    public float FollowSpeed;
    public CameraFollowStyle FollowStyle;
    public float RotateSpeed;
    public Vector3 Offset;
    public bool IsPlayer;
    public bool ReleaseMovementLockOnTransitionEnd;
    public bool UseDefaultData;
}

public class CameraFollowSubject : MonoBehaviour
{
    [SerializeField] private bool active;
    public bool Active { get { return active; } set { active = value; } }

    [Header("Init Data")]
    [SerializeField] private CameraFollowSubjectData subjectData;
    private CameraFollowSubjectData currentSubjectData;
    private Transform subject;
    private float followSpeed;
    private CameraFollowStyle followStyle;
    private float rotateSpeed;
    private Vector3 offset;
    private Vector3 targetPos;
    private Quaternion targetRot;

    [Header("Head Bob")]
    [SerializeField] private bool enableCameraBob;
    [SerializeField] private float headBobStrength;
    [SerializeField] private float headBobSpeed;
    [SerializeField] private float sprintingHeadBobStrengthMod;
    [SerializeField] private float sprintingHeadBobSpeedMod;
    private float headBobPos;
    private float headBobTimer;
    private float lastHeadBobSinValue;
    private bool canPlayFootstep;

    [Header("References")]
    [SerializeField] private PlayerInput p_Input;
    [SerializeField] private PlayerFootstepsSoundController p_Footsteps;

    [Header("Transition Settings")]
    [SerializeField] private float transitionPosGrace = 1;
    [SerializeField] private float transitionRotGrace = 1;
    [SerializeField] private float defaultTransitionPosSpeed;
    [SerializeField] private float defaultTransitionRotSpeed;
    [SerializeField] private bool defaultUseTransition;
    private float transitionRotSpeed;
    private float transitionPosSpeed;
    private bool useTransition;
    public bool Transitioning { get; private set; }

    [Header("Looking")]
    [SerializeField] private float playerLookSpeed;
    [SerializeField] private Vector2 verticalLookBounds;
    [SerializeField] private Vector2 horizontalLookBounds;
    [SerializeField] private float playerLookReleaseSpeed;
    private float playerVertLookMod;
    private float playerHorizontalLookMod;
    private int subjectDataBuffer = 1;

    private void Awake()
    {
        transitionPosSpeed = defaultTransitionPosSpeed;
        transitionRotSpeed = defaultTransitionRotSpeed;
        useTransition = defaultUseTransition;
        SetNewSubject(subjectData);
    }

    public void SetNewSubject(CameraFollowSubjectData data)
    {
        if (data.UseDefaultData)
        {
            currentSubjectData = subjectData;
        }
        else
        {
            currentSubjectData = data;
        }
        NewSubjectSet();
    }

    public void SetDefaultSubject()
    {
        currentSubjectData = subjectData;
        NewSubjectSet();
    }

    public void SetTransitionSettings(bool useTransition, float posChangeRate, float rotChangeRate)
    {
        this.useTransition = useTransition;
        transitionPosSpeed = posChangeRate;
        transitionRotSpeed = rotChangeRate;
    }

    public void NewSubjectSet()
    {
        subject = currentSubjectData.Subject;
        followSpeed = currentSubjectData.FollowSpeed;
        followStyle = currentSubjectData.FollowStyle;
        rotateSpeed = currentSubjectData.RotateSpeed;
        offset = currentSubjectData.Offset;
        enableCameraBob = currentSubjectData.IsPlayer;

        // set target pos here first so wait for transitions doesn't judge immedietely
        SetTargetPos();

        if (useTransition)
        {
            p_Input.LockInput = true;
            Transitioning = true;
            GameManager._Instance.StartCoroutine(WaitForTransition(currentSubjectData.ReleaseMovementLockOnTransitionEnd));
        }
        else
        {
            transform.position = targetPos;
        }
    }

    private IEnumerator WaitForTransition(bool releaseMovementLockOnTransitionEnd)
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, targetPos) < transitionPosGrace
                && Vector3.Distance(transform.rotation.eulerAngles, targetRot.eulerAngles) < transitionRotGrace)
            {
                break;
            }
            yield return null;
        }
        Transitioning = false;
        p_Input.LockInput = false;
        if (releaseMovementLockOnTransitionEnd)
        {
            GameManager._Instance.LockMovement = false;
        }
    }

    public void SetToTargetPos()
    {
        transform.position = targetPos;
    }

    private void SetTargetPos()
    {
        if (followStyle == CameraFollowStyle.PURE_LOOKAT)
        {
            targetPos = transform.position;
        }
        else
        {
            targetPos = subject.transform.position + offset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Active) return;

        SetTargetPos();
        if (enableCameraBob)
        {
            if ((p_Input.MoveUp || p_Input.MoveRight || p_Input.MoveLeft || p_Input.MoveDown) && !GameManager._Instance.LockMovement)
            {
                headBobTimer += Time.deltaTime * headBobSpeed * (p_Input.Sprinting ? sprintingHeadBobSpeedMod : 1);
                float newHeadBobSinValue = Mathf.Sin(headBobTimer);
                headBobPos = newHeadBobSinValue * headBobStrength * (p_Input.Sprinting ? sprintingHeadBobStrengthMod : 1);
                if (newHeadBobSinValue > lastHeadBobSinValue && canPlayFootstep)
                {
                    p_Footsteps.PlayStep();
                    canPlayFootstep = false;
                }
                if (newHeadBobSinValue < lastHeadBobSinValue && !canPlayFootstep)
                {
                    canPlayFootstep = true;
                }
                lastHeadBobSinValue = newHeadBobSinValue;
            }
            else
            {
                headBobTimer = 0;
                headBobPos = Mathf.Lerp(headBobPos, 0, Time.deltaTime * headBobStrength);
            }
            targetPos.y += headBobPos;
        }

        // Adjust Pos
        float changePosRate = (!Transitioning ? followSpeed : transitionPosSpeed);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * changePosRate);

        switch (followStyle)
        {
            case CameraFollowStyle.LOOKAT_WITH_OFFSET:
                targetRot = Quaternion.LookRotation(subject.transform.position - transform.position);
                break;
            case CameraFollowStyle.PURE_LOOKAT:
                targetRot = Quaternion.LookRotation(subject.transform.position - transform.position);
                break;
            case CameraFollowStyle.MATCH:
                targetRot = subject.transform.rotation;
                break;
            default:
                targetRot = transform.rotation;
                break;
        }

        // Looking
        // Determine Look Mod Changes
        // Vertical Looking
        if (p_Input.LookUp && playerVertLookMod > verticalLookBounds.x)
        {
            playerVertLookMod = Mathf.Lerp(playerVertLookMod, verticalLookBounds.x, Time.deltaTime * playerLookSpeed);
        }
        if (p_Input.LookDown && playerVertLookMod < verticalLookBounds.y)
        {
            playerVertLookMod = Mathf.Lerp(playerVertLookMod, verticalLookBounds.y, Time.deltaTime * playerLookSpeed);
        }
        if (GameManager._Instance.LockMovement)
        {
            // Horizontal Looking
            if (p_Input.TurnLeft && playerHorizontalLookMod > horizontalLookBounds.x)
            {
                playerHorizontalLookMod = Mathf.Lerp(playerHorizontalLookMod, horizontalLookBounds.x, Time.deltaTime * playerLookSpeed);
            }
            if (p_Input.TurnRight && playerVertLookMod < horizontalLookBounds.y)
            {
                playerHorizontalLookMod = Mathf.Lerp(playerHorizontalLookMod, horizontalLookBounds.y, Time.deltaTime * playerLookSpeed);
            }
        }

        // Released
        if (!p_Input.LookUp && !p_Input.LookDown)
        {
            playerVertLookMod = Mathf.Lerp(playerVertLookMod, 0, Time.deltaTime * playerLookReleaseSpeed);
        }
        if (!p_Input.TurnRight && !p_Input.TurnLeft)
        {
            playerHorizontalLookMod = Mathf.Lerp(playerHorizontalLookMod, 0, Time.deltaTime * playerLookReleaseSpeed);
        }
        // Adjust via Look Mod
        targetRot *= Quaternion.Euler(Vector3.right * playerVertLookMod);
        targetRot *= Quaternion.Euler(Vector3.up * playerHorizontalLookMod);

        // Adjust Rot
        float changeRotRate = (!Transitioning ? rotateSpeed : transitionRotSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * changeRotRate);
    }

    public IEnumerator ShowNewSubjectSequence(CameraFollowSubjectData[] data, float delay)
    {
        GameManager._Instance.LockMovement = true;

        for (int i = 0; i < data.Length; i++)
        {
            SetNewSubject(data[i]);

            yield return new WaitForSeconds(delay);
        }

        GameManager._Instance.LockMovement = false;
    }
}
