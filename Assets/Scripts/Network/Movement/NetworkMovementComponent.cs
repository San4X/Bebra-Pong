using System;
using System.Collections;
using System.Collections.Generic;
using Network.Movement;
using Unity.Netcode;
using UnityEngine;

public class NetworkMovementComponent : NetworkBehaviour
{
    [SerializeField] private float speed;

    private int _tick = 0;
    private float _tickRate = 1f / 60f;
    private float _tickDeltaTime = 0f;

    private const int BUFFER_SIZE = 1024;
    private InputState[] _inputStates = new InputState[BUFFER_SIZE];
    private TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

    public NetworkVariable<TransformState> serverTransformState = new NetworkVariable<TransformState>();
    public TransformState PreviousTransformState;

    private void OnEnable()
    {
        serverTransformState.OnValueChanged += OnServerStateChanged;
    }

    private void OnServerStateChanged(TransformState previousvalue, TransformState newvalue)
    {
        PreviousTransformState = previousvalue;
    }

    public void ProcessLocalPlayerMovement(float verticalInput)
    {
        _tickDeltaTime += Time.deltaTime;
        if (_tickDeltaTime > _tickRate)
        {
            int bufferIndex = _tick % BUFFER_SIZE;

            if (!IsServer)
            {
                MovePlayerServerRpc(_tick, verticalInput);
                MovePlayer(verticalInput);
            }
            else
            {
                MovePlayer(verticalInput);

                TransformState state = new TransformState()
                {
                    Tick = _tick,
                    Position = transform.position,
                    HasStartedMoving = true
                };

                PreviousTransformState = serverTransformState.Value;
                serverTransformState.Value = state;
            }

            InputState inputState = new InputState()
            {
                Tick = _tick,
                MovementInput = verticalInput
            };
            
            TransformState transformState = new TransformState()
            {
                Tick = _tick,
                Position = transform.position,
                HasStartedMoving = true
            };

            _inputStates[bufferIndex] = inputState;
            _transformStates[bufferIndex] = transformState;

            _tickDeltaTime -= _tickRate;
            _tick++;
        }
    }

    public void ProcessSimulatedPlayerMovement()
    {
        _tickDeltaTime += Time.deltaTime;
        if (_tickDeltaTime > _tickRate)
        {
            if (serverTransformState.Value.HasStartedMoving)
            {
                transform.position = serverTransformState.Value.Position;
            }

            _tickDeltaTime -= _tickRate;
            _tick++;
        }
    }
    
    private void MovePlayer(float verticalInput)
    {
        float movementInput = verticalInput * speed * _tickRate;
        
        PlayerMovement.Instance.HandlePlayerMovement(movementInput);
    }
    
    [ServerRpc]
    private void MovePlayerServerRpc(int tick, float verticalInput)
    {
        MovePlayer(verticalInput);

        TransformState state = new TransformState()
        {
            Tick = tick,
            Position = transform.position,
            HasStartedMoving = true
        };

        PreviousTransformState = serverTransformState.Value;
        serverTransformState.Value = state;
    }
}
