using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Ball : MonoBehaviour
{
    public float speed = 2f;
    public float reflectedAngle = 30;
    
    private Rigidbody2D rb;
    private Random _rnd = new Random();
    
    
    
    private Vector2 _inDirection = new Vector2(0, 0);
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        BallStarter();
        rb.gravityScale = 0f;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        rb.velocity = rb.velocity.normalized * speed;
        Debug.Log("Velocity: " + rb.velocity);
        Debug.DrawRay(rb.position, rb.velocity, Color.red);
    }
    
    void OnCollisionEnter2D(Collision2D collision) //_inDirection initialized at the beginning (BallStarter()) because after ball collides with another object it instantly changes its trajectory and only after Vector2.Reflect compilates so it need unchangeble variable of Vector2.
    {
        Vector2 inNormal = collision.contacts[0].normal;
        Vector2 reflectedVelocityDir = Vector2.Reflect(_inDirection , inNormal);
        _inDirection = reflectedVelocityDir;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            AdjustTrajectory(reflectedAngle, collision);
        }
        
        Debug.Log($"Old direction = {_inDirection} to new direction {reflectedVelocityDir}");
        //rb.velocity = newVelocityDir;
        
        rb.velocity = _inDirection;
        //_inDirection = rb.velocity;
    }
    
    void AdjustTrajectory(float angle, Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0]; // Get the first contact point
        float top = collision.collider.bounds.max.y;
        float bot = collision.collider.bounds.min.y;
        float angleMultiplier = 0;

        if (_inDirection.y < 0)
        {
            angleMultiplier = Mathf.InverseLerp( top, bot, contact.point.y);
            if(_inDirection.x < 0) angle-= angle*2;
        }
        else
        {
            angleMultiplier = Mathf.InverseLerp( bot, top, contact.point.y);
            if(_inDirection.x > 0) angle-= angle*2;
        }
        
        // Convert the angle from degrees to radians
        float radians = (angle * angleMultiplier) * Mathf.Deg2Rad;

        // Calculate the sine and cosine of the angle
        float cosAngle = Mathf.Cos(radians);
        float sinAngle = Mathf.Sin(radians);

        // Perform a 2D rotation of the velocity vector by the specified angle
        float newX = _inDirection.x * cosAngle - _inDirection.y * sinAngle;
        float newY = _inDirection.x * sinAngle + _inDirection.y * cosAngle;

        // Update the velocity vector with the adjusted trajectory
        _inDirection.x = newX;
        _inDirection.y = newY;

        // Normalize the adjusted velocity vector to maintain its direction
        _inDirection.Normalize();
    }

    public void BallStarter()
    {
        int x, y;
        Vector2 direction;
        
        y = _rnd.Next(-5, 5);
        int leftOrRight = _rnd.Next(0,2);
        if (leftOrRight == 0) x = -9;
        else x = 9;
        
        direction = new Vector2(x, y).normalized;
        rb.velocity = direction * speed;
        _inDirection = rb.velocity;
    }
}
// Визначаємо координати точки дотику
// конвертуємо її в число від 0 до 1 де 0 це сторона з якої прилітає м'яч
// чим більше число тим менший кут відбиття і навпаки

