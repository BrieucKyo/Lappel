﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool DebugMode = false;
    [NonSerialized]
    public bool enteredGame = false;  
    [NonSerialized]
    public bool introShowed = false; 
    
    public event Action onPlayerStart;

    void Start()
    {
        if (DebugMode) EnterGame();
        if (SceneManager.GetActiveScene().name == "Main Scene")
        {
            onPlayerStart += EnterGame;
            StartCoroutine("LetsGo");
        }
        else
        {
            EnterGame();
            PlayerManager.Instance.canMove = true;
        }
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0) UIManager.Instance.HideCommands();
        if (enteredGame || !introShowed) return;
        if (Input.GetKey(KeyCode.Space)) PlayerEnterGame();
    }
    
    private void PlayerEnterGame()
    {
        if (onPlayerStart != null) onPlayerStart();
    }

    private void EnterGame()
    {
        if (SceneManager.GetActiveScene().name == "Main Scene") MainSceneManager.Instance.Play();
        enteredGame = true;
    }

    IEnumerator LetsGo()
    {
        yield return new WaitForSeconds(7f);
        UIManager.Instance.ShowInteraction();
    }
}