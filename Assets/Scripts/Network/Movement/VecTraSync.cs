using Unity.Netcode;
using UnityEngine;

namespace Network.Movement
{
    public class Vector2Sync : NetworkBehaviour
    {
        [SerializeField] private float interpolationTime = 0.1f;
        
        private readonly NetworkVariable<Vector2> _netVector = new(writePerm: NetworkVariableWritePermission.Owner);
        private readonly NetworkVariable<float> _netTransformY = new(writePerm: NetworkVariableWritePermission.Owner);
        
        private Rigidbody2D _rb;
        private Vector3 _vel;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (IsOwner)
            {
                _netVector.Value = _rb.velocity;
                _netTransformY.Value = transform.position.y;
            }
            else
            {
                _rb.velocity = _netVector.Value;
                var vector3 = transform.position;
                vector3.y = _netTransformY.Value;
                transform.position = Vector3.SmoothDamp(transform.position, vector3, ref _vel, interpolationTime);
            }
        }
    }
}
