using UnityEngine;
using System.Collections;

public class EndCameraSequence : MonoBehaviour
{
    [Header("Camera Reference")]
    public Camera targetCamera;

    public GameObject text;
    
    [Header("Movement Settings")]
    public float moveDistance = 5f;
    public float moveDuration = 3f;
    
    [Header("Rotation Settings")]
    public float rotationDuration = 2f;
    
    [Header("Delay Settings")]
    public float delayBeforeQuit = 2f;
    
    [Header("Start Settings")]
    public bool startOnSceneStart = true;
    public float startDelay = 1f; // Delay before starting the sequence
    
    private bool sequenceStarted = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    void Start()
    {
        // If no camera specified, use this one
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();
            
        if (targetCamera != null)
        {
            initialPosition = targetCamera.transform.position;
            initialRotation = targetCamera.transform.rotation;
            Debug.Log("Camera sequence ready for: " + targetCamera.name);
            
            // Start the sequence automatically
            if (startOnSceneStart)
            {
                Invoke("StartEndSequence", startDelay);
            }
        }
    }
    
    void Update()
    {
        // Test with T key (optional - keep for testing)
        if (Input.GetKeyDown(KeyCode.T) && !sequenceStarted && targetCamera != null)
        {
            StartEndSequence();
        }
    }
    
    public void StartEndSequence()
    {
        if (targetCamera == null)
        {
            Debug.LogError("No camera assigned!");
            return;
        }
        
        if (!sequenceStarted)
        {
            sequenceStarted = true;
            StartCoroutine(CameraSequence());
        }
    }
    
    private IEnumerator CameraSequence()
    {
        Debug.Log("Starting camera end sequence...");
        
        // Step 1: Move camera forward
        yield return StartCoroutine(MoveCameraForward());
        
        // Step 2: Rotate camera 180 degrees
        yield return StartCoroutine(RotateCamera180());
        
        // Step 3: Wait for specified delay
        yield return new WaitForSeconds(delayBeforeQuit);
        
        // Step 4: Quit the game
        QuitGame();
    }
    
    private IEnumerator MoveCameraForward()
    {
        Debug.Log("Moving camera forward...");
        
        Vector3 startPosition = targetCamera.transform.position;
        // Use the camera's own forward direction, not world forward
        Vector3 endPosition = startPosition + targetCamera.transform.forward * moveDistance;
        
        Debug.Log("Start: " + startPosition);
        Debug.Log("End: " + endPosition);
        Debug.Log("Camera Forward: " + targetCamera.transform.forward);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / moveDuration;
            
            targetCamera.transform.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return null;
        }
        
        targetCamera.transform.position = endPosition;
        Debug.Log("Movement completed");
    }
    
    private IEnumerator RotateCamera180()
    {
        Debug.Log("Rotating camera 180 degrees...");
        
        Quaternion startRotation = targetCamera.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 180, 0);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rotationDuration;
            
            targetCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, progress);
            yield return null;
        }
        
        targetCamera.transform.rotation = endRotation;
        Debug.Log("Rotation completed");
        text.SetActive(true);
    }
    
    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    [ContextMenu("Test Sequence")]
    public void TestSequence()
    {
        if (!sequenceStarted && targetCamera != null)
        {
            StartEndSequence();
        }
    }
    
    [ContextMenu("Reset Camera")]
    public void ResetCamera()
    {
        StopAllCoroutines();
        sequenceStarted = false;
        
        if (targetCamera != null)
        {
            targetCamera.transform.position = initialPosition;
            targetCamera.transform.rotation = initialRotation;
            Debug.Log("Camera reset");
        }
    }
    
    // Visual debug in Scene view
    void OnDrawGizmos()
    {
        if (targetCamera != null)
        {
            // Draw movement path
            Gizmos.color = Color.green;
            Vector3 startPos = targetCamera.transform.position;
            Vector3 endPos = startPos + targetCamera.transform.forward * moveDistance;
            
            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawWireSphere(endPos, 0.3f);
            
            // Draw forward direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(targetCamera.transform.position, targetCamera.transform.forward * 2f);
        }
    }
}