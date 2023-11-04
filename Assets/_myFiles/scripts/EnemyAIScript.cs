using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(FieldOfView))]
public class EnemyAIScript : MonoBehaviour
{
    public List<GameObject> waypoints;
    private int waypointIndex;

    private GameObject playerRef;
    private Vector3 playerLastSeenPos;
    private Vector3 prevPlayerLastSeenPos;

    private FieldOfView fov;
    
    private NavMeshAgent enemy_NavMeshAgent;
    
    private Animator enemy_Animator;
    
    private Vector3 prevPosition;
    
    [SerializeField] private bool startAI = true;
    
    public float curSpeed;

    public bool isCoward;
    public Transform alarmPos;

    private bool GetPlayerPosOnce;
    private bool FindingPlayer;
    [SerializeField] private bool IsHostile;

    private void Start()
    {
        playerRef = GameManager.m_Instance.GetPlayer();

        for (int i = 0; i < transform.parent.gameObject.transform.childCount; i++)
        {
            if (transform.parent.GetChild(i).gameObject.tag == "EnemyPathPos")
            {
                waypoints.Add(transform.parent.GetChild(i).gameObject);
            }
        }

        if (waypoints.Count <= 0)
        {
            Debug.LogError(this.name + ": There are no waypoints");
        }
        prevPosition = transform.position;

        waypointIndex = 0;

        fov = GetComponent<FieldOfView>();
        enemy_NavMeshAgent = GetComponent<NavMeshAgent>();
        enemy_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (startAI) {
            Vector3 curMove = transform.position - prevPosition;
            curSpeed = curMove.magnitude / Time.deltaTime;
            prevPosition = transform.position;
            enemy_Animator.SetFloat("Speed", curSpeed);
            //enemy_Animator.SetFloat("MotionSpeed", curSpeed);


            switch (fov.GetEnemyState())
            {
                case EEnemyState.Suspicious:
                    Suspicious();
                    break;
                case EEnemyState.InvestigateHostile:
                    InvestigateHostile();
                    break;
                case EEnemyState.Hostile:
                    Hostile();
                    break;
                default:
                    if (waypoints.Count > 0 && !IsHostile)
                    {
                        float waypointDistance = Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position);
                        enemy_NavMeshAgent.destination = waypoints[waypointIndex].transform.position;

                        if (waypointDistance < 1f)
                        {
                            waypointIndex++;
                            if (waypointIndex >= waypoints.Count)
                            {
                                waypointIndex = 0;
                            }
                        }
                    }
                    else if (IsHostile)
                    {
                        Hostile();
                    }
                    break;
            }
        }
    }

    private void Suspicious()
    {
        if (!IsHostile)
        {
            enemy_NavMeshAgent.speed = 2;
            if (!FindingPlayer && fov.GetCanSeePlayer())
            {
                Debug.Log("GetPlayerPos");
                StartCoroutine(GetPlayerPos());
            }
            if (Vector3.Distance(transform.position, playerLastSeenPos) < 1f)
            {
                Debug.Log("waitForDefault");
                StartCoroutine(WaitBeforeDefault());
            }
        }
        else
        {
            Hostile();
        }
    }

    private void Hostile()
    {
        if (!isCoward)
        {
            if (fov.GetCanSeePlayer())
            {
                enemy_NavMeshAgent.speed = 4;
                IsHostile = true;
                enemy_NavMeshAgent.destination = playerRef.transform.position;

            }
            else
            {
                IsHostile = false;
                fov.SetEnemyState(EEnemyState.InvestigateHostile);
                InvestigateHostile();
            }
        }
        else
        {
            IsHostile = true;
            enemy_NavMeshAgent.speed = 4;
            enemy_NavMeshAgent.destination = alarmPos.position;
            if (!GetPlayerPosOnce)
            {
                playerLastSeenPos = playerRef.transform.position;
                GetPlayerPosOnce = true;
            }
            Debug.Log(playerLastSeenPos);
            if (Vector3.Distance(transform.position, alarmPos.position) < 1f) 
            {
                GetPlayerPosOnce = false;
                isCoward = false;
                IsHostile = false;
                fov.SetEnemyState(EEnemyState.InvestigateHostile);
                InvestigateHostile();
            }
        }
    }

    private void InvestigateHostile()
    {
        if (!fov.GetCanSeePlayer())
        {
            enemy_NavMeshAgent.destination = playerLastSeenPos;
        }
        else
        {
            fov.SetEnemyState(EEnemyState.Hostile);
        }
    }

    IEnumerator WaitBeforeDefault() 
    {
        yield return new WaitForSeconds(2f);
        fov.SetEnemyState(EEnemyState.Default);
    }

    IEnumerator GetPlayerPos()
    {
        FindingPlayer = true;
        enemy_NavMeshAgent.destination = transform.position;

        float timeToWait = 2.0f;
        while (timeToWait >= 0.0f) 
        {
            Debug.Log(timeToWait);
            timeToWait -= Time.deltaTime;
            if (Vector3.Distance(playerRef.transform.position, transform.position) < fov.GetRadiusSuspicious()) {
                playerLastSeenPos = playerRef.transform.position;
            }
            yield return null;
        }

        if (fov.GetCanSeePlayer())
        {
            fov.SetEnemyState(EEnemyState.Hostile);
            Hostile();
            yield return null;
        }
        else
        {
            enemy_NavMeshAgent.destination = playerLastSeenPos;
            yield return CheckBeforeLeaving();
        }
    }

    IEnumerator CheckBeforeLeaving()
    {
        /*float rate = (transform.rotation.y - 359) * 3f;

        float t = 0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            var rotY = Mathf.Lerp(transform.rotation.y, 359, Time.deltaTime * t);
            transform.rotation = Quaternion.Euler(0, rotY, 0);
            yield return null;
        }*/

        yield return new WaitForSeconds(4f);
        FindingPlayer = false;
        yield return null;
    }
}

public enum EEnemyState { Default, Suspicious, InvestigateHostile, Hostile}
