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
    public GameObject upperFrame, lowerFrame;
    [SerializeField] private Material playerMaterial;
    
    private Vector3 _initialPosition, _targetPosition;
    private float _upperBorder, _lowerBorder;

    private bool _isRight;

    private void Start()
    {
        _upperBorder = upperFrame.transform.position.y - upperFrame.GetComponent<BoxCollider2D>().bounds.extents.y - GetComponent<BoxCollider2D>().bounds.extents.y;
        _lowerBorder = lowerFrame.transform.position.y + lowerFrame.GetComponent<BoxCollider2D>().bounds.extents.y + GetComponent<BoxCollider2D>().bounds.extents.y;
        
        _initialPosition = transform.position;
        _targetPosition = _initialPosition;

        _isRight = transform.position.x > 0;

        SpriteRenderer render = GetComponent<SpriteRenderer>();
        
        render.color = new Color(
            PlayerPrefs.GetFloat("PlayerColorR"), 
            PlayerPrefs.GetFloat("PlayerColorG"), 
            PlayerPrefs.GetFloat("PlayerColorB"), 
            PlayerPrefs.GetFloat("PlayerColorA"));
    }

    private void Update()
    {
        if (_isRight && ball.transform.position.x >= aiActivationBorder.transform.position.x && Ball.BallVelocity.x > 0) // If paddle on the right, further than activation border and moving to right
        {
            // Predict the intersection of the ball with a vertical line passing through the paddle
            float timeToReachPaddle = (transform.position.x - ball.transform.position.x) / Ball.BallVelocity.x;
            float predictedY = ball.transform.position.y + Ball.BallVelocity.y * timeToReachPaddle;

            _targetPosition = new Vector3(transform.position.x, predictedY, transform.position.z);
        }
        else if (!_isRight && ball.transform.position.x <= aiActivationBorder.transform.position.x && Ball.BallVelocity.x < 0) // If paddle on the left, further than activation border and moving to left
        {
            float timeToReachPaddle = (transform.position.x - ball.transform.position.x) / Ball.BallVelocity.x;
            float predictedY = ball.transform.position.y + Ball.BallVelocity.y * timeToReachPaddle;

            _targetPosition = new Vector3(transform.position.x, predictedY, transform.position.z);
        }
        else
        {
            _targetPosition = _initialPosition;
        }
        
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(_targetPosition.x, Mathf.Clamp(_targetPosition.y, _lowerBorder, _upperBorder), _targetPosition.z), step);
        
    }
}

