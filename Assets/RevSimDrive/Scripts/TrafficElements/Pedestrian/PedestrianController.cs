using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PedestrianController : MonoBehaviour
{
    public GameObject pedestrian;
    public TMP_Text counter;
    public float count = 0;

    private List<GameObject> pedestrians = new List<GameObject>();
    private Transform pedestriansObjectChild;

    // Start is called before the first frame update
    void Start()
    {
        pedestriansObjectChild = transform.GetChild(transform.childCount - 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DeletePedestrian()
    {
        if(pedestrians.Count != 0)
        {
            Destroy(pedestrians[pedestrians.Count - 1]);
            pedestrians.RemoveAt(pedestrians.Count - 1);
            count--;
            counter.text = count.ToString();
        }
    }

    public void SpawnPedestrian()
    {
        int randomWaypointGroup = Random.Range(0, transform.childCount);

        Debug.Log("Amount of waypoint groups = " + transform.childCount + ", Random number chosen = " + randomWaypointGroup);

        int randomWaypointInsideGroup = Random.Range(0, transform.GetChild(randomWaypointGroup).childCount);

        Debug.Log("Amount of waypoints inside chosen group = " + transform.GetChild(randomWaypointGroup).childCount + ", Random number chosen = " + randomWaypointInsideGroup);

        Vector3 chosenSpawnpoint = transform.GetChild(randomWaypointGroup).GetChild(randomWaypointInsideGroup).position;

        GameObject pedestrianSpawned = Instantiate(pedestrian, chosenSpawnpoint, Quaternion.identity, pedestriansObjectChild);

        pedestrians.Add(pedestrianSpawned);

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

        count++;
        counter.text = count.ToString();
    }
}
