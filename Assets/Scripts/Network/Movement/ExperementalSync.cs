using System;
using Unity.Netcode;
using UnityEngine;

namespace Network.Movement
{
    public class ExperementalSync : NetworkBehaviour
    {
        private readonly NetworkVariable<Vector2> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
        private Rigidbody2D _rb;
        private ulong _ping;
        private float _currentOverestimation;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            InvokeRepeating(nameof(UpdateRTT), 0.5f, 0.5f);
            BallMovement.Instance.OnBallStarted += OnBallStarted_Event;
        }
        
        private void UpdateRTT()
        {
            if (IsServer) return;
            _ping = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.ServerClientId);
        }
        
        void Update()
        {
            if (IsOwner)
            {
                _netState.Value = _rb.velocity;
            }
            else
            {
                _currentOverestimation = _ping * 2f;
                _rb.velocity = _netState.Value;
            }
        }

        private void OnBallStarted_Event(object sender, EventArgs e)
        {
            ApplyOverestimationClientRpc();
        }
        
        [ClientRpc]
        private void ApplyOverestimationClientRpc()
        {
            if (IsServer) return;
            _rb.AddForce(_netState.Value * new Vector2(_currentOverestimation, _currentOverestimation));
            Debug.Log(_ping + " " + _currentOverestimation + " " + _netState.Value);
        }
    }
}
