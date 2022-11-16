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


    public bool stopSpawningPowerUp = false;


    [SerializeField]
    private int[] _powerUpTable = { 50, 30, 25, 20, 15, 10, 5 };

    [SerializeField]
    private int _powerUpTableTotal;

    [Header("Testing")]
    public Vector3[] _positions;
    private int _positionSelector;
    private int _lastSelectedPosition;
    private int _initialSpawnPositionCount = 9;
    private Vector3 _lastPos;


    private GameObject _selectedPowerUp;


    // Start is called before the first frame update
    void Start()
    {
        _waveManager = GetComponent<WaveManager>();
    }

    public void StartSpawning()
    {
        _stopSpawning = false;
        stopSpawningPowerUp = false;

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
                if (_waveManager.currentWave < 3)
                {                   
                    GameObject newEnemy = Instantiate(_enemyPrefabs[0], RandomPos(), Quaternion.identity); //Spawning Enemy Here

                    newEnemy.transform.parent = _enemyContainer.transform;

                    _waveManager.enemiesLeft++;

                    enemiesSpawned++;

                }
                else if (_waveManager.currentWave >= 3 && _waveManager.currentWave < 5 && _waveManager.bossWave == false)
                {                  
                    int randomEnemyID_2 = Random.Range(0, 2); //Used for spawning random enemy from array

                    GameObject newEnemy_2 = Instantiate(_enemyPrefabs[randomEnemyID_2], RandomPos(), Quaternion.identity); //Spawning Enemy Here

                    newEnemy_2.transform.parent = _enemyContainer.transform;

                    _waveManager.enemiesLeft++;

                    enemiesSpawned++;
                }
                else if (_waveManager.currentWave >= 5 && _waveManager.bossWave == false)
                {                  
                    int randomEnemyID_3 = Random.Range(0, 3); //Used for spawning random enemy from array

                    GameObject newEnemy_3 = Instantiate(_enemyPrefabs[randomEnemyID_3], RandomPos(), Quaternion.identity); //Spawning Enemy Here

                    newEnemy_3.transform.parent = _enemyContainer.transform;

                    _waveManager.enemiesLeft++;

                    enemiesSpawned++;
                }
                else if (_waveManager.currentWave == 10 && _waveManager.bossWave == true)
                {
                    GameObject bossAi = Instantiate(_enemyPrefabs[3], new Vector3(0, 8, 0), Quaternion.identity);
                    bossAi.transform.parent = _enemyContainer.transform;
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

        float randomTime = Random.Range(10f, 15f);


        while (stopSpawningPowerUp == false)
        {
            yield return new WaitForSeconds(randomTime);

            SelectPowerUp();

            if (_selectedPowerUp != null)
            {
                GameObject newPowerUp = Instantiate(_selectedPowerUp, RandomPos(), Quaternion.identity);

                newPowerUp.transform.parent = _powerUpContainer.transform;
            }
        }
    }

    void SelectPowerUp()
    {
        int a = 0;

        _powerUpTableTotal = 0;

        foreach (var powerUp in _powerUpTable)
        {
            _powerUpTableTotal += powerUp;
        }

        int randomNum = Random.Range(0, _powerUpTableTotal);

        foreach (var weight in _powerUpTable)
        {
            if (randomNum <= weight)
            {
                //Debug.Log("RandomNumber: " + randomNum + " is Less Than Equal to Weight: " + weight + " True");

                //Debug.Log("Spawn: " + _powerUpPrefabs[a]);

                _selectedPowerUp = _powerUpPrefabs[a];

                return;
            }
            else
            {
                a++;

                randomNum -= weight;

                //Debug.Log("RandomNum: " + randomNum + " Weight: " + weight + " False");

                if (a >= 8)
                {
                    a = 0;
                }
            }
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
