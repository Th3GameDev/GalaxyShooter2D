using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBomb : MonoBehaviour
{
    Animator anim;

    [SerializeField]
    private GameObject _bombFillRadius;

    [SerializeField]
    private float _moveSpeed = 5f;

    [SerializeField]
    private float _scaleRate;

    [SerializeField]
    private bool _startBombTimer;


    [Header("Testing")]
    public Vector3[] _positions;
    private int _positionSelector;
    private int _lastSelectedPosition;
    private Vector3 _lastPos;

    private Vector3 _targetPos;


    private void Awake()
    {
        _targetPos = GetRandomPosition();

        anim = GetComponent<Animator>();

        anim.SetTrigger("Timer");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeed * Time.deltaTime);

        if (transform.position == _targetPos)
        {
            StartBombTimer();           
        }
    }


    //Scales the Object to a specified size and stops
    void StartBombTimer()
    {
        if (_startBombTimer != false)
        {
            Vector3 scaleValue = new Vector3(20f, 20f, 20f);
            
            _bombFillRadius.gameObject.transform.localScale = Vector3.Lerp(_bombFillRadius.gameObject.transform.localScale, scaleValue, _scaleRate * Time.deltaTime);

            if (_bombFillRadius.transform.localScale.x > 1.2f)
            {
                _startBombTimer = false;
                anim.SetTrigger("Explode");
                ExplosionDamage(transform.position, 1.6f);
                _bombFillRadius.SetActive(false);
            }
        }
    }

    //Bomb dont estroy null check never gets ran even if null//
    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);      

        if (hitColliders.Length == 1)
        {
            Destroy(gameObject, 0.7f);         
        }

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Player")
            {
                Player player = hitCollider.GetComponent<Player>();

                player.Damage();

                Destroy(gameObject, 0.7f);
            }          
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.6f);
    }

    //Picks and returns a random position from array
    Vector3 GetRandomPosition()
    {
        Vector3 randomPos = Vector3.zero;

        _positionSelector = Random.Range(0, _positions.Length);


        if (_positionSelector == _lastSelectedPosition)
        {
            while (_positionSelector == _lastSelectedPosition)
            {
                _positionSelector = Random.Range(0, _positions.Length);
            }
        }

        _lastSelectedPosition = _positionSelector;

        randomPos = _positions[_positionSelector]; //Instantiate(_roadPrefabs[_roadSelector], i * _roadOffset, transform.rotation);

        _lastPos = randomPos;


        return randomPos;
    }

}
