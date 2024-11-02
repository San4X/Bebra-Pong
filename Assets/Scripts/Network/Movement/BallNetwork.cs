using System;
using Unity.Netcode;
using UnityEngine;

namespace Network.Movement
{
    public class BallNetwork : NetworkBehaviour
    {
        private readonly NetworkVariable<Vector2> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (IsOwner)
            {
                _netState.Value = _rb.velocity;
            }
            else
            {
                _rb.velocity = _netState.Value;
            }
        }
    }
}
