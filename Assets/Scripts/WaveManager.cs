using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private SpawnManager _spawnManager;

    private UIManager _uiManager;

    public int currentWave = 1;

    public int enemiesToSpawn = 5;

    public int enemiesLeft = 0;

    public bool startOfWave;

    public bool bossWave = false;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("Game Canvas is Null!!");
        }

        _spawnManager = GetComponent<SpawnManager>();
    }


    void Update()
    {
        if (enemiesLeft <= 0 && startOfWave != true)
        {
            enemiesLeft = 0;
            EndWave();
        }
        else if (enemiesLeft <= 0 && currentWave == 10)
        {
            Debug.Log("GameOver");
        }
    }

    public void StartWave()
    {
        startOfWave = true;
        StartCoroutine(StartWaveRoutine());
    }

    public void EndWave()
    {
        StartCoroutine(EndWaveRoutine());
    }

    IEnumerator StartWaveRoutine()
    {
        _uiManager.UpdateWaveStartDisplay(currentWave);

        yield return new WaitForSeconds(3f);

        if (enemiesLeft != enemiesToSpawn)
        {
            _spawnManager.StartSpawning();
        }
    }

    IEnumerator EndWaveRoutine()
    {
        currentWave++;

        if (currentWave >= 10)
        {          
            _spawnManager.stopSpawningPowerUp = true;
            bossWave = true;
            startOfWave = true;           
            enemiesToSpawn = 1;
            yield return new WaitForSeconds(2.5f);
            StartWave();
        }
        else if (currentWave < 10)
        {          
            _spawnManager.stopSpawningPowerUp = true;
            startOfWave = true;         
            enemiesToSpawn += 1;
            yield return new WaitForSeconds(2.5f);
            StartWave();
        }
    }
}
