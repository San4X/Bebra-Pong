using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Network.Movement
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private float interpolationTime = 0.1f;
         
        private readonly NetworkVariable<float> _netPosY = new(writePerm: NetworkVariableWritePermission.Owner);
        private Vector3 _vel;
        
        void Update()
        {
            if (IsOwner)
            {
                _netPosY.Value = transform.position.y;
            }
            else
            {
                var vector3 = transform.position;
                vector3.y = _netPosY.Value;
                transform.position = Vector3.SmoothDamp(transform.position, vector3, ref _vel, interpolationTime);
            }
        }
    }
}
