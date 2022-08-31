using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    //private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _audioClip;

    [Header("Power-Up Settings")]
    [SerializeField]
    [Range(1f, 5f)]
    private float _speed;

    [SerializeField] //0 == TripleShot 1 == SpeedBoost 2 == Shield
    [Range(0, 3)]
    private int _powerUpID;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
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

            AudioSource.PlayClipAtPoint(_audioClip, transform.position, 1);

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

                    default:
                        break;
                }
            }

            Destroy(gameObject);
        }
    }
}
