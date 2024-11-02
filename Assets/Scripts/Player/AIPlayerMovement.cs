using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class AIPaddleMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private GameObject aiActivationBorder;
    
    private GameObject _upperFrame, _lowerFrame;
    private float _upperBorder, _lowerBorder;
    private GameObject _ball;
    private Vector3 _initialPosition, _targetPosition;

    private bool _isRight;

    private void Start()
    {
        if (GameObject.FindWithTag("Ball"))
        {
            _ball = GameObject.FindWithTag("Ball");
        }
        
        _upperFrame = GameObject.FindWithTag("UpperFrame");
        _lowerFrame = GameObject.FindWithTag("LowerFrame");
        
        _upperBorder = _upperFrame.transform.position.y - _upperFrame.GetComponent<BoxCollider2D>().bounds.extents.y - GetComponent<BoxCollider2D>().bounds.extents.y;
        _lowerBorder = _lowerFrame.transform.position.y + _lowerFrame.GetComponent<BoxCollider2D>().bounds.extents.y + GetComponent<BoxCollider2D>().bounds.extents.y;
        
        _initialPosition = transform.position;
        _targetPosition = _initialPosition;

        _isRight = transform.position.x > 0;
        
        if (_isRight) aiActivationBorder.transform.localPosition = new Vector3(-26.5f, 0, 0);
        else aiActivationBorder.transform.localPosition = new Vector3(26.5f, 0, 0);
    }

    private void Update()
    {
        if (BallMovement.Instance == null) return;
        if (_isRight && _ball.transform.position.x >= aiActivationBorder.transform.position.x && BallMovement.Instance.ballVelocity.x > 0) // If paddle on the right, further than activation border and moving to right
        {
            // Predict the intersection of the _ball with a vertical line passing through the paddle
            float timeToReachPaddle = (transform.position.x - _ball.transform.position.x) / BallMovement.Instance.ballVelocity.x;
            float predictedY = _ball.transform.position.y + BallMovement.Instance.ballVelocity.y * timeToReachPaddle;

            _targetPosition = new Vector3(transform.position.x, predictedY, transform.position.z);
        }
        else if (!_isRight && _ball.transform.position.x <= aiActivationBorder.transform.position.x && BallMovement.Instance.ballVelocity.x < 0) // If paddle on the left, further than activation border and moving to left
        {
            float timeToReachPaddle = (transform.position.x - _ball.transform.position.x) / BallMovement.Instance.ballVelocity.x;
            float predictedY = _ball.transform.position.y + BallMovement.Instance.ballVelocity.y * timeToReachPaddle;

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

