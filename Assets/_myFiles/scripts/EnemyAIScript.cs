using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(FieldOfView))]
public class EnemyAIScript : MonoBehaviour
{
    [SerializeField]EEnemyState EnemyState;

    public List<GameObject> PatrolWaypoints;
    public List<GameObject> searchWaypoints;
    private int PatrolWaypointIndex;
    private int SearchWaypointIndex;

    private GameObject playerRef;
    private Vector3 playerLastSeenPos;

    private FieldOfView fov;
    
    private NavMeshAgent enemy_NavMeshAgent;
    
    private Animator enemy_Animator;
    
    private Vector3 prevPosition;
    
    [SerializeField] private bool startAI = true;
    
    public float curSpeed;

    public bool isCoward;
    private Transform alarmPos;

    private bool GetPlayerPosOnce;
    private bool FindingPlayer;
    private bool IsHostile;
    private bool isInvestigateHostile;
    [SerializeField]private bool hasInvestiagedLastPos;

    private float Health;
    private Rigidbody[] RagDollRigidbodies;

    private void Awake()
    {
        RagDollRigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRagDoll();
    }

    private void Start()
    {
        searchWaypoints = searchWaypoints.OrderBy(x => Random.Range(0, 100)).ToList();
        playerRef = GameManager.m_Instance.GetPlayer();

        for (int i = 0; i < transform.parent.gameObject.transform.childCount; i++)
        {
            if (transform.parent.GetChild(i).gameObject.tag == "EnemyPathPos")
            {
                PatrolWaypoints.Add(transform.parent.GetChild(i).gameObject);
            }
        }

        if (PatrolWaypoints.Count <= 0)
        {
            Debug.LogError(this.name + ": There are no waypoints");
        }
        prevPosition = transform.position;

        PatrolWaypointIndex = 0;

        fov = GetComponent<FieldOfView>();
        enemy_NavMeshAgent = GetComponent<NavMeshAgent>();
        enemy_Animator = GetComponent<Animator>();
        Health = 10f;

        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        if (Health <= 0)
        {
            enemy_Animator.enabled = false;
            enemy_NavMeshAgent.enabled = false;
            EnableRagDoll();
            startAI = false;
            StartCoroutine(DeleteAfterWaiting());
        }
        if (startAI) {
            Vector3 curMove = transform.position - prevPosition;
            curSpeed = curMove.magnitude / Time.deltaTime;
            prevPosition = transform.position;
            enemy_Animator.SetFloat("Speed", curSpeed);
            //enemy_Animator.SetFloat("MotionSpeed", curSpeed);


            switch (EnemyState)
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
                    if (PatrolWaypoints.Count > 0 && !IsHostile)
                    {
                        PatrolWaypointIndex = GoToWaypoints(PatrolWaypoints, PatrolWaypointIndex);   
                        /*float waypointDistance = Vector3.Distance(transform.position, PatrolWaypoints[PatrolWaypointIndex].transform.position);
                        enemy_NavMeshAgent.destination = PatrolWaypoints[PatrolWaypointIndex].transform.position;

                        if (waypointDistance < 1f)
                        {
                            PatrolWaypointIndex++;
                            if (PatrolWaypointIndex >= PatrolWaypoints.Count)
                            {
                                PatrolWaypointIndex = 0;
                            }
                        }*/
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
        if (!IsHostile && !isInvestigateHostile)
        {
            enemy_NavMeshAgent.speed = 2;
            if (!FindingPlayer && fov.GetCanSeePlayer())
            {
                StartCoroutine(GetPlayerPos());
            }
            if (Vector3.Distance(transform.position, playerLastSeenPos) < 1f)
            {
                 StartCoroutine(WaitBeforeDefault());
            }
        }
        else if (IsHostile)
        {
            Hostile();
        }
        else if(isInvestigateHostile)
        {
            InvestigateHostile();
        }
    }

    private void Hostile()
    {
        if (!isInvestigateHostile) {
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
                    EnemyState = EEnemyState.InvestigateHostile;
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
                if (Vector3.Distance(transform.position, alarmPos.position) < 1f)
                {
                    //EnemyManager.m_Instance.InvestigateHostileToDefault();
                    GetPlayerPosOnce = false;
                    isCoward = false;
                    IsHostile = false;
                    EnemyState = EEnemyState.InvestigateHostile;
                    InvestigateHostile();
                }
            }
        }else { InvestigateHostile(); }
    }

    private void InvestigateHostile()
    {
        isInvestigateHostile = true;
        if (!fov.GetCanSeePlayer())
        {
            if (hasInvestiagedLastPos)
            {
                SearchWaypointIndex = GoToWaypoints(searchWaypoints, SearchWaypointIndex);
            }
            else
            {
                enemy_NavMeshAgent.destination = playerLastSeenPos;
                if(Vector3.Distance(transform.position, playerLastSeenPos) < 1f) 
                {
                    hasInvestiagedLastPos = true;
                }
            }
        }
        else
        {
            isInvestigateHostile = false;
            EnemyState = EEnemyState.Hostile;
        }
    }

    private int GoToWaypoints(List<GameObject> waypoints, int index) 
    {
        float waypointDistance = Vector3.Distance(transform.position, waypoints[index].transform.position);
        enemy_NavMeshAgent.destination = waypoints[index].transform.position;

        if (waypointDistance < 1f)
        {
            index++;
            if (index >= waypoints.Count)
            {
                index = 0;
            }
        }
        return index;
    }

    IEnumerator WaitBeforeDefault() 
    {
        yield return new WaitForSeconds(2f);
        EnemyState = EEnemyState.Default;
    }

    IEnumerator GetPlayerPos()
    {
        FindingPlayer = true;
        enemy_NavMeshAgent.destination = transform.position;

        float timeToWait = 2.0f;
        while (timeToWait >= 0.0f) 
        {
            timeToWait -= Time.deltaTime;
            if (Vector3.Distance(playerRef.transform.position, transform.position) < fov.GetRadiusSuspicious()) {
                playerLastSeenPos = playerRef.transform.position;
            }
            yield return null;
        }

        if (fov.GetCanSeePlayer())
        {
            EnemyState = EEnemyState.Hostile;
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
        yield return new WaitForSeconds(4f);
        FindingPlayer = false;
        yield return null;
    }

    IEnumerator FOVRoutine()
    {
        EFOVState currentFOVState = EFOVState.Nothing;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            
            currentFOVState = fov.FieldOfViewCheck();
            
            //test out if statements :p
            switch (currentFOVState)
            {
                case EFOVState.Hostile:
                    //if (EnemyState != EEnemyState.InvestigateHostile) {
                        EnemyState = EEnemyState.Hostile;
                    //}
                    break;
                case EFOVState.Suspicious:
                    //if (EnemyState != EEnemyState.Hostile || EnemyState != EEnemyState.InvestigateHostile) { 
                        EnemyState = EEnemyState.Suspicious;
                    //}
                    break;
                case EFOVState.Nothing:
                    break;
            }
        }
    }

    IEnumerator DeleteAfterWaiting()
    {
        yield return new WaitForSeconds(10f);
        Destroy(this.gameObject);
    }

    private void DisableRagDoll() 
    {
        foreach (Rigidbody rb in RagDollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    private void EnableRagDoll()
    {
        foreach (Rigidbody rb in RagDollRigidbodies)
        {
            rb.isKinematic = false;
        }
    }

    public void SetAlarmPos(Transform pos) { alarmPos = pos; }
    public void SetSearchWaypoints(List<GameObject> waypoints) { searchWaypoints = waypoints; }
    public void SetIsCoward(bool cowardState) { isCoward = cowardState; }
    public void SetEnemyState(EEnemyState newState) { EnemyState = newState; }
    public void SetIsInvestigateHostile(bool newState) { isInvestigateHostile = newState; }
    public void SetIsHostile(bool newState) { IsHostile = newState; }
    public void SetHealth(float newHealth) { Health = newHealth; }
    public float GetHealth() { return Health; }
}

public enum EEnemyState { Default, Suspicious, InvestigateHostile, Hostile}
