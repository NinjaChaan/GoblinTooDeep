using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    
    [SerializeField] private Vector3 initialOffset;
    [SerializeField] private Quaternion initialRotation;
 
    
    private void LateUpdate()
    {
        if (target == null)
            return;
            
        // Calculate the desired position based on the target and offset
        Vector3 desiredPosition;
        
        desiredPosition = target.position + initialOffset;
        
        // Smoothly move the camera toward the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        
        // Keep rotation locked to initial rotation
        transform.rotation = initialRotation;
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnValidate()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player && !target)
            target = player.transform;
        
        if (target != null)
        {
            // Store initial offset and distance
            initialOffset = transform.position - target.position;
            initialRotation = transform.rotation;
        }
    }
}
