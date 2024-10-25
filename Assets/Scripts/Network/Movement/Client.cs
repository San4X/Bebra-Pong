using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network.Movement
{
    public struct InputPayload
    {
        public int Tick;
        public float VerticalInput;
    }

    public struct StatePayload
    {
        public int Tick;
        public Vector3 Position;
    }
    public class Client : MonoBehaviour
    {
        public static Client Instance;

        // shared
        private float _timer;
        private int _currentTick;
        private float _minTimeBetweenTicks;
        private const float SERVER_TICK_RATE = 30f;
        private const int BUFFER_SIZE = 1024;

        // for client
        private StatePayload[] _stateBuffer;
        private InputPayload[] _inputBuffer;
        private StatePayload _latestServerState;
        private StatePayload _lastProcessedState;
        private float _verticalInput;
        // for server
        private Queue<InputPayload> _inputQueue;
        private StatePayload[] _stateBufferServer;
        
        private void Start()
        {
            _minTimeBetweenTicks = 1f / SERVER_TICK_RATE;

            _stateBuffer = new StatePayload[BUFFER_SIZE];
            _inputBuffer = new InputPayload[BUFFER_SIZE];
            
            _inputQueue = new Queue<InputPayload>(BUFFER_SIZE);
            _stateBufferServer = new StatePayload[BUFFER_SIZE];
        }

        private void Update()
        {
            _verticalInput = Input.GetAxis("Vertical");

            _timer += Time.deltaTime;

            while (_timer >= _minTimeBetweenTicks)
            {
                _timer -= _minTimeBetweenTicks;
                HandleTick();
                _currentTick++;
            }
        }

        void HandleTick()
        {
            if (!_latestServerState.Equals(default(StatePayload)) &&
                (_lastProcessedState.Equals(default(StatePayload)) ||
                 !_latestServerState.Equals(_lastProcessedState)))
            {
                HandleServerReconciliation();
            }
            
            int bufferIndex = _currentTick % BUFFER_SIZE;

            // add payload to inputBuffer
            InputPayload inputPayload = new InputPayload();
            inputPayload.Tick = _currentTick;
            inputPayload.VerticalInput = _verticalInput;
            _inputBuffer[bufferIndex] = inputPayload;

            // add payload to stateBuffer
            _stateBuffer[bufferIndex] = ProcessMovement(inputPayload);

            SendToServerRpc(inputPayload);
        }

        [ServerRpc]
        void SendToServerRpc(InputPayload inputPayload)
        {
            _inputQueue.Enqueue(inputPayload);
        }

        [ClientRpc]
        void SendToClientRpc(StatePayload serverState)
        {
            _latestServerState = serverState;
        }
        
        [ServerRpc]
        void HandleTickServerRpc()
        {
            int bufferIndex = -1;
            while (_inputQueue.Count > 0)
            {
                InputPayload inputPayload = _inputQueue.Dequeue();

                bufferIndex = inputPayload.Tick % BUFFER_SIZE;

                StatePayload statePayload = ProcessMovement(inputPayload);
                _stateBufferServer[bufferIndex] = statePayload;
            }

            if (bufferIndex != -1)
            {
                SendToClientRpc(_stateBuffer[bufferIndex]);
            }
        }

        StatePayload ProcessMovement(InputPayload input)
        {
            transform.position += new Vector3(transform.position.x, input.VerticalInput * 5f * _minTimeBetweenTicks, 0);

            return new StatePayload()
            {
                Tick = input.Tick,
                Position = transform.position
            };
        }

        void HandleServerReconciliation()
        {
            _lastProcessedState = _latestServerState;

            int serverStateBufferIndex = _latestServerState.Tick % BUFFER_SIZE;
            float positionError =
                Vector3.Distance(_latestServerState.Position, _stateBuffer[serverStateBufferIndex].Position);

            if (positionError > 0.001f)
            {
                Debug.Log("Reconcile!");
                transform.position = _latestServerState.Position;

                _stateBuffer[serverStateBufferIndex] = _latestServerState;

                int tickToProcess = _latestServerState.Tick + 1;

                while (tickToProcess < _currentTick)
                {
                    int bufferIndex = tickToProcess % BUFFER_SIZE;
                    
                    StatePayload statePayload = ProcessMovement(_inputBuffer[tickToProcess]);
                    
                    _stateBuffer[bufferIndex] = statePayload;

                    tickToProcess++;
                }
                
            }
        }
    }
}