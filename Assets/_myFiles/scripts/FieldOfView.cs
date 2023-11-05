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

     float test;

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
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radiusSuspicious, targetMask);

        if (rangeCheck.Length > 0)
        {
            canSeePlayer = rangeChecker(rangeCheck);

            if (canSeePlayer)
            {
                Debug.Log("saw");
                float distToPlayer = Vector3.Distance(transform.position, rangeCheck[0].transform.position);

                if (distToPlayer > radiusHostile && distToPlayer < radiusSuspicious)
                {
                    return EFOVState.Suspicious;
                }
                else if (distToPlayer < radiusHostile)
                {
                    return EFOVState.Hostile;
                }
                else { return EFOVState.Nothing; }
            }
            else { return EFOVState.Nothing; }
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
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2 && distanceToTarget < radiusSuspicious)
        {
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask) )
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
