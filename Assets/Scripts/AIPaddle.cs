using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class AIPaddle : MonoBehaviour
{
    public float speed = 5.0f;
    public Ball ball;
    public GameObject aiActivationBorder;
    public float upperBoundary = 4.19f;
    public float lowerBoundary = -4.19f;
    public GameObject upperFrame, lowerFrame;
    
    private Vector3 _initialPosition, _targetPosition;
    private float _upperBoundary, _lowerBoundary;

    private void Start()
    {
        _upperBoundary = upperFrame.transform.position.y - upperFrame.GetComponent<BoxCollider2D>().size.y / 3f - GetComponent<BoxCollider2D>().size.y / 2f;
        _lowerBoundary = lowerFrame.transform.position.y + lowerFrame.GetComponent<BoxCollider2D>().size.y / 3f + GetComponent<BoxCollider2D>().size.y / 2f;
        
        _initialPosition = transform.position;
        _targetPosition = _initialPosition;
    }

    private void Update()
    {
        Vector2 ballDir = ball.GetComponent<Rigidbody2D>().velocity;
        if (ball.transform.position.x >= aiActivationBorder.transform.position.x && ballDir.x > 0) //if further than border and moving to right
        {
            // predict the intersection of the ball with a vertical line passing through the paddle
            float timeToReachPaddle = (transform.position.x - ball.transform.position.x) / ballDir.x;
            float predictedY = ball.transform.position.y + ballDir.y * timeToReachPaddle;

            _targetPosition = new Vector3(transform.position.x, predictedY, transform.position.z);
        }
        else
        {
            _targetPosition = _initialPosition;
        }
        
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(_targetPosition.x, Mathf.Clamp(_targetPosition.y, _lowerBoundary, _upperBoundary), _targetPosition.z), step);
        
    }
}

