using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText, winnerText;
    [SerializeField] private GameObject gameOverTint;
    [SerializeField] private int gameOverScore;
    [SerializeField] private GameObject restartButton, homeButton;
    private int _leftScore, _rightScore;
    private Ball _movementScript;
    private int _leftScoreCount, _rightScoreCount;

    private void Start()
    {
        _movementScript = GetComponent<Ball>();
        
        gameOverTint.SetActive(false);
        winnerText.enabled = false;
        restartButton.SetActive(false);
        homeButton.SetActive(false);
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
        if (_movementScript != null) _movementScript.BallStarter();
        
        if (collision.gameObject.CompareTag("L_Goal"))
        {
            _rightScore++; 
            //_rightScoreCount++;
        }
        else
        {
            _leftScore++;
            //_leftScoreCount++;
        }
        scoreText.text = $"{_leftScore}:{_rightScore}";
        
        if(_leftScore >= gameOverScore || _rightScore >= gameOverScore) GameOver();
    }

    void GameOver()
    {
        gameOverTint.SetActive(true);
        winnerText.enabled = true;
        restartButton.SetActive(true);
        homeButton.SetActive(true);
        
        Vector3 rotation = gameOverTint.transform.rotation.eulerAngles;

        if (_leftScore > _rightScore)
        {
            winnerText.text = "winner \n<=====";
            rotation.y = 180f;
        }
        else if (_rightScore > _leftScore)
        {
            winnerText.text = "winner \n=====>";
            rotation.y = 0f;
        }
        
        gameOverTint.transform.rotation = Quaternion.Euler(rotation);

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        gameOverTint.SetActive(false);
        winnerText.enabled = false;
        restartButton.SetActive(false);
        homeButton.SetActive(false);

        _leftScore = 0;
        _rightScore = 0;
        scoreText.text = $"{_leftScore}:{_rightScore}";
        
        Time.timeScale = 1f;
        
        _movementScript.BallStarter();
    }
}
