﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Player Laser")]
    [SerializeField]
    [Range(0f, 10f)]
    private float _LaserSpeed = 8f;

    private float _boundaryY = 5.5f;

    [Header("Enemy Laser")]
    [SerializeField]
    private bool isEnemyLaser = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemyLaser == false)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
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


    void MoveDown()
    {
        transform.Translate(Vector3.down * _LaserSpeed * Time.deltaTime);

        if (transform.position.y <= -_boundaryY)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        isEnemyLaser = true;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && isEnemyLaser == true)
        {
            Player player = col.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();

                Destroy(this.gameObject);
            }
        }
    }
}
