using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    //[SerializeField] private NetworkMovementComponent networkMovementComponent;
    [SerializeField] private float speed = 5f;
    
    private GameObject _upperFrame, _lowerFrame;
    private float _upperBorder, _lowerBorder;
    
    private float _timer;
    private float _minTimeBetweenTicks;
    private const float TICK_RATE = 60f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _minTimeBetweenTicks = 1f / TICK_RATE;
        
        _upperFrame = GameObject.FindWithTag("UpperFrame");
        _lowerFrame = GameObject.FindWithTag("LowerFrame");
        
        _upperBorder = _upperFrame.transform.position.y - _upperFrame.GetComponent<BoxCollider2D>().bounds.extents.y - GetComponent<BoxCollider2D>().bounds.extents.y;
        _lowerBorder = _lowerFrame.transform.position.y + _lowerFrame.GetComponent<BoxCollider2D>().bounds.extents.y + GetComponent<BoxCollider2D>().bounds.extents.y;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsClient && !IsOwner) return;
        float verticalInput = Input.GetAxis("Vertical");

        //networkMovementComponent.ProcessLocalPlayerMovement(verticalInput);
        // if (IsClient && IsLocalPlayer)
        // {
        //     networkMovementComponent.ProcessLocalPlayerMovement(verticalInput);
        // }
        // else
        // {
        //     networkMovementComponent.ProcessSimulatedPlayerMovement();
        // }
        
        // _timer += Time.deltaTime;
        // while (_timer >= _minTimeBetweenTicks)
        // {
        //     _timer -= _minTimeBetweenTicks;
        //     HandleMovementServerAuth(verticalInput);
        // }
        HandlePlayerMovement(verticalInput);
    }
    
    private void HandleMovementServerAuth(float verticalInput)
    {
        HandleMovementServerRpc(verticalInput);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void HandleMovementServerRpc(float inputVector)
    {
        // transform.Translate(new Vector3(0, inputVector, 0) * (speed * Time.deltaTime));
        transform.position += new Vector3(0, inputVector, 0) * (speed * _minTimeBetweenTicks);
    
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerBorder, _upperBorder), 0);
    }

    public void HandlePlayerMovement(float movementInput)
    {
        transform.position += new Vector3(0, movementInput * speed * Time.deltaTime, 0);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerBorder, _upperBorder), 0);
    }
}
