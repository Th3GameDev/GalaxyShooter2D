using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuidedLaserRadius : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    private List<Transform> _enemies = new List<Transform>();

    public Transform closestTarget;

    public bool inRadius;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inRadius == true)
        {
           closestTarget = GetClosestEnemy(_enemies);
        }
    }

    Transform GetClosestEnemy(List<Transform> targets)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = _player.transform.position;
        foreach (Transform potentialTarget in targets)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            inRadius = true;

            //if the object is not already in the list
            if (!_enemies.Contains(col.transform))  
            {
                //add the object to the list
                _enemies.Add(col.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            inRadius = false;

            //if the object is in the list
            if (_enemies.Contains(col.transform))
            {
                //remove it from the list
                _enemies.Remove(col.transform);
            }
        }
    }
}
