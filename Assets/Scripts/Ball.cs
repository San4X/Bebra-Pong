using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Random = System.Random;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    private Random _rnd = new Random();
    private int _ranX;
    private int _ranY;
    private Vector2 _ranDir;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        _ranX = _rnd.Next(-5, 5);
        _ranY = _rnd.Next(-5, 5);
        _ranDir = new Vector2(_ranX, _ranY);
        rb.AddForce(_ranDir * 100f);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
