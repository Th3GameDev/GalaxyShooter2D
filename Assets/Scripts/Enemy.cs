using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator _anim;

    private AudioSource _audioSource;

    [SerializeField]        // 0 = Basic Enemy / 1 = DodgingEnemy
    private int _enemyID;

    [SerializeField]
    private AudioClip _exploAudioClip;

    [Header("Movement Settings")]
    [SerializeField]
    [Range(0f, 5f)]
    private float _movementSpeed = 4f;

    private float _bottomBarrier = -7f;

    [SerializeField]
    private float _dodgeSpeed, _dodgeWaitTime;

    private float _newDodgeXPos;

    [SerializeField]
    private bool _canMove = true;

    [SerializeField]
    private bool _canDodge = false;

    private bool _dodge;

    private Vector2 _startWait = new Vector2(0.5f, 1f);

    private Vector3 _velocity;

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

        if (_enemyID == 1)
        {
            _canDodge = true;
            StartCoroutine(Dodge());
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

        //Clamp X Axis
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -8, 8), transform.position.y, 0);
    }
    void EnemyMovement()
    {
        if (_canMove == true)
        {
            transform.Translate(Vector3.down * _movementSpeed * Time.deltaTime);
        }

        if (_enemyID == 0)
        {
            if (transform.position.y <= _bottomBarrier)
            {
                float newXPos = Random.Range(-8f, 8f);
                transform.position = new Vector3(newXPos, 7f, 0f);
            }
        }
        else if (_enemyID == 1)
        {

            if (_canDodge)
            {
                float newXPos = Mathf.MoveTowards(transform.position.x, _newDodgeXPos, _dodgeSpeed * Time.deltaTime);

                transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);
            }

            if (transform.position.y > -3 && transform.position.y < 3.7f)
            {
                _dodge = true;
            }
            else
            {
                _dodge = false;
            }

            if (transform.position.y <= _bottomBarrier)
            {
                transform.position = new Vector3(transform.position.x, 7f, 0f);
            }
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
