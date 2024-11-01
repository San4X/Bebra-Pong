using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    public static Ball Instance { get; private set; }

    public event EventHandler<CollisionEventArgs> OnCollidedWithGoal;

    public class CollisionEventArgs : EventArgs
    {
        public Collision2D collision;
    }
    
    
    [SerializeField] private float speed = 2f;
    
    private Rigidbody2D _rb;
    private Vector2 _inDirection;

    public Vector2 ballVelocity;
    

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;

        if(Scoring.Instance != null) Scoring.Instance.StartDaBall += StartDaBall_Event;
        else BallStarter(); // If it is menu
    }
    
    // Update is called once per frame
    void Update()
    {
        ballVelocity = _rb.velocity;
        _rb.velocity = ballVelocity.normalized * speed;
    
        if (ballVelocity == new Vector2(0, 0) && transform.position != new Vector3(0, 0, 0)) BallStarter();
        
        Debug.DrawRay(_rb.position, ballVelocity, Color.red);
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
        else if (collision.gameObject.CompareTag("L_Goal") || collision.gameObject.CompareTag("R_Goal"))
        {
            OnCollidedWithGoal?.Invoke(this, new CollisionEventArgs{collision = collision});
        }
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

    private void StartDaBall_Event(object sender, EventArgs e) {
        BallStarter();
    }
    
    private void BallStarter()
    {
        transform.position = new Vector3(0, 0, transform.position.z);
        
        int x;
        int y = UnityEngine.Random.Range(-5, 5);
        int leftOrRight = UnityEngine.Random.Range(0,2);
        
        if (leftOrRight == 0) x = -9;
        else x = 9;
        
        Vector2 directionNormalized = new Vector2(x, y).normalized;
        // direction = _tempVector.normalized;
        _rb.velocity = directionNormalized * speed;
        _inDirection = _rb.velocity;
    }
}


