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
    private bool rotating = false;

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

                StartCoroutine(CrossTheRoad(waypoints[count -1], waypoints[count], 1f));
            }
            else
            {
                count = 0;
                targetWaypoint = waypoints[count];
                transform.LookAt(waypoints[count].transform);
                StartCoroutine(CrossTheRoad(waypoints[count - 1], waypoints[count], 1f));
            }
        }

    }

    IEnumerator CrossTheRoad(Transform current, Transform target, float duration)
    {
        if(target.tag == "CrossingWaypoint" && current.tag == "CrossingWaypoint")
        {
            float oldMoveSpeed = movespeed;
            movespeed = 0f;
            Quaternion baseRot = transform.rotation;
            Quaternion newRotR = baseRot  * Quaternion.Euler(new Vector3(0, 90, 0));
            Quaternion newRotL = baseRot * Quaternion.Euler(new Vector3(0, -90, 0));

            //LookRight
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(baseRot, newRotR, counter / duration);
                yield return null;
            }

            //LookLeft
            float counter2a = 0;
            while (counter2a < duration)
            {
                counter2a += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(newRotR, baseRot, counter2a / duration);
                yield return null; 
            }


            float counter2b = 0;
            while (counter2b < duration)
            {
                counter2b += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(baseRot, newRotL, counter2b / duration);
                yield return null;
            }


            //LookForward
            float counter3 = 0;
            while (counter3 < duration)
            {
                counter3 += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(newRotL, baseRot, counter3/ duration);
                yield return null;
            }

            movespeed = oldMoveSpeed;
        }
    }
}
