using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossroad : MonoBehaviour
{
    private List<List<Transform>> lanes = new List<List<Transform>>();
    public string currentGreen = "";
    public string previousLane = "none";

    private Coroutine cooldown;

    public GameObject trafficLightController;

    private float greenTime;
    private float yellowTime;
    private float redCooldown;
    private float backToBackCooldown;

    // Start is called before the first frame update
    void Start()
    {
        greenTime = trafficLightController.GetComponent<TrafficLightController>().GreenTime;
        yellowTime = trafficLightController.GetComponent<TrafficLightController>().YellowTime;
        redCooldown = trafficLightController.GetComponent<TrafficLightController>().Cooldown;
        backToBackCooldown = trafficLightController.GetComponent<TrafficLightController>().BackToBackCooldown + greenTime + yellowTime + redCooldown;

        for (int i = 0; i < transform.childCount; i++)
        {
            string s = transform.GetChild(i).name;
            List<Transform> toAdd = new List<Transform>();
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                for (int x = 0; x < transform.GetChild(i).GetChild(j).childCount; x++)

                    toAdd.Add(transform.GetChild(i).GetChild(j).GetChild(x));

            }
            lanes.Add(toAdd);
        }

        Debug.Log(lanes);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TrafficLightActivation(List<Light> lights, string laneName)
    {

        if (cooldown != null)
        {
            StopCoroutine(cooldown);
        }


        currentGreen = laneName;
        previousLane = currentGreen;

        for (int z = 0; z < lights.Count; z++)
        {
            StartCoroutine(TrafficLightCycle(lights));
        }

        cooldown = StartCoroutine(BackToBackCooldown());

    }

    IEnumerator TrafficLightCycle(List<Light> t)
    {
        foreach (Light light in t)
        {

            if (light.name == "GreenLight")
            {
                light.enabled = true;
            }
            else
            {
                light.enabled = false;
            }
        }
        yield return new WaitForSeconds(greenTime);

        foreach (Light light in t)
        {

            if (light.name == "YellowLight")
            {
                light.enabled = true;
            }
            else
            {
                light.enabled = false;
            }

        }
        yield return new WaitForSeconds(yellowTime);

        foreach (Light light in t)
        {
            if (light.name == "RedLight")
            {
                light.enabled = true;
            }
            else
            {
                light.enabled = false;
            }

        }
        yield return new WaitForSeconds(redCooldown);

        currentGreen = "";

        yield return null;
    }

    IEnumerator BackToBackCooldown()
    {
        Debug.Log("Starting cooldown");
        yield return new WaitForSeconds(backToBackCooldown);

        previousLane = "none";

        Debug.Log("Ending cooldown");
        yield return null;
    }
}
