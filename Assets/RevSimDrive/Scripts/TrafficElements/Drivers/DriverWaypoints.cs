using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverWaypoints : MonoBehaviour
{
    [Range(0.1f, 2f)]
    [SerializeField] private float waypointSize = 1f;

    public Transform next;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, waypointSize);

        if (transform.tag == "DriveTurnPoint")
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, waypointSize);
        }
        else if (transform.tag == "DriveTurnEndPoint")
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, waypointSize);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, next.position);


    }
}
