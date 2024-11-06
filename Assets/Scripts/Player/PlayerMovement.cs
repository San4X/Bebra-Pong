using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    
    [SerializeField] private float speed = 2f;
    
    private GameObject _upperFrame, _lowerFrame;
    private float _upperBorder, _lowerBorder;
    private float _verticalInput;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        _upperFrame = GameObject.FindWithTag("UpperFrame");
        _lowerFrame = GameObject.FindWithTag("LowerFrame");
        
        _upperBorder = _upperFrame.transform.position.y - _upperFrame.GetComponent<BoxCollider2D>().bounds.extents.y - GetComponent<BoxCollider2D>().bounds.extents.y;
        _lowerBorder = _lowerFrame.transform.position.y + _lowerFrame.GetComponent<BoxCollider2D>().bounds.extents.y + GetComponent<BoxCollider2D>().bounds.extents.y;
    }
    
    private void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerBorder, _upperBorder), 0);
        if(!IsOwner) return;
        if (!Input.anyKeyDown)
        {
            _moveDirection = Vector2.zero;
        }
        if (Input.GetKey(KeyCode.W))
        {
            _moveDirection = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _moveDirection = Vector2.down;
        }
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        HandlePlayerMovement();
    }

    private void HandlePlayerMovement()
    {
        _rb.velocity = _moveDirection * speed;
    }
}
