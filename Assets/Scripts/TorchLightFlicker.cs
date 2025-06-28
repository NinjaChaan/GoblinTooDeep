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

    void Start()
    {
        var lightgo = Instantiate(torchObject);
        lightTransform = lightgo.transform;
        torchLight = lightgo.GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    
    void Update()
    {
        lightTransform.position = lightPivotPoint.position + Vector3.up * 2;
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