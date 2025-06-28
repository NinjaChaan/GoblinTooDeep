using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spider : MonoBehaviour
{
    private GameObject targetGem;
    public float speed = 0.1f;
    public float speedWithGem = 0.1f;

    private Rigidbody rb;
    public Transform gemHoldPoint;

    private bool isHoldingGem = false;
    public Transform spiderHole; // Reference to the spider hole where the gem will be dropped

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Start checking for gems in the scene
        StartCoroutine(CheckForGems());
    }

    // Update is called once per frame
    void Update()
    {
        if (targetGem && !isHoldingGem)
        {
            // Move towards the target gem
            Vector3 direction = (targetGem.transform.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
            // Rotate towards the target gem
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (isHoldingGem)
        {
            //Move towards spiderhole
            Vector3 direction = (spiderHole.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * (speedWithGem * Time.deltaTime));
            // Rotate towards the spider hole
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Gem"))
        {
            isHoldingGem = true;
            targetGem.transform.parent = transform;
            targetGem.GetComponent<Rigidbody>().isKinematic =
                true; // Make the gem kinematic to stop physics interactions
            targetGem.GetComponent<Collider>().enabled = false; // Disable collider for the gem
            targetGem.transform.position = gemHoldPoint.position;
        }
    }

    private IEnumerator CheckForGems()
    {
        while (true)
        {
            // Wait for 1 second before checking for gems again
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