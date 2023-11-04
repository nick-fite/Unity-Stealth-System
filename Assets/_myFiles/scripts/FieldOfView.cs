using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float radiusHostile;
    [SerializeField] private float radiusSuspicious;
    [Range(0, 360)][SerializeField] private float angle;
    [SerializeField] private float timeBetweenChecks;

    //private GameObject playerRef;

    private LayerMask targetMask;
    private LayerMask obstructionMask;

    [SerializeField] private bool canSeePlayer;

    [SerializeField] private EEnemyState enemyState;

    private void Start()
    {
        targetMask = LayerMask.GetMask("TargetMask");
        obstructionMask = LayerMask.GetMask("ObstructionMask");
        enemyState = EEnemyState.Default;
        StartCoroutine(FOVRoutine());
    }

    IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenChecks);
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
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
            enemyState = EEnemyState.Hostile;
        }
        else if (suspicious && !hostile)
        {
            enemyState = EEnemyState.Suspicious;
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
    public EEnemyState GetEnemyState() { return enemyState; }
    public void SetEnemyState(EEnemyState state) { enemyState = state; }
}
