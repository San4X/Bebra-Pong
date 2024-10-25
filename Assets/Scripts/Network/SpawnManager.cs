using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] private Transform playerPrefab, ballPrefab;
    [SerializeField] private GameObject aiPrefab;

    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (GameModeManager.IsSolo == true) SetupAIScene();
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        ulong clientische;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            bool isServer = clientId == NetworkManager.Singleton.LocalClientId;
            
            Transform playerTransform = isServer
                ? Instantiate(playerPrefab, player1SpawnPoint.position, playerPrefab.rotation)
                : Instantiate(playerPrefab, player2SpawnPoint.position, playerPrefab.rotation);
            
            NetworkObject networkObject = playerTransform.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(clientId, true);
        }

        Transform ballTransform = Instantiate(ballPrefab);
        NetworkObject ballNetworkObject = ballTransform.GetComponent<NetworkObject>();
        ballNetworkObject.Spawn(true);
    }
    
    void SetupAIScene()
    {
        Instantiate(playerPrefab, new Vector3(-8f, 0, 0), playerPrefab.transform.rotation);
        Instantiate(aiPrefab, new Vector3(8f, 0, 0), aiPrefab.transform.rotation);
    }
    
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
