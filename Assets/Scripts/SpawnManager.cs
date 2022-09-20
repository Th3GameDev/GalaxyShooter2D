using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private WaveManager _waveManager;

    [Header("Enemy Spawning")]
    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject[] _enemyPrefabs;


    [Header("PowerUp Spawning")]
    [SerializeField]
    private GameObject[] _powerUpPrefabs;

    [SerializeField]
    private GameObject _powerUpContainer;

    [SerializeField]
    private bool _stopSpawning = false;

    [Header("Testing")]
    public Vector3[] _positions;
    private int _positionSelector;
    private int _lastSelectedPosition;
    private int _initialSpawnPositionCount = 9;
    private Vector3 _lastPos;


    // Start is called before the first frame update
    void Start()
    {
        _waveManager = GetComponent<WaveManager>();
    }

    public void StartSpawning()
    {
        _stopSpawning = false;
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnEnemyRoutine()
    {

        int enemiesSpawned = 0;

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning != true)
        {
            if (enemiesSpawned != _waveManager.enemiesToSpawn)
            {
                if (_waveManager.currentWave < 5)
                {
                    GameObject newEnemy = Instantiate(_enemyPrefabs[0], RandomPos(), Quaternion.identity); //Spawning Enemy Here

                    newEnemy.transform.parent = _enemyContainer.transform;

                    _waveManager.enemiesLeft++;
                    enemiesSpawned++;

                }
                else if (_waveManager.currentWave >= 5)
                {
                    int randomEnemyID = Random.Range(0, 2); //Used for spawning random enemy from array

                    GameObject newEnemy = Instantiate(_enemyPrefabs[randomEnemyID], RandomPos(), Quaternion.identity); //Spawning Enemy Here

                    newEnemy.transform.parent = _enemyContainer.transform;

                    _waveManager.enemiesLeft++;
                    enemiesSpawned++;
                }
            }
            else
            {
                _waveManager.startOfWave = false;
                enemiesSpawned = 0;
                StopSpawning();
            }

            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(.5f);

        float randomTime = Random.Range(3f, 10f);


        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(randomTime);

            int randomPowerUpID = Random.Range(0, 6);
            GameObject newPowerUp = Instantiate(_powerUpPrefabs[randomPowerUpID], RandomPos(), Quaternion.identity);
            newPowerUp.transform.parent = _powerUpContainer.transform;
        }
    }

    Vector3 RandomPos()
    {
        Vector3 randomPos = Vector3.zero;

        _positionSelector = Random.Range(0, _positions.Length);

        for (int i = 0; i < _initialSpawnPositionCount; i++)
        {
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

            //Debug.Log(randomPos);

            //transform.position = randomPos;
        }

        return randomPos;
    }
}
