using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private bool findPlayerOnStart = true;
    
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    
    private Vector3 initialOffset;
    private Quaternion initialRotation;
    
    private void Start()
    {
        if (findPlayerOnStart && target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("CameraController: No player found with tag 'Player'");
            }
        }
        
        if (target != null)
        {
            // Store initial offset and distance
            initialOffset = transform.position - target.position;
            initialRotation = transform.rotation;
        }
        else
        {
            Debug.LogError("CameraController: No target assigned!");
        }
    }
    
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
}
