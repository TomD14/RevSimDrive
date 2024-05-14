using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCheckBox : MonoBehaviour
{
    private float count = 0;
    private List<Light> lights = new List<Light>();

    private Crossroad crossroad;

    private void Start()
    {
        Transform lane = transform.parent.parent;
        crossroad = transform.parent.parent.parent.GetComponent<Crossroad>();

        for (int i = 0; i < lane.childCount; i++)
        {
            for(int j = 0; j < lane.GetChild(i).childCount; j++)
            {
                for (int z = 0; z < lane.GetChild(i).GetChild(j).childCount; z++)
                {
                    if (lane.GetChild(i).GetChild(j).GetChild(z).tag != "TrafficLightCollider")
                    {
                        lights.Add(lane.GetChild(i).GetChild(j).GetChild(z).GetComponent<Light>());
                    }
                }

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (lights.Count != 0 && crossroad.currentGreen == "" && crossroad.previousLane != transform.name)
        {
            crossroad.TrafficLightActivation(lights, transform.name);
        }
    }
}

