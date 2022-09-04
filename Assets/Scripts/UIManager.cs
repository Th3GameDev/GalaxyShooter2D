using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;


    [Header("Thruster UI")]
    [SerializeField]
    private Slider _thrusterSlider;

    [SerializeField]
    private TextMeshProUGUI _fuelPercentageText;

    [Header("Score UI")]
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [Header("Ammo UI")]
    [SerializeField]
    private TextMeshProUGUI _ammoCountText;

    [Header("GameOver UI")]
    [SerializeField]
    private TextMeshProUGUI _gameOverText;

    [SerializeField]
    private TextMeshProUGUI _restartLevelText;

    [SerializeField]
    [Range(0, 10)]
    private int _gameOverNumberOfBlinks;

    [SerializeField]
    [Range(0f, 1f)]
    private float _gameOverTextBlinkTime;

    [Header("Player Lives Display UI")]
    [SerializeField]
    private Image _playerLivesDisplay;

    [SerializeField]
    private Sprite[] _playerLivesSprites;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is Null.");
        }

        _gameOverText.gameObject.SetActive(false);
        _restartLevelText.gameObject.SetActive(false);

        _scoreText.text = "Score: " + 0;

        _ammoCountText.text = "Ammo:" + 15;
    }

    public void UpdateThruster(float fuelPercentage)
    {
        _thrusterSlider.value = fuelPercentage;
        _fuelPercentageText.text = Mathf.RoundToInt(fuelPercentage) + "%";
    }

    public void UpdateAmmoCount(int playerAmmo)
    {  
        _ammoCountText.text = $"Ammo:{playerAmmo}";
    }


    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLivesDisplay(int currentLives)
    {
        _playerLivesDisplay.sprite = _playerLivesSprites[currentLives];

        if (currentLives <= 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();

        _gameOverText.gameObject.SetActive(true);
        _restartLevelText.gameObject.SetActive(true);

        StartCoroutine(BlinkGameObject(_gameOverText.gameObject, _gameOverNumberOfBlinks, _gameOverTextBlinkTime));
    }

    public IEnumerator BlinkGameObject(GameObject gameObjectOne, int numBlinks, float seconds) 
    {      
        TextMeshProUGUI text = gameObjectOne.GetComponent<TextMeshProUGUI>();
      
        for (int i = 0; i < numBlinks * 2; i++)
        {
            //toggle Text
            text.enabled = !text.enabled;            
            //wait for a bit
            yield return new WaitForSeconds(seconds);
        }

        //make sure Text is enabled when we exit
        text.enabled = true;
    }
}
