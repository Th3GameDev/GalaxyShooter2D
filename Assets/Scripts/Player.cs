using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private CameraShake _camShake;

    [SerializeField]
    private int _score;

    //[Header("Player Setting")]
    private Transform _player;

    [Header("Player Setting")]
    [SerializeField]
    private int _maxLives = 3;

    [SerializeField]
    private int _currentLives;

    [SerializeField]
    private int _shieldLevel;

    [SerializeField]
    [Range(0f, 10f)]
    private float _movementSpeed = 5f;

    private float _boundaryX = 9.4f;
    private float _boundaryY = 3.9f;

    [SerializeField]
    private GameObject _leftDamagedEngine, _rightDamagedEngine;

    [SerializeField]
    private GameObject _exploPrefab;

    [Header("Shooting Settings")]
    [SerializeField]
    private int _maxAmmo = 15;

    [SerializeField]
    private int _currentAmmo;

    [SerializeField]
    [Range(0f, 1f)]
    private float _fireRate = 0.2f;

    [SerializeField]
    private GameObject _laserPrefab, _tripleShotLaserPrefab, _guidedLaserPrefab;

    [SerializeField]
    private Transform _laserOffset;

    private AudioSource _playerAudioSource;

    [SerializeField]
    private AudioClip _audioClip;

    private bool _canFire;

    [Header("Thruster Settings")]
    [SerializeField]
    private GameObject _thruster;

    [SerializeField]
    private float _fuelPercentage = 100f;

    [SerializeField]
    private float _refuelSpeed;

    private bool isThrusterActive;

    [Header("PowerUps Settings")]

    [SerializeField]
    private GuidedLaserRadius _laserRadius;

    [SerializeField]
    private int _guidedAmmoMax = 3;

    private int _guidedCurrentAmmo;

    [SerializeField]
    private GameObject _playerShieldVisualizer;

    [SerializeField]
    private SpriteRenderer _shieldSpriteRend;

    [SerializeField]
    private Color _shieldColor;

    [SerializeField]
    private bool _isTripleShotActive, _isShieldActive, _isSpeedBoostActive, _isGuidedLaserActive;

    [SerializeField]
    [Range(0f, 5f)]
    private float _tripleShotTimeActive = 5f, _SpeedBoostTimeActive = 5f, _guidedLaserTimeActive = 10f;

    [SerializeField]
    [Range(0f, 5f)]
    private float _speedMultiplier = 3.5f;

    [Header("Damage Blink Settings")]
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
        _camShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        _currentAmmo = _maxAmmo;
        _guidedCurrentAmmo = _guidedAmmoMax;

        _shieldColor = _shieldSpriteRend.color;

        if (_camShake == null)
        {
            Debug.LogError("Main Camera - CameraShake Script is Null.");
        }

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

        _currentLives = _maxLives;

        _player = transform;

        _player.position = new Vector3(0, -2.5f, 0);

        _canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        //Testing 
        if (Input.GetKeyUp(KeyCode.T))
        {
            //ActivateGuidedLaser();
            //ActivateThruster();          
            //ActivateReload();
            Damage();
            //StartCoroutine(BlinkGameObject(_playerSprite, numberofBlinks, blinkRate));          
        }

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

        if (Input.GetKey(KeyCode.LeftShift) && _fuelPercentage > 0)
        {
            if (_isSpeedBoostActive)
            {
                StopCoroutine(ActivateRefuel());
                ActivateThruster();
            }
            else
            {
                StopCoroutine(ActivateRefuel());
                ActivateThruster();
                _movementSpeed = 6.5f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isThrusterActive = false;

            if (_isSpeedBoostActive)
            {
                _thruster.SetActive(false);
                StartCoroutine(ActivateRefuel());
            }
            else
            {
                _thruster.SetActive(false);
                StartCoroutine(ActivateRefuel());
                _movementSpeed = 5f;
            }
        }


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
    }

    void Shoot()
    {
        if (_isTripleShotActive)
        {
            _fireRate = 0.5f;
            Instantiate(_tripleShotLaserPrefab, _laserOffset.transform.position, Quaternion.identity);
            _canFire = false;
            StartCoroutine(LaserCoolDownTimer());
        }
        else if (_isGuidedLaserActive == true && _guidedCurrentAmmo > 0 && _laserRadius.inRadius == true)
        {
            _guidedCurrentAmmo--;
            _uiManager.UpdateAmmoCount(_guidedCurrentAmmo);
            _fireRate = 1.2f;
            Instantiate(_guidedLaserPrefab, _laserOffset.transform.position, Quaternion.identity);
            _canFire = false;
            StartCoroutine(LaserCoolDownTimer());
        }
        else if (_currentAmmo > 0 && _isGuidedLaserActive == false)
        {
            _currentAmmo--;
            _uiManager.UpdateAmmoCount(_currentAmmo);
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
            _shieldLevel--;

            if (_shieldLevel == 2)
            {
                _shieldColor.a = 0.5f;
                _shieldSpriteRend.color = _shieldColor;
                return;
            }
            else if (_shieldLevel == 1)
            {
                _shieldColor.a = 0.2f;
                _shieldSpriteRend.color = _shieldColor;
                return;
            }
            else if (_shieldLevel <= 0)
            {
                _isShieldActive = false;
                _playerShieldVisualizer.SetActive(false);
                return;
            }
        }

        _currentLives--;

        if (_currentLives == 2)
        {
            _rightDamagedEngine.SetActive(true);
            _camShake.ShakeCamera();
            //_playerAudioSource.clip = _audioClips[1];
            //_playerAudioSource.Play();
        }
        else if (_currentLives == 1)
        {
            _leftDamagedEngine.SetActive(true);
            _camShake.ShakeCamera();
            //_playerAudioSource.clip = _audioClips[1];
            //_playerAudioSource.Play();
        }

        StartCoroutine(BlinkGameObject(_playerSprite, numberofBlinks, blinkRate));

        _uiManager.UpdateLivesDisplay(_currentLives);

        if (_currentLives < 1)
        {
            if (_spawnManager != null)
            {
                _spawnManager.StopSpawning();
            }

            Instantiate(_exploPrefab, gameObject.transform.position, Quaternion.identity);

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

    void ActivateThruster()
    {
        _fuelPercentage = Mathf.Clamp(_fuelPercentage, 0, 100);

        isThrusterActive = true;

        if (_fuelPercentage > 0)
        {
            _thruster.SetActive(true);
            _fuelPercentage -= 15 * 2 * Time.deltaTime;
            _uiManager.UpdateThruster(_fuelPercentage);
        }
        else if (_fuelPercentage <= 0)
        {
            _thruster.SetActive(false);
            _fuelPercentage = 0f;
            _uiManager.UpdateThruster(_fuelPercentage);
        }
    }

    IEnumerator ActivateRefuel()
    {
        while (_fuelPercentage != 100 && isThrusterActive == false)
        {
            yield return new WaitForSeconds(0.1f);

            _fuelPercentage += 30 * _refuelSpeed * Time.deltaTime;

            _uiManager.UpdateThruster(_fuelPercentage);

            if (_fuelPercentage >= 100)
            {
                _fuelPercentage = 100;

                _uiManager.UpdateThruster(_fuelPercentage);

                break;
            }
        }
    }

    public void ActivateReload()
    {
        _currentAmmo = _maxAmmo;
        _uiManager.UpdateAmmoCount(_currentAmmo);
    }

    public void ActivateRepair()
    {
        if (_currentLives == 2)
        {
            _currentLives++;
            _rightDamagedEngine.SetActive(false);
            _uiManager.UpdateLivesDisplay(_currentLives);
        }
        else if (_currentLives == 1)
        {
            _currentLives++;
            _leftDamagedEngine.SetActive(false);
            _uiManager.UpdateLivesDisplay(_currentLives);
        }
    }

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerUpTimer());
    }

    public void ActivateGuidedLaser()
    {
        _isGuidedLaserActive = true;
        _guidedCurrentAmmo = _guidedAmmoMax;
        _uiManager.UpdateAmmoCount(_guidedCurrentAmmo);
        StartCoroutine(GuidedLaserPowerUpTimer());
    }

    public void ActivateSpeedBoost()
    {
        _isSpeedBoostActive = true;
        _movementSpeed = 5f + _speedMultiplier;
        StartCoroutine(SpeedBoostPowerUpTimer());
    }

    public void ActivateShield()
    {
        _shieldLevel = 3;
        _shieldColor.a = 1.0f;
        _shieldSpriteRend.color = _shieldColor;
        _isShieldActive = true;
        _playerShieldVisualizer.SetActive(true);
    }

    IEnumerator TripleShotPowerUpTimer()
    {
        yield return new WaitForSeconds(_tripleShotTimeActive);
        _isTripleShotActive = false;
    }

    IEnumerator SpeedBoostPowerUpTimer()
    {
        yield return new WaitForSeconds(_SpeedBoostTimeActive);
        _movementSpeed = 5f;
        _isSpeedBoostActive = false;
    }

    IEnumerator GuidedLaserPowerUpTimer()
    {
        yield return new WaitForSeconds(_guidedLaserTimeActive);
        _isGuidedLaserActive = false;
        _uiManager.UpdateAmmoCount(_currentAmmo);
        
        
    }
}
