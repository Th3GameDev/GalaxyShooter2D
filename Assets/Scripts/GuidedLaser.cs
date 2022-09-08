using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedLaser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 5.0f;

    private GuidedLaserRadius _laserRadius;

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        _laserRadius = GameObject.Find("LaserRadius").GetComponent<GuidedLaserRadius>();
    }

    // Update is called once per frame
    void Update()
    {
        target = _laserRadius.closestTarget;

        if (target != null)
        {   
            MoveTowardsTarget(target);     
        }
    }


    void MoveTowardsTarget(Transform target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, _laserSpeed * Time.deltaTime);
        transform.up = target.transform.position - transform.position;
    }
}
