using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator _anim;

    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _exploAudioClip;

    [SerializeField]
    [Range(0f, 5f)]
    private float _movementSpeed = 4f;

    private float _bottomBarrier = -7f;

    [SerializeField]
    private bool _canMove = true;

    [Header("Shooting Settings")]
    [SerializeField]
    private float _fireRate = 0.3f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private Transform _barrelOffset;

    [SerializeField]
    private float _canFire = 1f;

    private Player _player;

    [SerializeField]
    private bool _canShoot;


    /*
    public Vector3[] _positions;
    private int _positionSelector;
    private int _lastSelectedEnemy;
    private int _initialSpawnPositionCount = 1;
    Vector3 posTemp;
    Vector3 _lastPos;
    */

    /*
    private void Awake()
    {
        _positionSelector = Random.Range(0, _positions.Length + 1);

        for (int i = 0; i < _initialSpawnPositionCount; i++)
        {
            if (_positionSelector == _lastSelectedEnemy)
            {
                while (_positionSelector == _lastSelectedEnemy)
                {
                    _positionSelector = Random.Range(0, _positions.Length);
                }
            }

            _lastSelectedEnemy = _positionSelector;

            Vector3 _pos = _positions[_positionSelector]; //Instantiate(_roadPrefabs[_roadSelector], i * _roadOffset, transform.rotation);

            _lastPos = _pos;

            Debug.Log(_pos);

            transform.position = _pos;

            //canSpawnRoad = false;
        }
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();

        _player = GameObject.Find("Player").GetComponent<Player>();

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogWarning("Animator is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();

        // Redo Enemy Fire //
        if (_canShoot)
        {
            EnemyFire();
        }
    }
    void EnemyMovement()
    {
        if (_canMove == true)
        {
            transform.Translate(Vector3.down * _movementSpeed * Time.deltaTime);
        }

        if (transform.position.y <= _bottomBarrier)
        {
            float newXPos = Random.Range(-8f, 8f);
            transform.position = new Vector3(newXPos, 7f, 0f);
        }
    }

    void EnemyFire()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(7f, 15f);

            _canFire = Time.time + _fireRate;

            Instantiate(_laserPrefab, _barrelOffset.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            this.gameObject.GetComponent<Collider2D>().enabled = false;

            _movementSpeed = 0;

            _anim.SetTrigger("OnDestroy");
            _audioSource.clip = _exploAudioClip;
            _audioSource.Play();

            Destroy(this.gameObject, 1.2f);

            if (_player != null)
            {
                _player.Damage();
            }

        }
        else if (other.tag == "PlayerLaser")
        {
            this.gameObject.GetComponent<Collider2D>().enabled = false;

            _movementSpeed = 0f;

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
