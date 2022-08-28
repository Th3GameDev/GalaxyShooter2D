using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private SpawnManager _spawnManager;

    [SerializeField]
    private GameObject exploPrefab;

    [SerializeField]
    private float rotSpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogWarning("Spawn Manager is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {     
        transform.Rotate(new Vector3(0, 0, -1 * rotSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
       if (col.gameObject.CompareTag("Laser"))
       {
            Instantiate(exploPrefab, gameObject.transform.position, Quaternion.identity);

            Destroy(col.gameObject);

            _spawnManager.StartSpawning();

            Destroy(gameObject);
       }
    }
}
