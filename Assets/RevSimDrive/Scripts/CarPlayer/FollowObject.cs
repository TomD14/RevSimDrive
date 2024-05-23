using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target; // The object to follow
    public Vector3 offset;   // Offset from the target object

    void LateUpdate()
    {
        if (target != null && target.parent != null)
        {
            // Update the camera's position to follow the target's parent position with an offset
            transform.position = target.parent.position + offset;
        }
    }
}
