using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIManager _uiManager;

    [SerializeField]
    private bool _gamePaused;

    [SerializeField]
    private bool _isGameStarted;

    [SerializeField]
    private bool _isGameComplete;

    [SerializeField]
    private bool _isGameOver;

    [SerializeField]
    float _gameTime;

    public double b;

    public float minutes;

    public float seconds;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("UIManager is Null.");
        }
    }

    void Update()
    {
        if (_isGameStarted)
        {
            //Track time Played 
            _gameTime += Time.deltaTime;
            GameTimer();
        }



        if (Input.GetKeyUp(KeyCode.R) && _isGameOver == true)
        {
            RestartLevel();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GamePause();
            //ExitGame();
        }
    }

    void GameTimer()
    {
        b = System.Math.Round(_gameTime, 2);

        minutes = Mathf.FloorToInt(_gameTime / 60);

        seconds = Mathf.FloorToInt(_gameTime % 60);

    }

    public void GamePause()
    {
        _gamePaused = !_gamePaused;

        if (_gamePaused != false)
        {
            _uiManager.ExitGamePopUp(_gamePaused);
            Time.timeScale = 0;
        }
        else
        {
            _uiManager.ExitGamePopUp(_gamePaused);
            Time.timeScale = 1;
        }
    }

    public void GameStarted()
    {
        _isGameStarted = true;
    }

    public void GameComplete()
    {
        _isGameComplete = true;

        Time.timeScale = 0;

    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
