using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    public static ScoreTrigger Instance { get; private set; }
    
    public event EventHandler<GoalEventArgs> OnGoalTrigger;
    public class GoalEventArgs : EventArgs
    {
        public string Tag;
    }

    private string _tag;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _tag = gameObject.tag;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnGoalTrigger?.Invoke(this, new GoalEventArgs{Tag = _tag});
    }
}
