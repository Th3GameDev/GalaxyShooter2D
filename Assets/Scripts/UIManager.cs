using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;
    private Player _p;

    [Header("Boss UI")]

    [SerializeField]
    private TextMeshProUGUI _bossAiNameText;

    [SerializeField]
    private Slider _bossHealthSlider;

    //[SerializeField]
    //private TextMeshProUGUI _bossHealthPercentageText;

    [Header("Wave UI")]
    [SerializeField]
    private TextMeshProUGUI _waveCounter;

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

    [Header("Game Paused UI")]
    [SerializeField]
    private GameObject _exitPopUpWindow;

    [Header("GameComplete UI")]
    [SerializeField]
    private GameObject _gameCompleteWindow;

    [SerializeField]
    private TextMeshProUGUI _gameScoreTotal;

    [SerializeField]
    private TextMeshProUGUI _gameTimeTotal;


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
        _p = GameObject.Find("Player").GetComponent<Player>();
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

    void Update()
    {

    }

    public void UpdateWaveStartDisplay(int currentWave)
    {
        _waveCounter.gameObject.SetActive(true);
        _waveCounter.text = "Wave:" + currentWave;
        StartCoroutine(BlinkGameObject(_waveCounter.gameObject, 2, .7f, false));
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

    public void UpdateBossUI(float healthPercentage)
    {

        if (_bossHealthSlider.gameObject.activeSelf != true)
        {
            _bossAiNameText.gameObject.SetActive(true);
            _bossHealthSlider.gameObject.SetActive(true);
        }

        if (healthPercentage <= 0)
        {
            _bossAiNameText.gameObject.SetActive(false);
            _bossHealthSlider.gameObject.SetActive(false);
        }

        _bossHealthSlider.value = healthPercentage;
        //_bossHealthPercentageText.text = Mathf.RoundToInt(healthPercentage) + "%";
    }

    public void GameCompleteUI()
    {

        _gameCompleteWindow.SetActive(true);

        if (_p != null)
        {
            _gameScoreTotal.text = $"{_p.currentScore.ToString()}";
        }

        _gameTimeTotal.text = string.Format("{0,00}:{1,00}", _gameManager.minutes.ToString(), _gameManager.seconds.ToString());
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();

        _gameOverText.gameObject.SetActive(true);
        _restartLevelText.gameObject.SetActive(true);

        StartCoroutine(BlinkGameObject(_gameOverText.gameObject, _gameOverNumberOfBlinks, _gameOverTextBlinkTime, true));
    }

    public void ExitGamePopUp(bool paused)
    {
        //Pop Up Exit UI
        if (paused)
        {
            _exitPopUpWindow.GetComponent<Animator>().SetTrigger("OnWindowOpen");
            _exitPopUpWindow.SetActive(true);
        }
        else
        {
            _exitPopUpWindow.GetComponent<Animator>().SetTrigger("OnWindowClose");
            _exitPopUpWindow.SetActive(false);
        }       
    }

    public IEnumerator BlinkGameObject(GameObject gameObjectOne, int numBlinks, float seconds, bool diactivateOnExit)
    {
        TextMeshProUGUI text = gameObjectOne.GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < numBlinks * 2; i++)
        {
            //toggle Text
            text.enabled = !text.enabled;
            //wait for a bit
            yield return new WaitForSeconds(seconds);
        }

        if (diactivateOnExit)
        {
            //make sure Text is enabled when we exit
            text.enabled = true;
        }
        else
        {
            text.enabled = false;
        }
    }
}
