using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
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

    private Transform targetGem;
    public float speed = 0.1f;
    public float speedWithGem = 0.1f;
    public float fleeSpeed = 4f;
    public float wanderDistance = 3f;
    public float gemPickupRange = 0.5f;

    public float fleeDistance = 20f;

    // private Rigidbody rb;
    public Transform gemHoldPoint;

    public GemScript holdingGem;
    private bool isHoldingGem = false;
    public Transform spiderHole; // Reference to the spider hole where the gem will be dropped

    private GameObject[] torches = Array.Empty<GameObject>(); // Array to hold all torch objects in the scene

    public float torchEscapeDistance = 5f; // Distance at which the spider will escape from the torch

    private bool avoidingTorch = false; // Flag to check if the spider is currently avoiding a torch

    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component for pathfinding
    private NavMeshPath path;
    private Animator animator;

    public SpiderState CurrentState = SpiderState.Idle;


    private float idleTimer = 0f;
    private float fleeTimer = 5f;

    public GameObject gemPrefab; // Reference to the gem prefab for spawning

    void Start()
    {
        var holes = FindObjectsByType<SpiderHole>(FindObjectsSortMode.None);

        spiderHole = holes[Random.Range(0, holes.Length)].transform; // Find hole

        // rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        // Start checking for torches
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
                    StartFlee(torch?.transform);
                    break;
                }
            }
        }

        if (CurrentState == SpiderState.FleeingTorch)
        {
            navMeshAgent.speed = fleeSpeed;
        }
        else if (isHoldingGem)
        {
            navMeshAgent.speed = speedWithGem;
        }
        else
        {
            navMeshAgent.speed = speed;
        }

        if (animator != null)
        {
            if (HasPathEnded())
            {
                animator.SetBool("Moving", false);
            }
            else
            {
                animator.SetBool("Moving", true);
            }
        }

        if (CurrentState == SpiderState.Idle)
        {
            Idle();
        }
        else if (CurrentState == SpiderState.ChasingGem)
        {
            ChasingGem();
        }
        else if (CurrentState == SpiderState.CarryingGem)
        {
            CarryingGem();
        }
        else if (CurrentState == SpiderState.FleeingTorch)
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

        if (Random.Range(0, 1f) < 0.1f)
        {
            FindTargetGem();
        }
        else
        {
            navMeshAgent.destination = GetRandomNearTargetPosition();
            idleTimer = Random.Range(1f, 10f);
        }
    }

    private void ChasingGem()
    {
        if (!targetGem)
        {
            CurrentState = SpiderState.Idle;
            return;
        }

        if (targetGem.GetComponent<GemScript>()?.isCarried ?? false)
        {
            CurrentState = SpiderState.Idle;
            targetGem = null;
            return;
        }

        if (HasPathEnded())
        {
            navMeshAgent.destination = targetGem.transform.position;
            Debug.Log(
                $"Chasing Gem ended Distance: {Vector3.Distance(transform.position, targetGem.transform.position)}, Range: {gemPickupRange}");
        }

        if (Vector3.Distance(transform.position, targetGem.transform.position) < gemPickupRange)
        {
            if (targetGem.TryGetComponent<SackScript>(out SackScript sack))
            {
                Debug.Log("Stealing from sack");
                sack.RemoveGem();
                var gem = Instantiate(gemPrefab);
                GrabGem(gem.GetComponent<GemScript>());
            }
            else
            {
                GrabGem(targetGem.GetComponent<GemScript>());
            }
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
            Debug.Log("FleeingTorch ended");
            CurrentState = SpiderState.Idle;
            idleTimer = 5f;
        }
    }

    private void StartFlee(Transform torch)
    {
        Debug.Log("Fleeeee");
        Vector3 fleeDirection = Vector3.Scale((transform.position - torch.position), new Vector3(1, 0, 1)).normalized;

        float distance = fleeDistance;
        while (distance > 0f)
        {
            Vector3 pos = transform.position + fleeDirection * distance;
            if (navMeshAgent.SetDestination(pos)) // Move away from the torch
                break;
            distance -= 1f;
        }

        CurrentState = SpiderState.FleeingTorch;

        // Rotate away from the torch
        // Quaternion lookRotation = Quaternion.LookRotation(fleeDirection);
        // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (isHoldingGem)
        {
            DropGem();
        }
    }

    private void GrabGem(GemScript gem)
    {
        Debug.Log("Grabbing Gem");
        if (gem.isCarried)
        {
            Debug.Log("Already Carrying");
            return;
        }

        if (isHoldingGem)
        {
            Debug.Log("Already Holding");
            return; // If already holding a gem, do nothing
        }

        isHoldingGem = true;
        gem.transform.parent = gemHoldPoint;
        gem.GetComponent<Rigidbody>().isKinematic = true;
        gem.GetComponent<Collider>().enabled = false;
        gem.transform.position = gemHoldPoint.position;
        holdingGem = gem;
        gem.isCarried = true;

        CurrentState = SpiderState.CarryingGem;
        Debug.Log("Now carrying.");
    }

    private void DropGem()
    {
        holdingGem.transform.parent = null;
        holdingGem.GetComponent<Rigidbody>().isKinematic = false;
        holdingGem.GetComponent<Collider>().enabled = true;
        holdingGem.isCarried = false;

        isHoldingGem = false;
        holdingGem = null;
    }

    private Vector3 GetRandomNearTargetPosition()
    {
        Vector2 dir = Random.insideUnitCircle * wanderDistance;
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
            yield return new WaitForSeconds(.5f);

            torches = GameObject.FindGameObjectsWithTag("Torch");
        }
    }

    private void FindTargetGem()
    {
        if (!targetGem)
        {
            List<GameObject> gems = GameObject.FindGameObjectsWithTag("Gem").ToList();

            gems = gems.Where(g => !g.GetComponent<GemScript>().isCarried).ToList();
            SackScript sackScript = FindObjectOfType<SackScript>();
            if (!PlayerScript.Instance.carryingSack && sackScript.gems > 0)
            {
                gems.Add(sackScript.gameObject);
                Debug.Log("Found Sack with Gems");
            }
            if (gems.Count > 0)
            {
                targetGem = gems[Random.Range(0, gems.Count)].transform;
                CurrentState = SpiderState.ChasingGem;
            }
        }
    }
}