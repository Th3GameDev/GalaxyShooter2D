using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    private enum AttackState { DefaultState, BasicAttack, TimeBombAttack, LaserBeamAttack }

    [SerializeField]
    private AttackState _currentState;

    private SpawnManager _spawnManager;

    private UIManager _uiManager;

    private Animator _anim;

    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip[] _audioClips;

    [Header("Boss Health")]
    [SerializeField]
    private float _bossMaxHealth = 1000;

    [SerializeField]
    private float _bossCurrentHealth;

    [SerializeField]
    private GameObject _enemyShield;

    private Transform[] _bossEngines = new Transform[2];

    private bool _hasShield;

    private bool _isAlive = true;

    private bool canTakeDamage = false;

    [Header("Movement")]
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private bool _canMove;

    private float _barrier = 7;

    private Vector3 _targetDir;

    [Header("Shooting")]

    [SerializeField]
    private float _nextFire = 0.5f;

    private float _rateOfFire = 5.0f;

    [SerializeField]
    private GameObject laserPrefab;

    [SerializeField]
    private GameObject _bombPrefab;

    [SerializeField]
    private GameObject _laserBeamPrefab;

    [SerializeField]
    private Transform _barrelOffset;

    [SerializeField]
    private int _currentShotsFired = 0;

    [SerializeField]
    private float _fireRate;

    [SerializeField]
    private bool _canshoot;

    [SerializeField]
    private bool _attackBasic;

    [SerializeField]
    private bool _attackTimeBomb;

    [SerializeField]
    private bool _attackLaserBeam;

    [Header("Testing")]
    public Vector3[] _positions;
    private int _positionSelector;
    private int _lastSelectedPosition;
    private Vector3 _lastPos;

    private bool _startingMove = true;

    private Vector3 _startingPos = new Vector3(0, 1f, 0);

    public Vector3[] _directions;

    private void Awake()
    {
        _anim = GetComponent<Animator>();

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is Null.");
        }
        _targetDir = GetRandomDirection();

        _bossCurrentHealth = _bossMaxHealth;

        _bossEngines[0] = transform.Find("BossDamagedLeft");
        _bossEngines[1] = transform.Find("BossDamagedRight");

        if (_bossEngines[0] == null)
        {
            Debug.LogWarning("BossDamagedLeft is Null!");
        }
        else if (_bossEngines[1] == null)
        {
            Debug.LogWarning("BossDamagedRight is Null!");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("UIManager is Null.");
        }

        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_bossCurrentHealth <= 0)
        {
            BossDamage(10);          
        }

        BossMovement();
    }

    void BossMovement()
    {
        switch (_currentState)
        {
            default:
            case AttackState.DefaultState:

                if (transform.position.y > 1)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _startingPos, _moveSpeed * Time.deltaTime);
                }
                else
                {
                    //Update The UI
                    _uiManager.UpdateBossUI(_bossCurrentHealth);

                    //StartAttack
                    _currentState = AttackState.BasicAttack;

                    canTakeDamage = true;

                    _attackBasic = true;

                }

                _attackBasic = false;
                _attackTimeBomb = false;
                _attackLaserBeam = false;
                break;

            case AttackState.BasicAttack:
                _attackBasic = true;
                _attackTimeBomb = false;
                _attackLaserBeam = false;
                BossBasicMovement();
                break;

            case AttackState.TimeBombAttack:
                _attackTimeBomb = true;
                _attackBasic = false;
                _attackLaserBeam = false;
                TimeBombMovement();
                break;

            case AttackState.LaserBeamAttack:
                _attackBasic = false;
                _attackTimeBomb = false;
                _attackLaserBeam = true;
                LaserBeamMovement();
                break;
        }
    }

    //Checks what attack is active and if canShoot is true we fire and start the fire cooldown
    void BossFire()
    {
        if (_canshoot && _attackBasic)
        {
            _fireRate = 0.5f;

            _canshoot = false;

            Instantiate(laserPrefab, _barrelOffset.position, Quaternion.identity);

            _audioSource.clip = _audioClips[1];
            _audioSource.Play();

            _currentShotsFired++;

            StopCoroutine(BossAttack());

            StartCoroutine(BossAttack());
        }
        else if (_canshoot && _attackTimeBomb)
        {
            _fireRate = 0.5f;

            _canshoot = false;

            Instantiate(_bombPrefab, transform.position, Quaternion.identity);

            //Play BOMB SHOOT sound//

            _currentShotsFired++;

            StopCoroutine(BossAttack());
            StartCoroutine(BossAttack());
        }
        else if (_canshoot && _attackLaserBeam)
        {
            _fireRate = 1.0f;

            //Play laser beam sound//

            _laserBeamPrefab.SetActive(true);
        }
    }

    //Attack CoolDown Waits number of seconds and sets canMove true and canShoot true
    IEnumerator BossAttack()
    {
        yield return new WaitForSeconds(_fireRate);
        _canMove = true;
        _canshoot = true;
    }

    //Picks a random direction and moves if it reaches the boundary left/right it will move in the oppsite direction
    //It will Randomly Fire three times and return to center of screen
    //SwitchAttack
    void BossBasicMovement()
    {
        if (Time.time > _nextFire)
        {
            BossFire();

            _nextFire = Time.time + _rateOfFire;
        }

        if (_currentShotsFired >= 3)
        {
            _targetDir = Vector3.zero;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 1f, 0), _moveSpeed * Time.deltaTime);

            if (transform.position == _startingPos)
            {
                //Debug.Log("Switching Attack to: TimeBomb");
                _canshoot = true;
                _currentShotsFired = 0;
                _targetDir = GetRandomPosition();
                _currentState = AttackState.TimeBombAttack;
            }
        }

        if (_canMove)
        {
            transform.Translate(_targetDir * _moveSpeed * Time.deltaTime);

            if (transform.position.x < -_barrier)
            {
                _targetDir = Vector3.right;
            }
            else if (transform.position.x > _barrier)
            {
                _targetDir = Vector3.left;
            }
        }      
    }

    // Picks a random position and move towards it,
    // once reached target position it will wait fire once and pick a new position and move.
    //When the it has shot 2 times it will retur to center of screen
    //SwitchAttack
    void TimeBombMovement()
    {
        if (_canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetDir, _moveSpeed * Time.deltaTime);

            if (_currentShotsFired >= 2)
            {
                _currentShotsFired = 2;

                _canshoot = false;

                _targetDir = new Vector3(0, 1f, 0);

                if (transform.position == _startingPos && _startingMove != true)
                {
                    //Debug.Log("Switching Attack to: LaserBeam");
                    _canshoot = true;
                    _canMove = true;
                    _currentShotsFired = 0;
                    _startingMove = true;
                    _targetDir = GetRandomDirection();
                    _currentState = AttackState.LaserBeamAttack;
                }
            }

            if (transform.position == _targetDir)
            {
                _startingMove = false;

                _canMove = false;

                BossFire();

                _targetDir = GetRandomPosition();
            }
        }
    }

    //Picks random dir Left or Right, Moves and fires
    //Once Reached barrier moves back to center stops
    //SwitchAttack
    void LaserBeamMovement()
    {
        if (_canMove)
        {
            transform.Translate(_targetDir * _moveSpeed * Time.deltaTime);
        }      

        BossFire();

        if (_startingMove != true)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 1f, 0), _moveSpeed * Time.deltaTime);

            if (transform.position == _startingPos)
            {
                //Debug.Log("Switching Attack to: Basic");               
                _laserBeamPrefab.SetActive(false);
                _canMove = true;
                _canshoot = true;
                _currentShotsFired = 0;
                _targetDir = GetRandomDirection();
                _currentState = AttackState.BasicAttack;
            }
        }

        if (transform.position.x <= -_barrier)
        {
            //Debug.Log("Barrie Left");
            _targetDir = Vector3.zero;
            _startingMove = false;
        }
        else if (transform.position.x >= _barrier)
        {
            //Debug.Log("Barrie Left");
            _targetDir = Vector3.zero;
            _startingMove = false;
        }

    }


    void BossDamage(float damageAmount)
    {
        if (_bossCurrentHealth == 100)
        {
            _bossEngines[0].gameObject.SetActive(true);

            //Decrese boss health
            _bossCurrentHealth -= damageAmount;

            //Update the Ui
            _uiManager.UpdateBossUI(_bossCurrentHealth);


            StopCoroutine(DamageEffect(gameObject, 1, .3f));

            //Play damage effect
            StartCoroutine(DamageEffect(gameObject, 1, .3f));
        }
        else if (_bossCurrentHealth == 50)
        {
            _bossEngines[1].gameObject.SetActive(true);

            //Decrese boss health
            _bossCurrentHealth -= damageAmount;

            //Update the Ui
            _uiManager.UpdateBossUI(_bossCurrentHealth);


            StopCoroutine(DamageEffect(gameObject, 1, .3f));

            //Play damage effect
            StartCoroutine(DamageEffect(gameObject, 1, .3f));
        }
        else if (_bossCurrentHealth == 0)
        {
            Debug.Log("Blow Up");

            if (_spawnManager != null)
            {
                _spawnManager.StopSpawning();
                _spawnManager.stopSpawningPowerUp = true;
            }

            _audioSource.clip = _audioClips[0];        
            _bossEngines[0].gameObject.SetActive(false);
            _bossEngines[1].gameObject.SetActive(false);

            _bossCurrentHealth = 0;

            _canMove = false;

            _canshoot = false;

            _isAlive = false;

            this.gameObject.GetComponent<Collider2D>().enabled = false;

            StartCoroutine(BossExplode());       
        }
        else if (_bossCurrentHealth > 0 && canTakeDamage == true)
        {
            //Decrese boss health
            _bossCurrentHealth -= damageAmount;

            //Update the Ui
            _uiManager.UpdateBossUI(_bossCurrentHealth);

            _audioSource.clip = _audioClips[2];
            _audioSource.Play();

            StopCoroutine(DamageEffect(gameObject, 1, .3f));

            //Play damage effect
            StartCoroutine(DamageEffect(gameObject, 1, .3f));
        }
    }

    IEnumerator BossExplode()
    {
        _anim.SetTrigger("BossExplode");

        yield return new WaitForSeconds(3.5f);

        _anim.SetTrigger("BossDestroy");

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    public IEnumerator DamageEffect(GameObject gameObject, int numBlinks, float seconds)
    {

        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();

        for (int i = 0; i < numBlinks + 1; i++)
        {
            renderer.color = Color.red;

            Color red = new Color(255, 0, 0);

            Color white = new Color(255, 255, 255);

            if (renderer.color != red)
            {               
                renderer.color = red;
            }
            else if (renderer.color != white)
            {
                renderer.color = white;
            }        

            yield return new WaitForSeconds(seconds);
        }

        renderer.color = Color.white;
    }


    //Picks and returns a random position from array
    Vector3 GetRandomPosition()
    {        
        int positionSelector = Random.Range(0, _positions.Length);

        Vector3 newPos = _positions[positionSelector];

        if (_startingMove && newPos == _startingPos)
        {
            positionSelector = Random.Range(0, _positions.Length);

            newPos = _positions[positionSelector];
        }
        else if (_startingMove != true && positionSelector == _lastSelectedPosition)
        {
            while (positionSelector == _lastSelectedPosition)
            {
                positionSelector = Random.Range(0, _positions.Length);

                newPos = _positions[positionSelector];
            }
        }

        _lastSelectedPosition = positionSelector;

        _lastPos = newPos;

        return newPos;
    }

    //Picks and returns a random Direction Left/Right
    Vector3 GetRandomDirection()
    {
        int selectedDir = Random.Range(0, _directions.Length);

        Vector3 randomDir = _directions[selectedDir];

        return randomDir;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            BossDamage(30);
        }
        else if (other.tag == "PlayerLaser")
        {
            BossDamage(10);
            Destroy(other.gameObject);
        }
    }

}
