using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Spawning")]
    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _enemyContainer;

    [Header("PowerUp Spawning")]
    [SerializeField]
    private GameObject[] _powerUpPrefabs;

    [SerializeField]
    private GameObject _powerUpContainer;

    private bool _stopSpawning = false;


    [Header("Testing")]
    public Vector3[] _positions;
    private int _positionSelector;
    private int _lastSelectedPosition;
    private int _initialSpawnPositionCount = 9;
    //Vector3 posTemp;
    Vector3 _lastPos;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Pos selected is" + RandomPos());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        //Vector3 randomPos = new Vector3(Random.Range(-8f, 8f), 7f, 0f);

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            GameObject newEnemy = Instantiate(_enemyPrefab, RandomPos(), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        //float randomTime = Random.Range(10f, 20f);

        yield return new WaitForSeconds(.5f);

        float randomTime = Random.Range(3f, 10f);


        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(randomTime);

            int randomPowerUpID = Random.Range(0, 4);
            GameObject newPowerUp = Instantiate(_powerUpPrefabs[randomPowerUpID], RandomPos(), Quaternion.identity);
            newPowerUp.transform.parent = _powerUpContainer.transform;
        }
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
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
