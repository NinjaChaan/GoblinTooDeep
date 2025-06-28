using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class OreRock : InteractableObject
{
    public GameObject mineablePart;
    public GameObject gemPrefab;
    public GameObject sparkVfx;
    public GameObject rubbleVfx;
    public GameObject rubbleSmolVfx;
    public int gemCount = 3;
    public int hitpoints = 5;
    public Vector2 explosionForceMinMax = new Vector2(5f, 10f);

    public List<Material> gemMaterials;
    
    public AudioSource audioSource;
    public AudioClip hitSound;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        RockHit();
    }

    private void RockHit()
    {
        audioSource.PlayOneShot(hitSound);
        Instantiate(sparkVfx, transform.position, Quaternion.identity);
        Instantiate(rubbleSmolVfx, transform.position, Quaternion.identity);
        hitpoints--;

        if (hitpoints <= 0)
        {
            GetComponentInChildren<Collider>().enabled = false;
            for (int i = 0; i < gemCount; i++)
            {
                GameObject gem = Instantiate(gemPrefab, transform.position, Random.rotation);

                Vector3 randomDirection = Random.onUnitSphere;
                randomDirection.y = Mathf.Abs(randomDirection.y);
                float explosionForce = Random.Range(explosionForceMinMax.x, explosionForceMinMax.y);
                var rb = gem.GetComponent<Rigidbody>();
                rb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);
                rb.AddTorque(Random.onUnitSphere * explosionForce, ForceMode.Impulse);
                gem.GetComponent<Renderer>().material = gemMaterials[Random.Range(0, gemMaterials.Count)];
            }

            Instantiate(rubbleVfx, transform.position, Quaternion.identity);
            mineablePart.SetActive(false);
        }
    }

    public override bool IsInteractable => hitpoints > 0;
    public override InteractButton InteractButton => InteractButton.Mine;

    public override void Interact()
    {
        if (PlayerScript.Instance.PickSelected)
        {
            RockHit();
        }
    }
}