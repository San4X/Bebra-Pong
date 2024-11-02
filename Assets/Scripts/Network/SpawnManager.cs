using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class SpawnManager : NetworkBehaviour
    {
        public static SpawnManager Instance { get; private set; }

        [SerializeField] private Transform playerPrefab, ballPrefab;
        [SerializeField] private GameObject aiPrefab;
        [SerializeField] private Transform player1SpawnPoint;
        [SerializeField] private Transform player2SpawnPoint;
    

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
    
        private void SetupAIScene()
        {
            Instantiate(playerPrefab, new Vector3(-8f, 0, 0), playerPrefab.transform.rotation);
            Instantiate(aiPrefab, new Vector3(8f, 0, 0), aiPrefab.transform.rotation);
        }
    
        public void LoadMenuScene()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
