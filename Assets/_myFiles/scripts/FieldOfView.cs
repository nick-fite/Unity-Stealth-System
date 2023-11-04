using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float radiusHostile;
    [SerializeField] private float radiusSuspicious;
    [Range(0, 360)][SerializeField] private float angle;

    //private GameObject playerRef;

    private LayerMask targetMask;
    private LayerMask obstructionMask;

    [SerializeField] private bool canSeePlayer;


    private void Start()
    {
        targetMask = LayerMask.GetMask("TargetMask");
        obstructionMask = LayerMask.GetMask("ObstructionMask");
        //StartCoroutine(FOVRoutine());
    }

    /*IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }*/

    public EFOVState FieldOfViewCheck()
    {
        Collider[] hostileRangeCheck = Physics.OverlapSphere(transform.position, radiusHostile, targetMask);
        Collider[] suspiciousRangeCheck = Physics.OverlapSphere(transform.position, radiusSuspicious, targetMask);

        bool hostile = false;
        bool suspicious = false;

        if (hostileRangeCheck.Length != 0)
        {
            canSeePlayer = rangeChecker(hostileRangeCheck);
            hostile = true;
        }
        else if (suspiciousRangeCheck.Length != 0)
        {
            canSeePlayer = rangeChecker(suspiciousRangeCheck);
            suspicious = true;
        }
        else if (canSeePlayer) { canSeePlayer = false; }

        if ((suspicious && hostile) || hostile)
        {
            return EFOVState.Hostile;
        }
        else if (suspicious && !hostile)
        {
            return EFOVState.Suspicious;
        }
        else
        {
            return EFOVState.Nothing;
        }
    }

    public bool rangeChecker(Collider[] collidersToCheck)
    {
        Transform target = collidersToCheck[0].transform;
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else { return false; }
    }

    public float GetAngle() { return angle; }
    public float GetRadiusHostile() { return radiusHostile; }
    public float GetRadiusSuspicious() { return radiusSuspicious; }
    public bool GetCanSeePlayer() { return canSeePlayer; }
}

public enum EFOVState {Hostile, Suspicious, Nothing };
