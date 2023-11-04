using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Serializable]
    private struct EnemyInfo
    {
        public GameObject Enemy;
        public GameObject EnemyPos;
        public bool isCoward;
    }

    [Header("Enemy Info")]
    [SerializeField] List<EnemyInfo> Enemies;

    [SerializeField] private List<GameObject> enemyGameobj;

    [SerializeField] private GameObject alarmButton;

    [SerializeField]private List<GameObject> searchWaypoints;


    public static EnemyManager m_Instance;

    [SerializeField] float WaitBeforePatrol;

    private bool PlayerHasBeenSeen;
    private bool WaitForNormalRunning;

    private void Awake()
    {
        WaitForNormalRunning = false;
        if (m_Instance != null && m_Instance != this)
        {
            Debug.LogError("Multiple GameManagers found. Deleting Copy...");
            Destroy(this);
        }
        else
        {
            m_Instance = this;
        }

    }

    private void Start()
    {
        if (alarmButton == null) { alarmButton = GameObject.FindGameObjectWithTag("AlarmButton"); }
        if (searchWaypoints.Count < 1) { searchWaypoints = GameObject.FindGameObjectsWithTag("SearchWaypoint").ToList(); }

        for (int i = 0; i < Enemies.Count; i++) 
        {
            GameObject enemy = Instantiate(Enemies[i].Enemy, Enemies[i].EnemyPos.transform.position, Enemies[i].EnemyPos.transform.rotation);

            EnemyAIScript AI;
            if (enemy.GetComponentInChildren<EnemyAIScript>().gameObject.TryGetComponent(out AI))
            {
                AI.SetAlarmPos(alarmButton.transform);
                AI.SetSearchWaypoints(searchWaypoints);
                AI.SetIsCoward(Enemies[i].isCoward);
            }

            enemyGameobj.Add(enemy);
        }
    }

    public void InvestigateHostileToDefault()
    {
        if (!WaitForNormalRunning) {
            Debug.Log("running");
            StartCoroutine(WaitBeforeNormalPatrol());
        }
    }

    IEnumerator WaitBeforeNormalPatrol() 
    {
        Debug.Log("runnin");
        WaitForNormalRunning = true;
        float timeToWait = WaitBeforePatrol;
        while (timeToWait > 0)
        {
            timeToWait -= Time.deltaTime;
            if (PlayerHasBeenSeen)
            {
                timeToWait = WaitBeforePatrol;
            }
            SetEnemiesState(EEnemyState.Default, false, false);
            yield return new WaitForSeconds(0.1f);
        }
        WaitForNormalRunning = false;
    }

    private void SetEnemiesState(EEnemyState newState, bool isHostile, bool isInvestigateHostile) 
    {
        
        foreach (GameObject obj in enemyGameobj)
        {
            obj.GetComponentInChildren<EnemyAIScript>().SetEnemyState(newState);
            obj.GetComponentInChildren<EnemyAIScript>().SetIsHostile(isHostile);
            obj.GetComponentInChildren<EnemyAIScript>().SetIsInvestigateHostile(isInvestigateHostile);
        }
    }

    public void SetPlayerHasBeenSeen(bool newState) { PlayerHasBeenSeen = newState; }
}
