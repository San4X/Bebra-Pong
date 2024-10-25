using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    
    [SerializeField] private GameObject colorPickerUI;
    [SerializeField] private GameObject modeSelectionUI;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        colorPickerUI.SetActive(false);
        modeSelectionUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void LoadGameScene()
    {
        if(NetworkManager.Singleton.IsClient) NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        else SceneManager.LoadScene("Game");
    }
}
