using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float radiusHostile;
    [SerializeField] private float radiusSuspicious;
    [Range(0, 360)][SerializeField] private float angle;

    //private GameObject playerRef;

    private LayerMask targetMask;
    private LayerMask obstructionMask;

    [SerializeField] private bool canSeePlayer;
    [SerializeField] Transform RayStart;

    private GameObject ObstructionBehindPlayer;

     float test;

    private void Start()
    {
        targetMask = LayerMask.GetMask("TargetMask");
        obstructionMask = LayerMask.GetMask("ObstructionMask");
        //StartCoroutine(FOVRoutine());
    }

    public EFOVState FieldOfViewCheck()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radiusSuspicious, targetMask);

        if (rangeCheck.Length > 0)
        {
            canSeePlayer = rangeChecker(rangeCheck);

            if (canSeePlayer)
            {
                float distToPlayer = Vector3.Distance(transform.position, rangeCheck[0].transform.position);

                if (distToPlayer > radiusHostile && distToPlayer < radiusSuspicious)
                {
                    Renderer obstructionRenderer;
                    if (ObstructionBehindPlayer && ObstructionBehindPlayer.TryGetComponent(out obstructionRenderer)) 
                    {
                        if (obstructionRenderer.material != GameObject.FindGameObjectWithTag("PlayerAlphaSurface").GetComponent<Renderer>().material)
                        {
                            return EFOVState.Suspicious;
                        }
                        else { return EFOVState.Nothing; }
                    }
                    else
                    {
                        return EFOVState.Suspicious;
                    }
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
            
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask ))
            {
                RaycastHit obstruction;
                if (Physics.Raycast(RayStart.position, directionToTarget, out obstruction, distanceToTarget + 10f, obstructionMask))
                {
                    ObstructionBehindPlayer = obstruction.collider.gameObject;
                    Debug.Log(ObstructionBehindPlayer.name);
                }
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
