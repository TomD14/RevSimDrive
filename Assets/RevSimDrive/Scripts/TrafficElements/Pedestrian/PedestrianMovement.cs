using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianMovement : MonoBehaviour
{
    public List<Transform> waypoints;

    [SerializeField] private float movespeed = 3f;

    [SerializeField] private float distanceThreshold = 0.1f;

    private Transform targetWaypoint;
    private int count = 1;

    void Start()
    {
        targetWaypoint = waypoints[count].transform;
        transform.LookAt(waypoints[count].transform);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, movespeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetWaypoint.position) < distanceThreshold)
        {
            if (count + 1 !< waypoints.Count)
            {
                count++;
                targetWaypoint = waypoints[count];
                transform.LookAt(waypoints[count].transform);
            }
            else
            {
                count = 0;
                targetWaypoint = waypoints[count];
                transform.LookAt(waypoints[count].transform);
            }
        }

    }

    private void CrossTheRoad()
    {

    }
}
