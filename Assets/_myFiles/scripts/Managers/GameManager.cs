using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager m_Instance;

    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private Transform PlayerSpawn;
    private GameObject Chest;
    private GameObject Player;

    private void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Debug.LogError("Multiple GameManagers found. Deleting Copy...");
            Destroy(this);
        }
        else
        {
            m_Instance = this;
        }

        //spawning player
        if (PlayerPrefab && PlayerSpawn)
        {
            Player = Instantiate(PlayerPrefab, PlayerSpawn.transform.position, PlayerSpawn.transform.rotation);
        }
        else
        {
            Debug.LogWarning("PlayerPrefab or PlayerSpawn not referenced");
        }

        Chest = GameObject.FindGameObjectWithTag("PlayerChest");
    }

    public GameObject GetPlayer() { return Player.GetComponentInChildren<CharacterController>().gameObject; }
    public GameObject GetChest() { return Chest; }
}
