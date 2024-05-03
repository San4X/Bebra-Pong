using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class Ball : MonoBehaviour
{
    public float speed = 2f;
    
    private Rigidbody2D rb;
    private Random _rnd = new Random();
    private int _ranX;
    private int _ranY;
    private Vector2 _ranDir;
    private Vector2 _tempDir;

    private Vector2 _reflectedVector = new Vector2((float)0.71,(float)-0.71);
    private Vector2 _inDirection = new Vector2(0, 0);
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //rb.gravityScale = 0f;
        // _ranX = _rnd.Next(-5, 5);
        // _ranY = _rnd.Next(-5, 5);
        // _ranDir = new Vector2(_ranX, _ranY);
        _tempDir = new Vector2(1, 1);
        rb.velocity = _tempDir * speed;
        rb.gravityScale = 0f;
        _inDirection = rb.velocity;
    }
    
    // Update is called once per frame
    void Update()
    {
        rb.velocity = rb.velocity.normalized * speed;
        Debug.Log("Velocity: " + rb.velocity);
        Debug.DrawRay(rb.position, rb.velocity, Color.red);
    }
    
    void OnCollisionEnter2D(Collision2D collision) //_inDirection initialized at the beginning because after ball collides with another object it instantly changes its trajectory and only after Vector2.Reflect compilates so it need unchangeble variable of Vector2.
    {
        Vector2 inNormal = collision.contacts[0].normal;
        Vector2 newVelocityDir = Vector2.Reflect(_inDirection , inNormal);
        Debug.Log($"Old direction = {_inDirection} to new direction {newVelocityDir}");
        rb.velocity = newVelocityDir;
        _inDirection = rb.velocity;
    }
}
