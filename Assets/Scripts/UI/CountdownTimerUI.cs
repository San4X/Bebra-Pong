using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownTimerUI : MonoBehaviour
{

    private TextMeshProUGUI _countdownText;
    

    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.Instance.OnGameStarted += OnGameStarted_Event;
        GameStateManager.Instance.OnCountdownStarted += OnCountdownStarted_Event;
        _countdownText = GetComponent<TextMeshProUGUI>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        _countdownText.text = Mathf.Ceil(GameStateManager.Instance.GetCountdownToStartTime()).ToString();
    }

    private void OnGameStarted_Event(object sender, EventArgs e)
    {
        Hide();
    }

    private void OnCountdownStarted_Event(object sender, EventArgs e)
    {
        Show();
        Debug.Log("Countdown started!");
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
