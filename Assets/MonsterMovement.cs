using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    public enum MonsterState
    {
        CHARGE,
        EXIT
    }

    [SerializeField] private float chargeSpeed;
    [SerializeField] private Transform target;
    private MonsterState state;

    [SerializeField] private Animator anim;
    [SerializeField] private Vector2 minMaxSpawnDistFromTarget;
    [SerializeField] private Vector2 minMaxRandomizeOffset;
    [SerializeField] private float minChargeDistToKill = 2.5f;
    [SerializeField] private LayerMask floorLayers;

    private void Start()
    {
        Sleep();
    }

    private void ChangeState(MonsterState state)
    {
        this.state = state;
        StateChange();
    }

    public void Sleep()
    {
        anim.SetBool("Run", false);
        ChangeState(MonsterState.EXIT);
        transform.position = Vector3.up * -250;
    }

    public void Wake()
    {
        Vector3 directionFromCenter = target.position.normalized;
        directionFromCenter.y = 0;
        Vector3 spawnPos = directionFromCenter * RandomHelper.RandomFloat(minMaxSpawnDistFromTarget);
        transform.position = spawnPos + new Vector3(1, 0, 1) * RandomHelper.RandomFloat(minMaxRandomizeOffset);
        ChangeState(MonsterState.CHARGE);
    }

    private IEnumerator ChargeState()
    {
        anim.SetBool("Run", true);

        while (state == MonsterState.CHARGE)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * chargeSpeed);
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, floorLayers);
            if (hit.transform != null)
            {
                newPos.y = hit.point.y;
            }
            transform.position = newPos;
            transform.rotation = Quaternion.LookRotation(target.position - transform.position);


            if (Vector3.Distance(transform.position, target.position) < minChargeDistToKill)
            {
                state = MonsterState.EXIT;
                GameManager._Instance.DeadEnding();
            }

            yield return null;
        }
        StateChange();
    }

    private void StateChange()
    {
        switch (state)
        {
            case MonsterState.CHARGE:
                StartCoroutine(ChargeState());
                break;
            default:
                return;
        }
    }
}
