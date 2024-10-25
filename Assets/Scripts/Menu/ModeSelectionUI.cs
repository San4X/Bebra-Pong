using System;
using Menu.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ModeSelectionUI : MonoBehaviour
{
    public static ModeSelectionUI Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI menuIconModeName;
    [SerializeField] private TMP_InputField createLobbyName;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start() {
        LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;

        createLobbyName.text = "Some sort of lobby" + Random.Range(123, 321);
    }
    
    private void LobbyManager_OnKickedFromLobby(object sender, EventArgs e) {
        Show();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e) {
        Show();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e) {
        Hide();
    }
    
    public void SetGameModeToAI()
    {
        GameModeManager.IsSolo = true;
        menuIconModeName.text = "Solo";
    }
    
    public void SetModeIconNameToHost()
    {
        GameModeManager.IsSolo = false;
        menuIconModeName.text = "Host";
        NetworkManager.Singleton.StartHost();
    }
    
    public void SetModeIconNameToClient()
    {
        GameModeManager.IsSolo = false;
        menuIconModeName.text = "Client";
        NetworkManager.Singleton.StartClient();
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}
