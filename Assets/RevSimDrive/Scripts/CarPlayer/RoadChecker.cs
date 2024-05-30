using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadChecker : MonoBehaviour
{
    public Transform SpawnPointsParent;
    public ScreenFader screenFader;

    private float roadChecker = 0;
    private GameObject carPlayer;
    private CarController carController;
    private List<Transform> spawnPoints = new List<Transform>();
    private List<Transform> pointTowardsPoints = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        carPlayer = transform.parent.gameObject;
        carController = carPlayer.GetComponent<CarController>();

        for(int i = 0; i < SpawnPointsParent.childCount; i++)
        {
            spawnPoints.Add(SpawnPointsParent.GetChild(i));
            pointTowardsPoints.Add(SpawnPointsParent.GetChild(i).GetChild(0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Road")
        {
            roadChecker++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Border" && roadChecker == 0)
        {
            StartCoroutine(TeleportCar());
        }

        if (other.tag == "Road")
        {
            roadChecker--;
        }
    }

    private IEnumerator TeleportCar()
    {
        Transform closestPoint = null;
        Transform closestPointTowardsPoint = null;

        for (int t = 0; t < spawnPoints.Count; t++)
        {
            if (closestPoint != null)
            {
                float distance = Vector3.Distance(spawnPoints[t].position, carPlayer.transform.position);
                float closestDistance = Vector3.Distance(closestPoint.position, carPlayer.transform.position);

                if (distance < closestDistance)
                {
                    closestPoint = spawnPoints[t];
                    closestPointTowardsPoint = pointTowardsPoints[t];
                }
            }
            else
            {
                closestPoint = spawnPoints[t];
                closestPointTowardsPoint = pointTowardsPoints[t];
            }
        }

        // Start fade in
        yield return StartCoroutine(screenFader.FadeIn());

        carController.StopCar();
        carPlayer.transform.position = closestPoint.position;
        carPlayer.transform.LookAt(closestPointTowardsPoint.position);

        // Start fade out
        yield return StartCoroutine(screenFader.FadeOut());
    }
}
