using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public event EventHandler OnGameStarted;
    public event EventHandler OnCountdownStarted;

    [SerializeField] private GameObject pressAnyBtnUI, waitingForPlayersUI;
    [SerializeField] private Button restartButton, homeButton;
    [SerializeField] private TextMeshProUGUI readyForNextRoundLeftText, readyForNextRoundRightText;

    private readonly NetworkVariable<float> _countdownToStartTimer = new (3f, writePerm: NetworkVariableWritePermission.Owner);
    private bool _anyKeyPressed, _isCountdownStarted;
    private Dictionary<ulong, bool> _playerReadyDictionary;


    private void Awake()
    {
        Instance = this;
        _playerReadyDictionary = new Dictionary<ulong, bool>();
        _anyKeyPressed = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        waitingForPlayersUI.SetActive(false);
        HideGameOverUI();
        
        Pause();

        restartButton.onClick.AddListener(() =>
        {
            SetReadyTextServerRpc();
            SetPlayerReadyServerRpc();
        });
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
            if (_playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId]) continue;
            allClientReady = false;
            break;
        }

        if (!allClientReady) return;
        StartCountdownClientRpc();
        SetPlayersUnready();
    }

    private void SetPlayersUnready()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (_playerReadyDictionary.ContainsKey(clientId))
            {
                _playerReadyDictionary[clientId] = false;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyTextServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetReadyTextClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SetReadyTextClientRpc(ulong clientId)
    {
        
        if (clientId == 0) readyForNextRoundLeftText.gameObject.SetActive(true);
        else readyForNextRoundRightText.gameObject.SetActive(true);
    }

    private void StartCountdownSingleplayer() // Start countdown (Singleplayer)
    {
        OnCountdownStarted?.Invoke(this, EventArgs.Empty);
        
        Resume();
        HideGameOverUI();
        
        _isCountdownStarted = true;
    }
    
    [ClientRpc]
    private void StartCountdownClientRpc() // Start countdown for all clients (Multiplayer)
    {
        OnCountdownStarted?.Invoke(this, EventArgs.Empty);
        
        Resume();
        waitingForPlayersUI.SetActive(false);
        HideGameOverUI();
        
        _isCountdownStarted = true;
    }

    private void StartGameSingleplayer() // Start game after countdown (Singleplayer)
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }
    
    [ClientRpc]
    private void StartGameClientRpc() // Start game after countdown for all clients (Multiplayer)
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    private void CountdownToStartTimer()
    {
        _countdownToStartTimer.Value -= Time.deltaTime;
        if (_countdownToStartTimer.Value <= 0)
        {
            _isCountdownStarted = false;
            _countdownToStartTimer.Value = 3f;
            
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
        ShowGameOverUI();
    }

    private void ShowGameOverUI()
    {
        restartButton.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(true);
    }
    
    private void HideGameOverUI()
    {
        restartButton.gameObject.SetActive(false);
        homeButton.gameObject.SetActive(false);
        readyForNextRoundLeftText.gameObject.SetActive(false);
        readyForNextRoundRightText.gameObject.SetActive(false);
        
    }
}
