using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    [SerializeField]
    private int _score;

    //[Header("Player Setting")]
    private Transform _player;

    [Header("Player Setting")]
    [SerializeField]
    private int _maxLives = 3;

    [SerializeField]
    private int currentLives;

    [SerializeField]
    [Range(0f, 10f)]
    private float _movementSpeed = 5f;

    private float _boundaryX = 9.4f;
    private float _boundaryY = 3.9f;

    [SerializeField]
    private GameObject _leftDamagedEngine, _rightDamagedEngine;

    [Header("Shooting Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _fireRate = 0.2f;

    [SerializeField]
    private GameObject _laserPrefab, tripleShotLaserPrefab;

    [SerializeField]
    private Transform _laserOffset;

    private AudioSource _playerAudioSource;

    [SerializeField]
    private AudioClip _audioClip;

    private bool _canFire;

    [Header("PowerUps Settings")]

    [SerializeField]
    private GameObject _playerShieldVisualizer;

    [SerializeField]
    private bool _isTripleShotActive, _isShieldActive;

    [SerializeField]
    [Range(0f, 5f)]
    private float _tripleShotTimeActive = 5f, _SpeedBoostTimeActive = 5f;

    [SerializeField]
    [Range(0f, 5f)]
    private float _speedMultiplier = 3.5f;

    [Header("TESTING")]
    [Range(0f, 5f)]
    public float blinkRate = 0.3f;

    [Range(0, 10)]
    public int numberofBlinks = 3;

    [SerializeField]
    private GameObject _playerSprite;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _playerAudioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is Null.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UIManager is Null.");
        }

        if (_playerAudioSource == null)
        {
            Debug.LogError("Player AudioSource is Null.");
        }

        currentLives = _maxLives;

        _player = transform;

        _player.position = Vector3.zero;

        _canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space) && _canFire)
        {
            Shoot();
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal") * _movementSpeed * Time.deltaTime;
        float verticalInput = Input.GetAxis("Vertical") * _movementSpeed * Time.deltaTime;

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        _player.Translate(direction);

        if (_player.position.x >= _boundaryX)
        {
            _player.position = new Vector3(-_boundaryX, _player.position.y, 0);
        }
        else if (_player.position.x <= -_boundaryX)
        {
            _player.position = new Vector3(_boundaryX, _player.position.y, 0);
        }

        _player.position = new Vector3(_player.position.x, Mathf.Clamp(_player.position.y, -_boundaryY, _boundaryY), 0);

        /*
        if (_player.position.y >= _boundaryY)
        {
            //_player.position = new Vector3(_player.position.x, -_boundaryY, 0);
            _player.position = new Vector3(_player.position.x, _boundaryY, 0);
        }
        else if (_player.position.y <= -_boundaryY)
        {
            //_player.position = new Vector3(_player.position.x, _boundaryY, 0);
            _player.position = new Vector3(_player.position.x, -_boundaryY, 0);
        }
        */

        //Testing 
        if (Input.GetKeyDown(KeyCode.T))
        {
            Damage();
            //StartCoroutine(BlinkGameObject(_playerSprite, numberofBlinks, blinkRate));          
        }
    }

    void Shoot()
    {
        if (_isTripleShotActive)
        {
            _fireRate = 0.5f;
            Instantiate(tripleShotLaserPrefab, transform.position, Quaternion.identity);
            _canFire = false;
            StartCoroutine(LaserCoolDownTimer());
        }
        else
        {
            _fireRate = 0.3f;
            Instantiate(_laserPrefab, _laserOffset.position, Quaternion.identity);
            _canFire = false;
            StartCoroutine(LaserCoolDownTimer());
        }

        _playerAudioSource.clip = _audioClip;
        _playerAudioSource.Play();
        
    }

    IEnumerator LaserCoolDownTimer()
    {
        yield return new WaitForSeconds(_fireRate);
        _canFire = true;
    }

    public IEnumerator BlinkGameObject(GameObject gameObject, int numBlinks, float seconds)
    {
       
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
       
        for (int i = 0; i < numBlinks + 1; i++)
        {
            bool isActive = gameObject.activeSelf;

            gameObject.SetActive(!isActive);
           
            yield return new WaitForSeconds(seconds);
        }

        gameObject.SetActive(true);
    }
    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _playerShieldVisualizer.SetActive(false);
            return;
        }

        currentLives--;

        if (currentLives == 2)
        {
            _rightDamagedEngine.SetActive(true);
            //_playerAudioSource.clip = _audioClips[1];
            //_playerAudioSource.Play();
        }
        else if (currentLives == 1)
        {
            _leftDamagedEngine.SetActive(true);
            //_playerAudioSource.clip = _audioClips[1];
            //_playerAudioSource.Play();
        }
     
        StartCoroutine(BlinkGameObject(_playerSprite, numberofBlinks, blinkRate));

        _uiManager.UpdateLivesDisplay(currentLives);

        if (currentLives < 1)
        {
            if (_spawnManager != null)
            {
                _spawnManager.StopSpawning();
            }


            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        _score += points;

        if (_uiManager != null)
        {
            _uiManager.UpdateScore(_score);
        }
    }

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerUpTimer());
    }

    public void ActivateSpeedBoost()
    {
        _movementSpeed += _speedMultiplier;
        StartCoroutine(SpeedBoostTimer());
    }

    public void ActivateShield()
    {
        _isShieldActive = true;
        _playerShieldVisualizer.SetActive(true);
    }

    IEnumerator TripleShotPowerUpTimer()
    {
        yield return new WaitForSeconds(_tripleShotTimeActive);
        _isTripleShotActive = false;
    }

    IEnumerator SpeedBoostTimer()
    {
        yield return new WaitForSeconds(_SpeedBoostTimeActive);
        _movementSpeed = 5f;
    }

}
