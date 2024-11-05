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
        public string Side;
    }
    

    private void Awake()
    {
        Instance = this;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.transform.position.x < 0) OnGoalTrigger?.Invoke(this, new GoalEventArgs{Side = "Left"});
        else OnGoalTrigger?.Invoke(this, new GoalEventArgs{Side = "Right"});
    }
}
