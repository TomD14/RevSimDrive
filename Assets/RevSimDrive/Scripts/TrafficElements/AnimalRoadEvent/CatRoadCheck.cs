using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatRoadCheck : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 1f;
    private Animator animator;
    private bool isWalking = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if(other.tag == "Border")
        {
            StopMoving();
            Debug.Log("Stopeed walking");
        }
    }

    public void MoveForward()
    {
        animator.SetTrigger("Walk");
        isWalking = true;
    }

    private void StopMoving()
    {
        animator.SetTrigger("Idle");
        isWalking = false;
        Vector3 newPosition = transform.position - transform.forward * 0.2f;
        transform.position = newPosition;

        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * 180);
        transform.rotation = targetRotation;
    }
}
