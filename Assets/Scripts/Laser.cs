using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 10f)]
    private float _LaserSpeed = 8f;

    private float _boundaryY = 5.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _LaserSpeed * Time.deltaTime);
      
        if (transform.position.y >= _boundaryY)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }   
    }
}
