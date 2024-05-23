using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DrawPath : MonoBehaviour
{
    public Transform player;
    public Transform goal;
    public Material lineMaterial;
    public Color lineColor = Color.green;

    private LineRenderer lineRenderer;
    private NavMeshPath navMeshPath;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        navMeshPath = new NavMeshPath();
        lineRenderer.positionCount = 0;

        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.agentTypeID = NavMesh.GetSettingsByIndex(NavMesh.GetSettingsCount() - 1).agentTypeID;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    void Update()
    {
        if (player != null && goal != null)
        {
            navMeshAgent.Warp(player.position); 

            if (navMeshAgent.CalculatePath(goal.position, navMeshPath))
            {
                if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    lineRenderer.positionCount = navMeshPath.corners.Length;
                    lineRenderer.SetPositions(navMeshPath.corners);
                }
                else
                {
                    lineRenderer.positionCount = 0;
                }
            }
        }
    }
}
