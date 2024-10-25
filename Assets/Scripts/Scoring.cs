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
    public static Scoring Instance { get; private set; }
    public event EventHandler StartDaBall;
    
    [SerializeField] private TextMeshProUGUI scoreText, winnerText;
    [SerializeField] private int gameOverScore;
    [SerializeField] private GameObject gameOverTint;
    
    private int _leftScore, _rightScore;
    private GameStateManager _gameStateManager;
    private int _leftScoreCount, _rightScoreCount;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Ball.Instance.OnCollidedWithGoal += OnBallCollidedWithGoal_Event;
        _gameStateManager = GetComponent<GameStateManager>();
        
        gameOverTint.SetActive(false);
        winnerText.enabled = false;
    }

    private void OnBallCollidedWithGoal_Event(object sender, Ball.CollisionEventArgs e)
    {
        Score(e.collision);
    }
    
    private void Score(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("L_Goal"))
        {
            _rightScore++; 
            StartDaBall?.Invoke(this, EventArgs.Empty);
            //_rightScoreCount++;
        }
        else
        {
            _leftScore++;
            StartDaBall?.Invoke(this, EventArgs.Empty);
            //_leftScoreCount++;
        }
        scoreText.text = $"{_leftScore}:{_rightScore}";
        
        if(_leftScore >= gameOverScore || _rightScore >= gameOverScore) GameOver();
    }

    private void GameOver()
    {
        gameOverTint.SetActive(true);
        winnerText.enabled = true;
        
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
        
        _gameStateManager.GameOver();
    }

    public void Restart()
    {
        gameOverTint.SetActive(false);
        winnerText.enabled = false;

        _leftScore = 0;
        _rightScore = 0;
        scoreText.text = $"{_leftScore}:{_rightScore}";
        
        StartDaBall?.Invoke(this, EventArgs.Empty);
    }
}
