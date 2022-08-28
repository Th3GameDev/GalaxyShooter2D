using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    [Range(0f, 5f)]
    private float _movementSpeed = 4f;

    private float _bottomBarrier = -7f;

    private Player _player;

    
    private bool canMove;
    //private bool animDonePlaying = false;

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
        canMove = true;

        _player = GameObject.Find("Player").GetComponent<Player>();

        anim = GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogWarning("Animator is Null");
        }

        //float startXPos = Random.Range(-8f, 8f);

        //transform.position = new Vector3(startXPos, 10f, 0f);      
    }

    // Update is called once per frame
    void Update()
    {       
        if (canMove == true)
        {
            transform.Translate(Vector3.down * _movementSpeed * Time.deltaTime);
        }
        
        
        if (transform.position.y <= _bottomBarrier)
        {
            float newXPos = Random.Range(-8f, 8f);
            transform.position = new Vector3(newXPos, 7f, 0f);       
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            this.gameObject.GetComponent<Collider2D>().enabled = false;

            _movementSpeed = 0;

            anim.SetTrigger("OnDestroy");

            Destroy(this.gameObject, 1.2f);

            if (_player != null)
            {
                _player.Damage();
            }

        }
        else if (other.tag == "Laser")
        {
            this.gameObject.GetComponent<Collider2D>().enabled = false;

            _movementSpeed = 0f;

            anim.SetTrigger("OnDestroy");

            Destroy(this.gameObject, 1.2f);

            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }
        }
    }
}
