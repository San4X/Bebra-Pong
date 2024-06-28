using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class Scoring : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    private int _leftScore, _rightScore;
    private Ball _movementScript;

    private void Start()
    {
        _movementScript = GetComponent<Ball>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("L_Goal") || collision.gameObject.CompareTag("R_Goal"))
        {
            Score(collision);
        }
    }

    void Score(Collision2D collision)
    {
        if (_movementScript != null)
        {
            //_movementScript.enabled = false;
            transform.position = new Vector3(0, 0, transform.position.z);
            _movementScript.BallStarter();
        }
        //_movementScript.enabled = true;
        
        
        if (collision.gameObject.CompareTag("L_Goal")) _rightScore++;
        else _leftScore++;
        scoreText.text = $"{_leftScore}:{_rightScore}";
    }
}
