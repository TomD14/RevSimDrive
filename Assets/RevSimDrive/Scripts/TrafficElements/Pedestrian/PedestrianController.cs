using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianController : MonoBehaviour
{
    public GameObject pedestrian;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnPedestrian()
    {
        int randomWaypointGroup;
        randomWaypointGroup = Random.Range(0, transform.childCount);

        Debug.Log("Amount of waypoint groups = " + transform.childCount + ", Random number chosen = " + randomWaypointGroup);

        int randomWaypointInsideGroup;
        randomWaypointInsideGroup = Random.Range(0, transform.GetChild(randomWaypointGroup).childCount);

        Debug.Log("Amount of waypoints inside chosen group = " + transform.GetChild(randomWaypointGroup).childCount + ", Random number chosen = " + randomWaypointInsideGroup);

        Vector3 chosenSpawnpoint = transform.GetChild(randomWaypointGroup).GetChild(randomWaypointInsideGroup).position;

        GameObject pedestrianSpawned = Instantiate(pedestrian, chosenSpawnpoint, Quaternion.identity);

        PedestrianMovement pedMov = pedestrianSpawned.GetComponent<PedestrianMovement>();

        int j = 0;
        for (int i = 0; i < transform.GetChild(randomWaypointGroup).childCount; i++)
        {
            if (i + randomWaypointInsideGroup < transform.GetChild(randomWaypointGroup).childCount)
            {
                pedMov.waypoints.Add(transform.GetChild(randomWaypointGroup).GetChild(randomWaypointInsideGroup + i));
            }
            else
            {
                pedMov.waypoints.Add(transform.GetChild(randomWaypointGroup).GetChild(j));
                j++;
            }
        }
    }
}
