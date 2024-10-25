using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Network
{
    public class LobbyUI : MonoBehaviour
    {
        public static LobbyUI Instance { get; private set; }
        
        [SerializeField] private GameObject hostPlayer, joinedPlayer;
        [SerializeField] private TextMeshProUGUI lobbyName, lobbyCode;
        [SerializeField] private Button closeBtn, leaveLobbyBtn, copyLobbyCodeBtn, kickPlayerBtn;
        
        private void Awake()
        {
            Instance = this;

            leaveLobbyBtn.onClick.AddListener(() =>
            {
                LobbyManager.Instance.LeaveLobby();
            });
            kickPlayerBtn.onClick.AddListener(() =>
            {
                LobbyManager.Instance.KickPlayerFromLobby();
            });
        }

        private void Start()
        {
            LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
            LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
            LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
            LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;
            
            Hide();
        }

        private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
        {
            CreateLobbyUI(e.lobby);
        }
        
        private void LobbyManager_OnLeftLobby(object sender, EventArgs e) {
            Hide();
        }

        public void CreateLobbyUI(Lobby lobby)
        {
            gameObject.SetActive(true);
            joinedPlayer.SetActive(false);

            lobbyName.text = lobby.Name;
            lobbyCode.text = lobby.LobbyCode;
            
            hostPlayer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                lobby.Players[0].Data["PlayerName"].Value;
            if (lobby.Players.Count == 2)
            {
                joinedPlayer.SetActive(true);
                joinedPlayer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    lobby.Players[1].Data["PlayerName"].Value;
                kickPlayerBtn.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());
            }
            
            copyLobbyCodeBtn.onClick.AddListener(CopyLobbyCodeToClipboard);
            
            Debug.Log("Lobby created!");
        }

        private void CopyLobbyCodeToClipboard()
        {
            GUIUtility.systemCopyBuffer = lobbyCode.text;
        }
        
        private void Hide() {
            gameObject.SetActive(false);
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
            LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
            LobbyManager.Instance.OnLeftLobby -= LobbyManager_OnLeftLobby;
            LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnLeftLobby;
        }
    }
}
