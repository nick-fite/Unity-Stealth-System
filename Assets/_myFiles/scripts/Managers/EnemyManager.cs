using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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

    private Vector3 LastSeenPos;

    private TextMeshProUGUI AlertNum;

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
        AlertNum = GameObject.FindGameObjectWithTag("AlertNum").GetComponent<TextMeshProUGUI>();
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
            StartCoroutine(WaitBeforeNormalPatrol());
        }
    }

    IEnumerator WaitBeforeNormalPatrol() 
    {
        WaitForNormalRunning = true;
        float timeToWait = WaitBeforePatrol;
        while (timeToWait > 0)
        {
            timeToWait -= Time.deltaTime;
            if (PlayerHasBeenSeen)
            {
                timeToWait = WaitBeforePatrol;
            }
            AlertNum.text = timeToWait.ToString();
            yield return null;
        }
        AlertNum.text = "0";
        SetEnemiesDefaultState();
        WaitForNormalRunning = false;
        yield return null;
    }

    private void SetEnemiesState(EEnemyState newState, bool isHostile, bool isInvestigateHostile, bool isCoward) 
    {   
        foreach (GameObject obj in enemyGameobj)
        {
            obj.GetComponentInChildren<EnemyAIScript>().SetEnemyState(newState);
            obj.GetComponentInChildren<EnemyAIScript>().SetIsHostile(isHostile);
            obj.GetComponentInChildren<EnemyAIScript>().SetIsInvestigateHostile(isInvestigateHostile);
            obj.GetComponentInChildren<EnemyAIScript>().SetIsCoward(isCoward);
            foreach (EnemyInfo info in Enemies) {
                if (info.Enemy == obj)
                {
                    obj.GetComponentInChildren<EnemyAIScript>().SetIsCoward(info.isCoward);
                }
            }
        }
    }

    private void SetEnemiesDefaultState() 
    {
        foreach (GameObject obj in enemyGameobj)
        {
            GameObject enemyObj = obj.GetComponentInChildren<EnemyAIScript>().gameObject;
            EnemyAIScript AI;
            if (enemyObj.TryGetComponent(out AI))
            {
                AI.SetEnemyState(EEnemyState.Default);
                AI.SetIsHostile(false);
                AI.SetIsInvestigateHostile(false);
                foreach (EnemyInfo info in Enemies)
                {
                    if (info.Enemy == obj)
                    {
                        AI.SetIsCoward(info.isCoward);
                    }
                }
            }
        }
    }

    public void AlertAllEnemies() 
    {
        SetEnemiesState(EEnemyState.InvestigateHostile, true, true, false);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemyGameobj.Contains(enemy)) {
            enemyGameobj.Remove(enemy);
        }
    }

    public void SetPlayerHasBeenSeen(bool newState) { PlayerHasBeenSeen = newState; }
    public void SetLastSeenPos(Vector3 newPos) { LastSeenPos = newPos; }
    public Vector3 GetLastSeenPos() { return LastSeenPos; }

}
