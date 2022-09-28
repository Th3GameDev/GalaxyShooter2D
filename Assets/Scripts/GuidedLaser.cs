using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedLaser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 5.0f;

    private GuidedLaserRadius _laserRadius;

    private Player _player;

    public Transform target;

    [SerializeField]
    private bool _isEnemyLaser;

    // Start is called before the first frame update
    void Start()
    {
        if (_isEnemyLaser)
        {
            _player = GameObject.Find("Player").GetComponent<Player>();

            if (_player == null)
            {
                Debug.Log("Player Script is Null!!");
            }
        }
        else
        {
            _laserRadius = GameObject.Find("LaserRadius").GetComponent<GuidedLaserRadius>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (_isEnemyLaser)
        {
            target = _player.transform;

            if (target != null)
            {
                MoveTowardsTarget(target);
            }
        }
        else
        {
            target = _laserRadius.closestTarget;

            if (target != null)
            {
                MoveTowardsTarget(target);
            }
        }
    }


    void MoveTowardsTarget(Transform target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, _laserSpeed * Time.deltaTime);
        transform.up = target.transform.position - transform.position;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && _isEnemyLaser == true)
        {        
            if (_player != null)
            {
                _player.Damage();

                Destroy(this.gameObject);
            }
        }
    }
}
