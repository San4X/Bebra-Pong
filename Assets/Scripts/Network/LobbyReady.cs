using System;
using System.Collections;
using System.Collections.Generic;
using Menu.Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyReady : NetworkBehaviour
{
    [SerializeField] private Button readyBtn;
    
    private Dictionary<ulong, bool> _playerReadyDictionary;

    private void Awake()
    {
        _playerReadyDictionary = new Dictionary<ulong, bool>();
        readyBtn.onClick.AddListener(() =>
        {
            SetPlayerReadyServerRpc();
        });
    }

    private void Start()
    {
        LobbyManager.Instance.OnKickedFromLobby += PlayerNotReady_Event;
        LobbyManager.Instance.OnLeftLobby += PlayerNotReady_Event;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        
        bool allClientReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
            {
                allClientReady = false;
                break;
            }
        }
        if(allClientReady) MenuManager.Instance.LoadGameScene();
    }

    private void PlayerNotReady_Event(object sender, EventArgs e)
    {
        PlayerReadyLeftServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyLeftServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Player " + serverRpcParams.Receive.SenderClientId + " is NOT ready!");
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = false;
    }
}
