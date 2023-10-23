using System.Collections;
using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 minMaxTimeBetweenDecisions = new Vector2(3, 10);
    [SerializeField] private Vector2 chanceToMove = new Vector2(1, 3);
    [SerializeField] private Vector2 minMaxTimeSpentMoving = new Vector2(1, 2);
    private bool isMoving;
    private Vector3 targetPos;
    [SerializeField] private Vector2 minMaxXMovement;
    [SerializeField] private Vector2 minMaxZMovement;
    [SerializeField] private Vector2 chanceToChangeTargetPos;
    [SerializeField] private AnimalRotation animalRot;
    [SerializeField] private Transform toMove;
    private Vector2 minMaxXBounds;
    private Vector2 minMaxZBounds;

    [SerializeField] private bool footstepAudioEnabled;
    [SerializeField] private float timeBetweenFootsteps;
    [SerializeField] private RandomClipAudioClipContainer footsteps;

    public bool Disabled { get; set; }

    public void SetBounds(Vector2 xBounds, Vector2 zBounds)
    {
        minMaxXBounds = xBounds;
        minMaxZBounds = zBounds;
    }

    [ContextMenu("Start")]
    private void StartMoving()
    {
        isMoving = true;
        footstepTimer = 0;
    }

    [ContextMenu("Stop")]
    private void StopMoving()
    {
        isMoving = false;
    }

    [ContextMenu("Set Target Position")]
    private void SetTargetPos()
    {
        targetPos = new Vector3(
            toMove.localPosition.x + RandomHelper.RandomFloat(minMaxXMovement),
            toMove.localPosition.y,
            toMove.localPosition.z + RandomHelper.RandomFloat(minMaxZMovement));

        // Adjust based on bounds
        if (targetPos.x < minMaxXBounds.x) targetPos.x = minMaxXBounds.x;
        if (targetPos.x > minMaxXBounds.y) targetPos.x = minMaxXBounds.y;
        if (targetPos.z < minMaxZBounds.x) targetPos.z = minMaxZBounds.x;
        if (targetPos.z > minMaxZBounds.y) targetPos.z = minMaxZBounds.y;
    }

    private IEnumerator Brain()
    {
        if (Disabled) yield break;

        yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenDecisions));

        if (Disabled) yield break;

        if (RandomHelper.EvaluateChanceTo(chanceToMove))
        {
            if (RandomHelper.EvaluateChanceTo(chanceToChangeTargetPos)) SetTargetPos();

            animalRot.Overriden = true;
            yield return StartCoroutine(animalRot.LookTo(targetPos));
            StartMoving();
            yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeSpentMoving));
            StopMoving();
            animalRot.Overriden = false;
        }

        StartCoroutine(Brain());
    }

    private void Start()
    {
        StartCoroutine(Brain());
    }

    private float footstepTimer;
    private void Update()
    {
        if (Disabled) return;
        if (!isMoving) return;
        toMove.localPosition = Vector3.MoveTowards(toMove.localPosition, targetPos, Time.deltaTime * moveSpeed);

        if (footstepAudioEnabled)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                footsteps.PlayOneShot();
                footstepTimer = timeBetweenFootsteps;
            }
        }
    }
}