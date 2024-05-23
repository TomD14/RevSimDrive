using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianWaypoints : MonoBehaviour
{
    [Range(0.1f, 2f)]
    [SerializeField] private float waypointSize = 1f;

    private void OnDrawGizmos()
    {
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.blue;

            if (t.tag == "CrossingWaypoint")
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(t.position, waypointSize);
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.DrawWireSphere(t.position, waypointSize);
            }
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if(transform.GetChild(i).tag == "CrossingWaypoint" && transform.GetChild(i + 1).tag == "CrossingWaypoint")
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
            }
        }

        Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }
}
