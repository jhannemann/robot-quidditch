// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    public static Game instance;
    
    public float length;
    public int maxBalls;
    public int score;
    public int shotsBlocked;

    
    [HideInInspector]
    public bool started;
    public int numberOfBalls;
    public Image ballImage;
    
    private Text scoreText;
    private Text blockedText;
    private Text timeText;
    private Text ballsText;
    private Text gameOverText;
    private Button startGameButton;
    private Button quitButton;
    private float timeRemaining;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        
        InitGame();
    }

    private void InitGame()
    {
        started = false;
        score = 0;
        shotsBlocked = 0;
        timeRemaining = length;
        scoreText = GameObject.Find("Score Text").GetComponent<Text>();
        blockedText = GameObject.Find("Blocked Text").GetComponent<Text>();
        timeText = GameObject.Find("Time Text").GetComponent<Text>();
        ballsText = GameObject.Find("Balls Text").GetComponent<Text>();
        gameOverText = GameObject.Find("Game Over Text").GetComponent<Text>();
        startGameButton = GameObject.Find("Start Game Button").GetComponent<Button>();
        quitButton = GameObject.Find("Quit Button").GetComponent<Button>();
        ballImage = GameObject.Find("Ball Image").GetComponent<Image>();
        
        scoreText.text = "    Score: " + score.ToString();
        blockedText.text = "Blocked: " + shotsBlocked.ToString();
        timeText.text = "     Time: 0";
        ballsText.text = "     Balls: " + numberOfBalls.ToString();
        gameOverText.gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(true);
        startGameButton.onClick.AddListener(instance.StartGame);
        quitButton.gameObject.SetActive(true);
        quitButton.onClick.AddListener(Application.Quit);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("DefaultScene");
        started = true;
        score = 0;
        shotsBlocked = 0;
        timeRemaining = length;
        numberOfBalls = maxBalls;
        gameOverText.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    private void StopGame()
    {
        started = false;
        gameOverText.gameObject.SetActive(true);
        startGameButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        timeText.text = "      Time: 0";
    }
    
    void LateUpdate()
    {
        if (started)
        {
            timeRemaining -= Time.deltaTime;
            scoreText.text = "    Score: " + score.ToString();
            blockedText.text = "Blocked: " + shotsBlocked.ToString();
            timeText.text = "     Time: " + ((int)(timeRemaining)).ToString();
            ballsText.text = "     Balls: " + numberOfBalls.ToString();
            if (timeRemaining <= 0.0f)
            {
                StopGame();
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (started)
            {
                StopGame();
            }
            else
            {
                StartGame();
            }
        }

        if(Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
