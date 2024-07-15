using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Ball : MonoBehaviour
{
    public float speed = 2f;
    public float reflectedAngle = 30;
    
    private Rigidbody2D _rb;
    private Vector2 _inDirection;
    private readonly Random _rnd = new Random();

    public static Vector2 BallVelocity;

    private Vector2 _tempVector = new Vector2(1, -1);
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        BallStarter();
        _rb.gravityScale = 0f;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        BallVelocity = _rb.velocity;
        _rb.velocity = BallVelocity.normalized * speed;
        
        Debug.Log("Velocity: " + BallVelocity);
        Debug.DrawRay(_rb.position, BallVelocity, Color.red);

        if (BallVelocity == new Vector2(0, 0)) _rb.velocity = _inDirection;
    }
    
    void OnCollisionEnter2D(Collision2D collision) //_inDirection initialized at the beginning (BallStarter()) because after ball collides with another object it instantly changes its trajectory and only after Vector2.Reflect compilates so it need unchangeble variable of Vector2.
    {
        Vector2 inNormal = collision.contacts[0].normal;
        Vector2 incomingVelocityDir = _inDirection;
        Vector2 reflectedVelocityDir = Vector2.Reflect(_inDirection , inNormal);
        float reflectionAngle = Vector2.Angle(incomingVelocityDir, reflectedVelocityDir);
        _inDirection = reflectedVelocityDir;
        
        _rb.velocity = _inDirection; // If not a player - stay normally reflected
        
        if (collision.gameObject.CompareTag("Player"))
        {
            AdjustAngle(reflectionAngle, collision);
        }
        
        Debug.Log($"Old direction = {_inDirection} to new direction {reflectedVelocityDir}");
    }
    
    void AdjustAngle(float reflectionAngle, Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0]; // Get the first contact point
        float top = collision.collider.bounds.max.y;
        float bot = collision.collider.bounds.min.y;
        float angleMultiplier;
        float angleAdjuster = reflectionAngle / 3f;

        if (_inDirection.y < 0) // If ball moves down
        {
            angleMultiplier = Mathf.InverseLerp( top, bot, contact.point.y);
            if(_inDirection.x < 0) angleAdjuster-= angleAdjuster*2; // If ball moves to the left
        }
        else
        {
            angleMultiplier = Mathf.InverseLerp( bot, top, contact.point.y);
            if(_inDirection.x > 0) angleAdjuster-= angleAdjuster*2;
        }
        
        // Convert the angle from degrees to radians
        float radians = (angleAdjuster * angleMultiplier) * Mathf.Deg2Rad;

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
        _rb.velocity = _inDirection;
    }
    // Визначаємо координати точки дотику
    // конвертуємо її в число від 0 до 1 де 0 це сторона з якої прилітає м'яч
    // чим більше число тим менший кут відбиття і навпаки

    public void BallStarter()
    {
        int x;
        int y = _rnd.Next(-5, 5);
        int leftOrRight = _rnd.Next(0,2);
        
        if (leftOrRight == 0) x = -9;
        else x = 9;
        
        Vector2 direction = new Vector2(x, y).normalized;
        // direction = _tempVector.normalized;
        _rb.velocity = direction * speed;
        _inDirection = _rb.velocity;
    }
}


