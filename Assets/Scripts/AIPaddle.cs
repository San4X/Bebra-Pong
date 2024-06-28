using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIPaddle : MonoBehaviour
{
    public float speed = 5.0f;
    public Transform ball;
    public float reactionTime = 0.5f;
    public float upperBoundary = 4.19f;
    public float lowerBoundary = -4.19f;
    public float xBorder = 2f;

    

    private void Update()
    {

        if (ball.position.x >= 2)
        {
            if (ball.position.y > transform.position.y && transform.position.y < upperBoundary)
            {
                transform.Translate(Vector2.up * (speed * Time.deltaTime));
            }
            else if (ball.position.y < transform.position.y && transform.position.y > lowerBoundary)
            {
                transform.Translate(Vector2.down * (speed * Time.deltaTime));
            }
        }
        
    }
}

