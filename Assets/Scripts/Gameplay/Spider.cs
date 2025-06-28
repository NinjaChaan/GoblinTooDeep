using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Spider : MonoBehaviour
{
    private GameObject targetGem;
    public float speed = 0.1f;
    public float speedWithGem = 0.1f;

    // private Rigidbody rb;
    public Transform gemHoldPoint;

    private bool isHoldingGem = false;
    public Transform spiderHole; // Reference to the spider hole where the gem will be dropped

    private GameObject[] torches = Array.Empty<GameObject>(); // Array to hold all torch objects in the scene

    public float torchEscapeDistance = 5f; // Distance at which the spider will escape from the torch

    private bool avoidingTorch = false; // Flag to check if the spider is currently avoiding a torch

    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component for pathfinding
    private NavMeshPath path;

    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Start checking for gems in the scene
        StartCoroutine(CheckForGems());
        StartCoroutine(CheckForTorches());
    }

    // Update is called once per frame
    void Update()
    {
        avoidingTorch = false; // Reset the avoidingTorch flag at the start of each update
        if (torches.Length > 0)
        {
            foreach (var torch in torches)
            {
                if (Vector3.Distance(torch.transform.position, transform.position) < torchEscapeDistance)
                {
                    avoidingTorch = true; // Set the flag to indicate the spider is avoiding a torch
                    // If the spider is too close to a torch, escape in a random direction
                    Vector3 escapeDirection = (transform.position - torch.transform.position).normalized;
                    //rb.MovePosition(transform.position + escapeDirection * (speed * Time.deltaTime));
                    // Rotate away from the torch
                    Quaternion lookRotation = Quaternion.LookRotation(escapeDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                    break;
                }
            }
        }

        if (targetGem && !isHoldingGem)
        {
            navMeshAgent.destination = targetGem.transform.position;
            // Move towards the target gem
            Vector3 direction = (targetGem.transform.position - transform.position).normalized;
            // rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
            //navMeshAgent.Move(direction * (speed * Time.deltaTime));
            // Rotate towards the target gem
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (isHoldingGem)
        {
            navMeshAgent.destination = spiderHole.position;
            //Move towards spiderhole
            Vector3 direction = (spiderHole.position - transform.position).normalized;
            // navMeshAgent.Move(direction * (speed * Time.deltaTime));
            // rb.MovePosition(transform.position + direction * (speedWithGem * Time.deltaTime));
            // Rotate towards the spider hole
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Gem"))
        {
            if (isHoldingGem)
            {
                return; // If already holding a gem, do nothing
            }
            isHoldingGem = true;
            targetGem.transform.parent = transform;
            // targetGem.GetComponent<Rigidbody>().isKinematic =
            //     true; // Make the gem kinematic to stop physics interactions
            targetGem.GetComponent<Collider>().enabled = false; // Disable collider for the gem
            targetGem.transform.position = gemHoldPoint.position;
        }
    }

    private IEnumerator CheckForTorches()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);

            torches = GameObject.FindGameObjectsWithTag("Torch");
        }
    }

    private IEnumerator CheckForGems()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!targetGem)
            {
                GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
                if (gems.Length > 0)
                {
                    targetGem = gems[Random.Range(0, gems.Length)];
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("SpiderHole"))
        {
            if (isHoldingGem)
            {
                Destroy(gameObject); // Destroy the spider when it reaches the hole}
            }
        }
    }
}