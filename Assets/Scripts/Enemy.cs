using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private WaveManager _waveManager;

    private Animator _anim;

    private AudioSource _audioSource;

    [SerializeField]        // 0 = Basic Enemy / 1 = AggressiveEnemy
    private int _enemyID;

    [SerializeField]
    private AudioClip _exploAudioClip;

    private bool isAlive = true;

    [Header("Movement Settings")]
    [SerializeField]
    [Range(0f, 5f)]
    private float _movementSpeed = 4f;

    private float _bottomBarrier = -7f;

    [SerializeField]
    private float _ramSpeed;

    [SerializeField]
    private float _dodgeSpeed, _dodgeWaitTime;

    private float _newDodgeXPos;

    [SerializeField]
    private bool _canMove = true;

    [SerializeField]
    private bool _canDodge = false;

    [SerializeField]
    private bool isAggressive;

    private bool _dodge;

    private Vector2 _startWait = new Vector2(0.5f, 1f);

    [Header("Shooting Settings")]
    [SerializeField]
    private float _fireRate = 0.3f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _aggressiveEnemyLaser;

    [SerializeField]
    private GameObject _guidedLaserPrefab;

    [SerializeField]
    private Transform _barrelOffset;

    private Player _player;

    [SerializeField]
    private bool _canShoot;

    [SerializeField]
    private bool _guidedLaser;

    private bool _firstPass = true;

    [SerializeField]
    private GameObject _enemyShield;

    private bool _hasShield;

    private float _distance;




    // Start is called before the first frame update
    void Start()
    {
        _waveManager = GameObject.Find("SpawnManager").GetComponent<WaveManager>();

        if (_waveManager == null)
        {
            Debug.LogError("Wave Manager is Null!");
        }

        _audioSource = gameObject.GetComponent<AudioSource>();

        _player = GameObject.Find("Player").GetComponent<Player>();

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogWarning("Animator is Null!");
        }

        if (_enemyID == 2)
        {
            _canDodge = true;
            StartCoroutine(Dodge());
        }
        else if (_enemyID == 1)
        {
            isAggressive = true;
        }

        if (_waveManager.currentWave >= 2)
        {
            int num = Random.Range(0, 3);

            if (num == 1)
            {
                _hasShield = true;
                _enemyShield.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();

        if (_canShoot && isAlive && isAggressive == false)
        {
            EnemyFire();
        }

        //Clamp X Axis
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -8, 8), transform.position.y, 0);
    }

    private void FixedUpdate()
    {
        if (_enemyID == 1)
        {
            RaycastHit2D hit = Physics2D.Raycast(_barrelOffset.position, Vector2.up);

            if (hit.collider != null)
            {
                if (hit.collider.tag == "Player")
                {
                    Debug.DrawRay(transform.position, Vector2.up * 5, Color.green);

                    if (_canShoot)
                    {
                        EnemyFire();
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, Vector2.up * 5, Color.red);
                }
            }
        }
        else if (_enemyID == 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(_barrelOffset.position, Vector2.down * .5f);

            if (hit.collider != null)
            {
                if (hit.collider.tag == "PowerUp")
                {
                    Debug.DrawRay(_barrelOffset.position, Vector2.down * 5, Color.green);
                    
                    EnemyFire();
                }
            }
            else
            {
                Debug.DrawRay(_barrelOffset.position, Vector2.down * 5, Color.red);
            }
        }
    }


    void EnemyMovement()
    {
        if (_canMove == true)
        {
            if (isAggressive != true)
            {
                transform.Translate(Vector3.down * _movementSpeed * Time.deltaTime);
            }
            else if (isAggressive)
            {
                if (_player != null)
                {
                    _distance = Vector2.Distance(this.transform.position, _player.transform.position);

                    if (_distance > 3)
                    {
                        transform.Translate(Vector3.down * _movementSpeed * Time.deltaTime);

                        transform.up = Vector3.zero;
                    }
                    else if (_distance < 3)
                    {
                        if (isAlive != true)                            
                            return;

                        transform.position = Vector3.Lerp(transform.position, _player.transform.position, _ramSpeed * Time.deltaTime);

                        transform.up = this.transform.position - _player.transform.position;                       
                    }                  
                }
                else
                {
                    transform.Translate(Vector3.down * _movementSpeed * Time.deltaTime);

                    transform.up = Vector2.zero;
                }
            }

            if (transform.position.y <= _bottomBarrier)
            {
                float newXPos = Random.Range(-8f, 8f);
                transform.position = new Vector3(newXPos, 7f, 0f);
                _firstPass = false;
            }
        }

        if (_enemyID == 2)
        {
            if (_canDodge)
            {
                float newXPos = Mathf.MoveTowards(transform.position.x, _newDodgeXPos, _dodgeSpeed * Time.deltaTime);

                transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);
            }

            if (transform.position.y > -3 && transform.position.y < 4f)
            {
                _dodge = true;
            }
            else
            {
                _dodge = false;
            }
        }
    }


    void EnemyFire()
    {
        if (_guidedLaser && _firstPass == false)
        {
            _canShoot = false;
            Instantiate(_guidedLaserPrefab, _barrelOffset.position, Quaternion.identity);
            StartCoroutine(EnemyFireCoolDown());
        }
        else if (isAggressive == true)
        {
            _canShoot = false;
            Instantiate(_aggressiveEnemyLaser, _barrelOffset.position, Quaternion.identity);
            StartCoroutine(EnemyFireCoolDown());
        }
        else if (_firstPass == false && isAggressive == false)
        {
            _canShoot = false;
            Instantiate(_laserPrefab, _barrelOffset.position, Quaternion.identity);
            StartCoroutine(EnemyFireCoolDown());
        }
    }


    IEnumerator EnemyFireCoolDown()
    {
        if (_guidedLaser)
        {
            if (_firstPass)
            {
                _fireRate = Random.Range(1.5f, 2f);

                yield return new WaitForSeconds(_fireRate);

                _canShoot = true;
            }
            else
            {

                _fireRate = Random.Range(10f, 15f);

                yield return new WaitForSeconds(_fireRate);

                _canShoot = true;
            }
        }
        else
        {

            if (_firstPass)
            {
                _fireRate = Random.Range(1f, 1.5f);

                yield return new WaitForSeconds(_fireRate);

                _canShoot = true;

            }
            else
            {
                _fireRate = Random.Range(4f, 6f);

                yield return new WaitForSeconds(_fireRate);

                _canShoot = true;
            }
        }
    }

    IEnumerator Dodge()
    {
        yield return new WaitForSeconds(Random.Range(_startWait.x, _startWait.y));

        while (true)
        {
            if (_dodge)
            {
                _newDodgeXPos = Random.Range(-8, 8) * -Mathf.Sign(transform.position.x);
                //Debug.Log("Dodge Pos = " + _newDodge);
                yield return new WaitForSeconds(_dodgeWaitTime);
            }

            yield return new WaitForSeconds(.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            if (_hasShield)
            {
                _hasShield = false;
                _enemyShield.SetActive(false);
                _player.Damage();
            }
            else
            {
                _waveManager.enemiesLeft--;

                this.gameObject.GetComponent<Collider2D>().enabled = false;

                _movementSpeed = 0;
                _ramSpeed = 0;
                _dodgeSpeed = 0;
                isAlive = false;

                _anim.SetTrigger("OnDestroy");
                _audioSource.clip = _exploAudioClip;
                _audioSource.Play();

                Destroy(this.gameObject, 1.2f);

                if (_player != null)
                {
                    _player.Damage();
                }
            }

        }
        else if (other.tag == "PlayerLaser")
        {

            if (_hasShield)
            {
                _hasShield = false;
                _enemyShield.SetActive(false);
                Destroy(other.gameObject);
            }
            else
            {
                _waveManager.enemiesLeft--;

                this.gameObject.GetComponent<Collider2D>().enabled = false;

                _movementSpeed = 0f;
                _dodgeSpeed = 0;
                isAlive = false;

                _anim.SetTrigger("OnDestroy");

                _audioSource.clip = _exploAudioClip;
                _audioSource.Play();

                Destroy(this.gameObject, 1.2f);

                Destroy(other.gameObject);

                if (_player != null)
                {
                    _player.AddScore(10);
                }
            }
        }
    }
}
