using System;
using System.Collections.Generic;
using Menu.Network;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Menu.Network
{
    public class LobbyManager : MonoBehaviour
    {
        
        public static LobbyManager Instance { get; private set; }
        
        public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
        public event EventHandler<LobbyEventArgs> OnJoinedLobby;
        public event EventHandler OnKickedFromLobby; 
        public event EventHandler OnLeftLobby;
        //public event EventHandler OnGameStarted;
        
        public class LobbyEventArgs : EventArgs {
            public Lobby lobby;
        }
        
        [SerializeField] private TMP_InputField lobbyName;
        [SerializeField] private TMP_InputField updatedLobbyName;
        [SerializeField] private TextMeshProUGUI lobbyAccessibilityBtnText;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private GameObject lobbyPrefabListBtn;
        [SerializeField] private Transform lobbyListContent;
        [SerializeField] private TextMeshProUGUI playerName;
        
        private Lobby _joinedLobby;
        private float _heartbeatTimer;
        private float _lobbyUpdateTimer;
        private bool _isLobbyPrivate;
        
        public const string KEY_PLAYER_NAME = "PlayerName";
        public const string KEY_START_GAME = "StartGame_RelayCode";


        private void Awake()
        {
            Instance = this;
        }

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _isLobbyPrivate = true;
        }

        void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPolling();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (IsLobbyHost())
            {
                _heartbeatTimer -= Time.deltaTime;
                if (_heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15f;
                    _heartbeatTimer = heartbeatTimerMax;

                    await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
                }
            }
        }

        private async void HandleLobbyPolling()
        {
            if (_joinedLobby == null) return;
            _lobbyUpdateTimer -= Time.deltaTime;
            if (_lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 2f;
                _lobbyUpdateTimer = lobbyUpdateTimerMax;

                _joinedLobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                
                if (!IsPlayerInLobby()) {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, EventArgs.Empty);

                    _joinedLobby = null;
                    return;
                }
                    
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs{lobby = _joinedLobby});

                if (_joinedLobby.Data[KEY_START_GAME].Value != "0")
                {
                    if (!IsLobbyHost())
                    {
                        JoinOnline();
                    }
                }
            }
        }

        public async void CreateLobby()
        {
            int maxPlayers = 2;

            try
            {
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = _isLobbyPrivate,
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                    {
                        { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                    }
                };

                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName.text, maxPlayers, createLobbyOptions);

                OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
                Debug.Log("Created lobby: " + _joinedLobby.Name + " " + _joinedLobby.MaxPlayers + " " + _joinedLobby.LobbyCode);
                
                CreateOnline();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public void LobbyAccessibilityBtnHandler() // Make lobby private or public when creating
        {
            _isLobbyPrivate = !_isLobbyPrivate;
            lobbyAccessibilityBtnText.text = _isLobbyPrivate ? "private" : "public";
        }

        public async void ListLobbies()
        {
            try
            {
                // Delete list before creating new
                foreach (Transform child in lobbyListContent)
                {
                    Destroy(child.gameObject);
                }

                // Set filter
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
                {
                    Count = 20,
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    },
                    Order = new List<QueryOrder>
                    {
                        new QueryOrder(true, QueryOrder.FieldOptions.Created)
                    }
                };

                // Getting filtered list
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

                Debug.Log("Lobbies found: " + queryResponse.Results.Count);
                foreach (var lobby in queryResponse.Results)
                {
                    Debug.Log(lobby.Name + " " + (lobby.MaxPlayers - lobby.AvailableSlots) + "/" + lobby.MaxPlayers);

                    // Instantiating list
                    GameObject newLobbyListItem = Instantiate(lobbyPrefabListBtn, lobbyListContent);
                    newLobbyListItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                    newLobbyListItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                        lobby.MaxPlayers - lobby.AvailableSlots + "/" + lobby.MaxPlayers;

                    newLobbyListItem.GetComponent<Button>().onClick.AddListener(() => JoinLobbyByClick(lobby));
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void JoinLobbyByCode()
        {
            try
            {
                var joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
                };

                _joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text, joinLobbyByCodeOptions);

                LobbyUI.Instance.CreateLobbyUI(_joinedLobby);
                Debug.Log("Joined lobby by code: " + lobbyCodeInput.text);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void JoinLobbyByClick(Lobby clickedLobby) // Mb merge into JoinLobbyByCode?
        {
            try
            {
                var joinLobbyByIdOptions = new JoinLobbyByIdOptions
                {
                    Player = GetPlayer()
                };
                
                Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(clickedLobby.Id, joinLobbyByIdOptions);
                _joinedLobby = lobby;

                LobbyUI.Instance.CreateLobbyUI(lobby);
                Debug.Log("Joined lobby by code: " + lobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public Lobby GetJoinedLobby()
        {
            return _joinedLobby;
        }

        public async void UpdateLobbyName()
        {
            try
            {
                _joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Name = updatedLobbyName.text
                });

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void UpdatePlayerName()
        {
            try
            {
                await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId,
                    new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            {
                                "PlayerName",
                                new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName.text)
                            }
                        }
                    });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                
                OnLeftLobby?.Invoke(this, EventArgs.Empty);

                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
        public async void KickPlayerFromLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, _joinedLobby.Players[1].Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName.text) }
                }
            };
        }
        
        public bool IsLobbyHost() {
            return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }
        
        private bool IsPlayerInLobby() {
            if (_joinedLobby != null && _joinedLobby.Players != null) {
                foreach (Player player in _joinedLobby.Players) {
                    if (player.Id == AuthenticationService.Instance.PlayerId) {
                        // This player is in this lobby
                        Debug.Log("Player in lobby");
                        return true;
                    }
                }
            }
            return false;
        }

        private async void CreateOnline()
        {
            if (IsLobbyHost())
            {
                try
                {
                    string relayCode = await Relay.Instance.CreateRelay();

                    await Lobbies.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                        }
                    });
                    
                    ModeSelectionUI.Instance.SetModeIconNameToHost();
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        private async void JoinOnline()
        {
            if (!IsLobbyHost() && !NetworkManager.Singleton.IsClient)
            {
                await Relay.Instance.JoinRelay(_joinedLobby.Data[KEY_START_GAME].Value);
                
                ModeSelectionUI.Instance.SetModeIconNameToClient();
            }
        }
    }
}

