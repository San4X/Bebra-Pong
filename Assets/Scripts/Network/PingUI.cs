using System.Linq;
using TMPro;
using Unity.Netcode;

namespace Network
{
    public class PingUI : NetworkBehaviour
    {
        private ulong _ping;
        private ulong _otherClientId;
        private ulong _serverClientId;
        private TextMeshProUGUI _text;
        
        private void Start()
        {
            if(!IsClient) Destroy(this);
            _serverClientId = NetworkManager.ServerClientId;
            _otherClientId = NetworkManager.Singleton.ConnectedClientsIds.First(id => id != _serverClientId);
            _text = GetComponent<TextMeshProUGUI>();
            InvokeRepeating(nameof(UpdatePing), 0.5f, 0.5f);
        }
        
        private void UpdatePing()
        {
            _ping = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(!IsServer ? _serverClientId : _otherClientId);
            _ping /= 2; // i need time only in one direction
            _text.text = $"{_ping} ms";
        }
    }
}
