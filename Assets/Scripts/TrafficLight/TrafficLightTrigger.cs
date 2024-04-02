using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightTrigger : MonoBehaviour
{
    public GameObject trafficLightPole;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trafficLightPole.GetComponent<TrafficLightController>().StartColorChange();
            Debug.Log("Started the color change.");
        }
    }
}
