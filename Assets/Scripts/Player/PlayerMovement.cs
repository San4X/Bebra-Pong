using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    
    [SerializeField] private float speed = 5f;
    
    private NetworkVariable<float> _clientVerticalInput = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private GameObject _upperFrame, _lowerFrame;
    private float _upperBorder, _lowerBorder;
    private float _verticalInput;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _upperFrame = GameObject.FindWithTag("UpperFrame");
        _lowerFrame = GameObject.FindWithTag("LowerFrame");
        
        _upperBorder = _upperFrame.transform.position.y - _upperFrame.GetComponent<BoxCollider2D>().bounds.extents.y - GetComponent<BoxCollider2D>().bounds.extents.y;
        _lowerBorder = _lowerFrame.transform.position.y + _lowerFrame.GetComponent<BoxCollider2D>().bounds.extents.y + GetComponent<BoxCollider2D>().bounds.extents.y;
    }
    
    private void Update()
    {
        if(!IsOwner) return;
        _verticalInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        HandlePlayerMovement();
    }

    private void HandlePlayerMovement()
    {
        
        transform.position += new Vector3(0, _verticalInput * speed * Time.deltaTime, 0);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerBorder, _upperBorder), 0);
    }
}
