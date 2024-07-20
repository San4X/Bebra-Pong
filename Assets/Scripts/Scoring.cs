using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText, winnerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private int gameOverScore;
    [SerializeField] private Button restartButton;
    private int _leftScore, _rightScore;
    private Ball _movementScript;
    private int _leftScoreCount, _rightScoreCount;

    private void Start()
    {
        _movementScript = GetComponent<Ball>();
        
        gameOverPanel.SetActive(false);
        winnerText.enabled = false;
        restartButton.enabled = false;
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


        if (collision.gameObject.CompareTag("L_Goal"))
        {
            _rightScore++; 
            _rightScoreCount++;
        }
        else
        {
            _leftScore++;
            _leftScoreCount++;
        }
        scoreText.text = $"{_leftScore}:{_rightScore}";
        
        if(_leftScoreCount >= gameOverScore || _rightScoreCount >= gameOverScore) GameOver();
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        winnerText.enabled = true;
        restartButton.enabled = true;
        
        Vector3 rotation = gameOverPanel.transform.rotation.eulerAngles;

        if (_leftScore > _rightScore)
        {
            winnerText.text = "winner \n --->";
            rotation.y = 0f;
        }
        else if (_rightScore > _leftScore)
        {
            winnerText.text = "winner \n <---";
            rotation.y = 180f;
        }
        
        gameOverPanel.transform.rotation = Quaternion.Euler(rotation);

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        gameOverPanel.SetActive(false);
        winnerText.enabled = false;
        restartButton.enabled = false;

        _leftScore = 0;
        _rightScore = 0;
        
        Time.timeScale = 1f;
        
        _movementScript.BallStarter();
    }
}
