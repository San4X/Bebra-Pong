using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public event EventHandler OnGameStarted;
    public event EventHandler OnCountdownStarted; 
    
    [SerializeField] private GameObject pressAnyBtnUI, waitingForPlayersUI, restartButton, homeButton;

    private readonly NetworkVariable<float> _countdownToStartTimer = new (3f, writePerm: NetworkVariableWritePermission.Owner);
    private bool _anyKeyPressed, _isCountdownStarted;
    private Dictionary<ulong, bool> _playerReadyDictionary;


    private void Awake()
    {
        Instance = this;
        _playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Pause();
        
        waitingForPlayersUI.SetActive(false);
        HideGameOverBtns();
        
        _anyKeyPressed = false;
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (!_anyKeyPressed && Input.anyKeyDown)
        {
            OnAnyKeyPress();

            _anyKeyPressed = true;
        }
        
        if(_isCountdownStarted && IsServer) CountdownToStartTimer(); // Work only on Server (Multiplayer)
        else if(_isCountdownStarted && !IsClient) CountdownToStartTimer(); // Work only if not Client (Singleplaeyr)
    }

    private void OnAnyKeyPress()
    {
        pressAnyBtnUI.SetActive(false);
        if (NetworkManager.Singleton.IsClient) // If Multiplayer
        {
            waitingForPlayersUI.SetActive(true);
            
            SetPlayerReadyServerRpc();
        }
        else // If Singleplayer
        {
            StartCountdownSingleplayer();
        }
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

        if (allClientReady)
        {
            StartCountdownClientRpc();
        }
    }

    private void StartCountdownSingleplayer() // Start countdown (Singleplayer)
    {
        _isCountdownStarted = true;
        OnCountdownStarted?.Invoke(this, EventArgs.Empty);
        Resume();
    }
    
    [ClientRpc]
    private void StartCountdownClientRpc() // Start countdown for all clients (Multiplayer)
    {
        waitingForPlayersUI.SetActive(false);
        
        _isCountdownStarted = true;
        OnCountdownStarted?.Invoke(this, EventArgs.Empty);
        Resume();
    }

    private void StartGameSingleplayer() // Start game after countdown (Singleplayer)
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
        
        HideGameOverBtns();
    }
    
    [ClientRpc]
    private void StartGameClientRpc() // Start game after countdown for all clients (Multiplayer)
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
        
        HideGameOverBtns();
    }

    private void CountdownToStartTimer()
    {
        _countdownToStartTimer.Value -= Time.deltaTime;
        if (_countdownToStartTimer.Value <= 0)
        {
            _isCountdownStarted = false;
            
            if (NetworkManager.Singleton.IsClient)
            {
                StartGameClientRpc();
            }
            else StartGameSingleplayer();
        }
    }

    public float GetCountdownToStartTime()
    {
        return _countdownToStartTimer.Value;
    }
    
    private static void Pause()
    {
        Time.timeScale = 0f;
    }

    private static void Resume()
    {
        Time.timeScale = 1f;
    }
    
    public void GameOver()
    {
        Pause();
        ShowGameOverBtns();
    }

    private void ShowGameOverBtns()
    {
        restartButton.SetActive(true);
        homeButton.SetActive(true);
    }
    
    private void HideGameOverBtns()
    {
        restartButton.SetActive(false);
        homeButton.SetActive(false);
    }
}
