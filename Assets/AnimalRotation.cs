using System.Collections;
using UnityEngine;

public class AnimalRotation : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 axis;
    [SerializeField] private Vector2 minMaxTimeBetweenDecisions = new Vector2(3, 10);
    [SerializeField] private Vector2 chanceToRotate = new Vector2(1, 3);
    [SerializeField] private Vector2 minMaxTimeSpentRotating = new Vector2(1, 2);
    private bool isRotating;
    private bool positiveRotation;
    public bool Overriden { get; set; }
    public bool Disabled { get; set; }

    public IEnumerator LookTo(Vector3 position)
    {
        position.y = transform.position.y;
        Quaternion targetRot = Quaternion.LookRotation(position - transform.position);

        while (transform.rotation != targetRot)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
            yield return null;
        }
    }

    [ContextMenu("Start")]
    private void StartRotating()
    {
        isRotating = true;
    }

    [ContextMenu("Stop")]
    private void StopRotating()
    {
        isRotating = false;
    }

    [ContextMenu("Change Direction")]
    private void ChangeDirection()
    {
        positiveRotation = !positiveRotation;
    }

    private IEnumerator Brain()
    {
        if (Disabled) yield break;

        yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenDecisions));

        if (Disabled) yield break;

        if (RandomHelper.EvaluateChanceTo(chanceToRotate))
        {
            if (!Overriden)
            {
                if (RandomHelper.EvaluateChanceTo(new Vector2(1, 2))) ChangeDirection();
                if (Disabled) yield break;
                StartRotating();
                yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeSpentRotating));
                StopRotating();
            }
        }

        StartCoroutine(Brain());
    }

    private void Start()
    {
        StartCoroutine(Brain());
    }

    private void Update()
    {
        if (Disabled) return;
        if (!isRotating) return;
        if (!Overriden)
        {
            transform.Rotate(axis, (positiveRotation ? 1 : -1) * rotateSpeed * Time.deltaTime);
        }
    }
}
