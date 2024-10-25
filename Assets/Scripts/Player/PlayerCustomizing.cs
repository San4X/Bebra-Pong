using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizing : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        
        render.color = new Color(
            PlayerPrefs.GetFloat("PlayerColorR"), 
            PlayerPrefs.GetFloat("PlayerColorG"), 
            PlayerPrefs.GetFloat("PlayerColorB"), 
            PlayerPrefs.GetFloat("PlayerColorA"));
    }
}
