using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DriverController : MonoBehaviour
{
    public TMP_Text Counter;
    public GameObject Driver;

    private float count = 0;
    private Transform driverObjectChild;
    private List<GameObject> drivers = new List<GameObject>();
    private List<Transform> driverWaypoints = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        driverObjectChild = transform.GetChild(transform.childCount - 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RemoveDriver()
    {
        if (drivers.Count != 0)
        {
            Destroy(drivers[drivers.Count - 1]);
            drivers.RemoveAt(drivers.Count - 1);
            count--;
            Counter.text = count.ToString();
        }
    }

    public void AddDriver()
    {
        driverWaypoints.Clear();

        int RandomlLoc = Random.Range(0, transform.childCount - 1);
        Transform spawnPath = transform.GetChild(RandomlLoc);

        for (int i = 0; i < spawnPath.childCount; i++)
        {
            driverWaypoints.Add(spawnPath.GetChild(i));
        }

        if (driverWaypoints.Count != 0)
        {
            Vector3 chosenSpawnpoint = driverWaypoints[0].position;
            GameObject driverSpawned = Instantiate(Driver, chosenSpawnpoint, Quaternion.identity, driverObjectChild);
            drivers.Add(driverSpawned);

            DriverMovement driverMovement = driverSpawned.GetComponent<DriverMovement>();

            driverMovement.waypointParent = spawnPath;
        }

        count++;
        Counter.text = count.ToString();
    }
}
