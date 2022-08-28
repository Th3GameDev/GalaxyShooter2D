using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver;


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R) && _isGameOver == true)
        {
            RestartLevel();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ExitGame();
        }
    }


    public void GameOver()
    {
        _isGameOver = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    void ExitGame()
    {
        Application.Quit();
    }

}
