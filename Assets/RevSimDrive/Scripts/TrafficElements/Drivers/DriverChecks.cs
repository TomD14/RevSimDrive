using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverChecks : MonoBehaviour
{
    private List<GameObject> objectsInTrigger = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to check if there are any objects in the trigger
    public bool IsTriggerBoxContainingObjects()
    {
        return objectsInTrigger.Count > 0;
    }

    // Called when an object enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("aids");
            objectsInTrigger.Add(other.gameObject);
        }
    }

    // Called when an object exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            objectsInTrigger.Remove(other.gameObject);
        }
    }
}
