using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreHandler : NetworkBehaviour
{
    public static ScoreHandler Instance { get; private set; }
    public event EventHandler NeedBallRestart;
    public event EventHandler NeedBallReset;
    
    [SerializeField] private TextMeshProUGUI scoreText, winnerText;
    [SerializeField] private int scoreLimit;
    [SerializeField] private GameObject gameOverTint;
    
    private int _leftScore, _rightScore;
    private GameStateManager _gameStateManager;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ScoreTrigger.Instance.OnGoalTrigger += OnGoalTrigger_Event;
        GameStateManager.Instance.OnCountdownStarted += OnCountdownStarted_Event;
        _gameStateManager = GetComponent<GameStateManager>();
        
        gameOverTint.SetActive(false);
        winnerText.enabled = false;
    }

    private void OnGoalTrigger_Event(object sender, ScoreTrigger.GoalEventArgs e)
    {
        Score(e.Side);
    }
    
    private void OnCountdownStarted_Event(object sender, EventArgs e)
    {
        NeedBallReset?.Invoke(this, EventArgs.Empty);
        HideGameOverUI();
        ResetScore();
    }
    
    private void Score(string goalSide)
    {
        if (goalSide == "Left") _rightScore++;
        else _leftScore++;
        
        scoreText.text = $"{_leftScore}:{_rightScore}";
        if(_leftScore >= scoreLimit || _rightScore >= scoreLimit) GameOver();
        
        NeedBallRestart?.Invoke(this, EventArgs.Empty);
    }

    private void GameOver()
    {
        ShowGameOverUI();
        
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

    private void ResetScore()
    {
        HideGameOverUI();

        _leftScore = 0;
        _rightScore = 0;
        scoreText.text = $"{_leftScore}:{_rightScore}";
    }

    private void ShowGameOverUI()
    {
        gameOverTint.SetActive(true);
        winnerText.enabled = true;
    }
    
    private void HideGameOverUI()
    {
        gameOverTint.SetActive(false);
        winnerText.enabled = false;
    }
}
