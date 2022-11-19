using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private WaveManager _waveManager;
    private GameManager _gameManager;

    [SerializeField]
    private GameObject _exploPrefab;

    [SerializeField]
    private float _rotSpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _waveManager = GameObject.Find("SpawnManager").GetComponent<WaveManager>();
       
        if (_waveManager == null)
        {
            Debug.LogWarning("Wave Manager is Null");
        }

        if (_gameManager == null)
        {
            Debug.LogWarning("Game Manager is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {     
        transform.Rotate(new Vector3(0, 0, -1 * _rotSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
       if (col.gameObject.CompareTag("PlayerLaser"))
       {        
            Instantiate(_exploPrefab, gameObject.transform.position, Quaternion.identity);
            
            Destroy(col.gameObject);

            _gameManager.GameStarted();         
            _waveManager.StartWave();

            Destroy(gameObject);
       }
    }
}
