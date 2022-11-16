using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Animator _anim;
    private AudioSource _source;

    [SerializeField]
    private AudioClip[] _audioClips;

    [Header("Power-Up Settings")]
    [SerializeField]
    [Range(1f, 5f)]
    private float _speed;

    [SerializeField] //0 == TripleShot 1 == SpeedBoost 2 == Shield
    [Range(0, 6)]
    private int _powerUpID;

    [SerializeField]
    private bool _canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _source = GetComponent<AudioSource>();
       _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_canMove)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
       
        //_audioSource = GetComponent<AudioSource>();

        if (transform.position.y <= -6.6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Player player = col.GetComponent<Player>();

            //AudioSource.PlayClipAtPoint(_audioClips[0], transform.position, 1);
            _source.clip = _audioClips[0];
            _source.Play();

            _renderer.enabled = false;

            this.gameObject.GetComponent<CircleCollider2D>().enabled = false;

            if (player != null)
            {           
                switch (_powerUpID)
                {
                    case 0:
                        player.ActivateTripleShot();                
                        break;

                    case 1:
                        player.ActivateSpeedBoost();
                        break;

                    case 2:
                        player.ActivateShield();
                        break;

                    case 3:
                        player.ActivateRepair();
                        break;

                    case 4:
                        player.ActivateReload();
                        break;

                    case 5:
                        player.ActivateGuidedLaser();
                        break;

                    case 6:
                        player.Damage();
                        break;

                    default:
                        break;
                }
            }

            Destroy(gameObject, .5f);
        }
        else if (col.CompareTag("EnemyLaser"))
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = false;

            _canMove = false;

            _anim.SetTrigger("OnDestroy");

            _source.clip = _audioClips[1];
            _source.Play();

            Destroy(gameObject, .5f);
            Destroy(col.gameObject);
        }
    }


}
