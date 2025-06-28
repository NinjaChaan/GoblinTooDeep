using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Spider : MonoBehaviour
{
    public enum SpiderState
    {
        Idle,
        ChasingGem,
        CarryingGem,
        FleeingTorch
    }
    
    private GameObject targetGem;
    public float speed = 0.1f;
    public float speedWithGem = 0.1f;
    public float wanderDistance = 3f;
    public float gemPickupRange = 0.3f;

    // private Rigidbody rb;
    public Transform gemHoldPoint;

    public GameObject holdingGem;
    private bool isHoldingGem = false;
    public Transform spiderHole; // Reference to the spider hole where the gem will be dropped

    private GameObject[] torches = Array.Empty<GameObject>(); // Array to hold all torch objects in the scene

    public float torchEscapeDistance = 5f; // Distance at which the spider will escape from the torch

    private bool avoidingTorch = false; // Flag to check if the spider is currently avoiding a torch

    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component for pathfinding
    private NavMeshPath path;
    
    public SpiderState CurrentState = SpiderState.Idle;


    private float idleTimer = 0f;
    private float fleeTimer = 5f;
    
    void Start()
    {
        var holes = FindObjectsByType<SpiderHole>(FindObjectsSortMode.None);
        
        spiderHole = holes[Random.Range(0, holes.Length)].transform; // Find hole
        
        // rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Start checking for gems in the scene
        StartCoroutine(CheckForTorches());
        
        CurrentState = SpiderState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            if (!NavMesh.FindClosestEdge(transform.position, out var hit, NavMesh.AllAreas))
            {
                Debug.LogError("No NavMesh found!");
                return;
            }
            navMeshAgent.Warp(hit.position);
        }
        
        if (torches.Length > 0 && CurrentState != SpiderState.FleeingTorch)
        {
            foreach (var torch in torches)
            {
                if (Vector3.Distance(torch.transform.position, transform.position) < torchEscapeDistance)
                {
                    StartFlee(torch.transform);
                    break;
                }
            }
        }
        
        if (isHoldingGem)
        {
            navMeshAgent.speed = speedWithGem;
        } else
        {
            navMeshAgent.speed = speed;
        }

        if (CurrentState == SpiderState.Idle)
        {
            Idle();
        }else if (CurrentState == SpiderState.ChasingGem)
        {
            ChasingGem();
        } else if (CurrentState == SpiderState.CarryingGem)
        {
            CarryingGem();
        } else if (CurrentState == SpiderState.FleeingTorch)
        {
            FleeingTorch();
        }
    }

    private void Idle()
    {
        if (idleTimer > 0f)
        {
            idleTimer -= Time.deltaTime;
            return;
        }

        if (isHoldingGem)
        {
            CurrentState = SpiderState.CarryingGem;
            return;
        }
        
        if (Random.Range(0, 1f) < 0.3f)
        {
            FindTargetGem();
        }
        else
        {
            navMeshAgent.destination = GetRandomNearTargetPosition();
            idleTimer = Random.Range(1f, 5f);
        }
    }

    private void ChasingGem()
    {
        if (!targetGem)
        {
            CurrentState = SpiderState.Idle;
            return;
        }
        if (HasPathEnded())
        {
            navMeshAgent.destination = targetGem.transform.position;
        }

        if (Vector3.Distance(transform.position, targetGem.transform.position) < gemPickupRange)
        {
            GrabGem(targetGem);
        }
    }
    
    private void CarryingGem()
    {
        if (!holdingGem || !isHoldingGem)
        {
            CurrentState = SpiderState.Idle;
            return;
        }
        if (HasPathEnded())
        {
            navMeshAgent.destination = spiderHole.position;
        }
        
        if (Vector3.Distance(transform.position, spiderHole.position) < gemPickupRange)
        {
            Destroy(gameObject);
        }
    }
    
    private void FleeingTorch()
    {
        if (HasPathEnded())
        {
            CurrentState = SpiderState.Idle;
            idleTimer = 5f;
        }
    }

    private void StartFlee(Transform torch)
    {
        Vector3 fleeDirection = (transform.position - torch.position).normalized;
        navMeshAgent.SetDestination(transform.position + fleeDirection * 10f); // Move away from the torch
        
        CurrentState = SpiderState.FleeingTorch;
        
        // Rotate away from the torch
        // Quaternion lookRotation = Quaternion.LookRotation(fleeDirection);
        // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (isHoldingGem)
        {
            DropGem();
        }
    }

    private void GrabGem(GameObject gem)
    {
        if (isHoldingGem)
        {
            return; // If already holding a gem, do nothing
        }
        isHoldingGem = true;
        targetGem.transform.parent = gemHoldPoint;
        targetGem.GetComponent<Rigidbody>().isKinematic = true;
        targetGem.GetComponent<Collider>().enabled = false;
        targetGem.transform.position = gemHoldPoint.position;
        holdingGem = gem;

        CurrentState = SpiderState.CarryingGem;
    }

    private void DropGem()
    {
        holdingGem.transform.parent = null;
        holdingGem.GetComponent<Rigidbody>().isKinematic = false;
        holdingGem.GetComponent<Collider>().enabled = true;
        
        isHoldingGem = false;
        holdingGem = null;
    }

    private Vector3 GetRandomNearTargetPosition()
    {
        Vector2 dir = Random.insideUnitCircle* wanderDistance;
        Vector3 randomPos = transform.position + new Vector3(dir.x, 0, dir.y);
        return randomPos;
    }

    private bool HasPathEnded()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    private IEnumerator CheckForTorches()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);

            torches = GameObject.FindGameObjectsWithTag("Torch");
        }
    }

    private void FindTargetGem()
    {
        if (!targetGem)
        {
            GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
            if (gems.Length > 0)
            {
                targetGem = gems[Random.Range(0, gems.Length)];
                CurrentState = SpiderState.ChasingGem;
            }
        }
    }
}