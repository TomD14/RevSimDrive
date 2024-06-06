using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEvent : MonoBehaviour
{
    [SerializeField] public GameObject Cat;
    private CatRoadCheck catScript;


    // Start is called before the first frame update
    void Start()
    {
        if (Cat != null)
        {
            catScript =  Cat.GetComponent<CatRoadCheck>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            catScript.MoveForward();
            Debug.Log("Animal detected player!");
        }
    }

    
}
