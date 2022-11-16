using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private Player _player;

    public bool canMove = false;

    [Header("Player Laser")]
    [SerializeField]
    [Range(0f, 10f)]
    private float _laserSpeed = 8f;

    private float _boundaryY = 5.5f;

    [SerializeField]
    private bool _isGuidedLaser = false;

    [Header("Enemy Laser")]
    [SerializeField]
    private bool _isEnemyLaser = false;

    [SerializeField]
    private bool _isAggressive;

    // Start is called before the first frame update
    void Start()
    {       
        //Throws Error
        if (_isGuidedLaser)
        {
             _player = GameObject.Find("Player").GetComponent<Player>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else if (_isEnemyLaser == true && _isAggressive == true)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        if (canMove == true)
        {
            transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);
        }

        if (transform.position.y >= _boundaryY)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }

    void MoveDown()
    {
        if (_isGuidedLaser == true)
        {
            if (_player != null)
            {
                Vector3 targetDir = transform.position - _player.transform.position;
                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), .1f);
                Destroy(gameObject, 1.2f);
            }
            else
            {
                Destroy (gameObject);
                Debug.Log("Player is Null");
            }
        }

        if (canMove == true)
        {
            transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);
        }

        if (transform.position.y <= -_boundaryY)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }


    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && _isEnemyLaser == true)
        {
            Player player = col.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();

                Destroy(this.gameObject);
            }
        }
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        if (_isGuidedLaser == false)
            return;

        if (_isEnemyLaser == false)
        {
            if (col.gameObject.tag == "Enemy")
            {
                //transform.up = col.transform.position - this.transform.position;

                Vector3 targetDir = col.transform.position - transform.position;
                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), .1f);

                //transform.up = col.transform.position - transform.position;
            }
        }
    }
}
