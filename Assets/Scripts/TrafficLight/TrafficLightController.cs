using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public Material green;
    public Material red;

    public GameObject light;

    public float changeCountdown = 5f;
    public float reverseCountdown = 5f;

    private bool traffickStarted = false;

    void Start()
    {

    }

    public void StartColorChange()
    {
        if (!traffickStarted)
        {
            traffickStarted = true;
            StartCoroutine(ChangeColorRoutine());
        }
    }

    IEnumerator ChangeColorRoutine()
    {
        yield return new WaitForSeconds(changeCountdown);

        light.GetComponent<Renderer>().material = green;

        yield return new WaitForSeconds(reverseCountdown);

        light.GetComponent<Renderer>().material = red;
        traffickStarted = false;
    }

}
