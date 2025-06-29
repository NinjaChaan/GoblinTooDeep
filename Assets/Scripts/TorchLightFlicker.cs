using System.Collections;
using UnityEngine;

public class TorchLightFlicker : MonoBehaviour
{
    public GameObject torchObject; 
    private Transform lightTransform; 
    private Light torchLight;
    public float flickerSpeed = 0.1f;
    public float flickerIntensityMin = 4.9f;
    public float flickerIntensityMax = 5.1f;
    public Transform lightPivotPoint;

    public float torchStrength = 1f;
    public ParticleSystem torchParticles;
    private float particlesSize;
    private float particlesRate;
    private float range;
    private float intensityMin;
    private float intensityMax;

    void Start()
    {
        var lightgo = Instantiate(torchObject);
        lightTransform = lightgo.transform;
        torchLight = lightgo.GetComponent<Light>();
        StartCoroutine(Flicker());
        particlesSize = torchParticles.main.startSize.constant;
        particlesRate = torchParticles.emission.rateOverTime.constant;
        range = torchLight.range;
        intensityMin = flickerIntensityMin;
        intensityMax = flickerIntensityMax;
    }

    
    void Update()
    {
        lightTransform.position = lightPivotPoint.position + Vector3.up * 2;
    }
    
    public void SetTorchStrength(float strength)
    {
        torchStrength = strength;
        var main = torchParticles.main;
        var emission = torchParticles.emission;
        torchLight.range = Mathf.Max(range * strength, range * 0.5f);
        flickerIntensityMin = Mathf.Max(intensityMin * strength, intensityMin * 0.5f);
        flickerIntensityMax = Mathf.Max(intensityMax * strength, intensityMax * 0.5f);
        main.startSize = Mathf.Max(particlesSize * strength, particlesSize * 0.5f);
        emission.rateOverTime = Mathf.Max(particlesRate * strength, particlesRate * 0.5f);
    }

    // Method to handle the flickering effect
    private IEnumerator Flicker()
    {
        while (true)
        {
            // Randomly set the intensity of the torch light
            torchLight.intensity = Random.Range(flickerIntensityMin, flickerIntensityMax);
            // Wait for a short duration before the next flicker
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    public void SetLightOnOff(bool isOn)
    {
        torchLight.enabled = isOn;
    }
}