using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laserbeam : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Player player = col.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();

                //Destroy(this.gameObject);
            }
        }
    }
}
