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
        startOfWave = true;
        currentWave++;
        enemiesToSpawn += 5;
        yield return new WaitForSeconds(2.5f);
        StartWave();
    }
}
