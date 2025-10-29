using UnityEngine;
using System.Collections;
using HorrorGame.Player;

public class Basement : MonoBehaviour
{
    [Header("Teleport Unlock Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayerMask = -1;
    
    [Header("Text Display")]
    [SerializeField] private GameObject textObject; // Text to display
    [SerializeField] private float textDisplayTime = 2f; // How long to show text
    [SerializeField] public GameObject oldtask;
    [SerializeField] public GameObject newtask;
    [SerializeField] public GameObject water;
    [SerializeField] public GameObject barrier;
    
    [Header("Water Movement Settings")]
    [SerializeField] public float waterMoveSpeed = 2f; // Speed of water movement
    [SerializeField] private float waterMoveDistance = 10f; // How far water should move down
    [SerializeField] private float waterMoveDuration = 5f; // How long water should move
    
    [Header("Deactivation Delay")]
    [SerializeField] private float deactivationDelay = 2f; // Delay before object disappears
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;
    
    private Camera playerCamera;
    private bool hasBeenUsed = false;
    private bool shouldMoveWater = false;
    private float waterMoveTimer = 0f;
    private Vector3 waterStartPosition;
    
    void Start()
    {
        playerCamera = Camera.main;
        
        // Make sure the object is active at start
        gameObject.SetActive(true);
        hasBeenUsed = false;
        shouldMoveWater = false;
        
        // Deactivate text on start
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
        
        // Store initial water position
        if (water != null)
        {
            waterStartPosition = water.transform.position;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(interactKey) && !hasBeenUsed)
        {
            TryUnlockTeleport5();
        }
        
        // Continuously move water if flag is set
        if (shouldMoveWater && water != null)
        {
            MoveWaterDown();
        }
    }
    
    void TryUnlockTeleport5()
    {
        if (playerCamera == null) return;
        
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactRange, interactLayerMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            // Check if we hit this object
            if (hitObject == gameObject)
            {
                UnlockTeleport5();
                oldtask.SetActive(false);
                newtask.SetActive(true);
                barrier.SetActive(false);
            }
        }
    }
    
    void UnlockTeleport5()
    {
        // Mark as used immediately to prevent multiple interactions
        hasBeenUsed = true;
        
        // Activate text immediately
        if (textObject != null)
        {
            textObject.SetActive(true);
            Debug.Log("Text displayed!");
        }
        
        // Start water movement
        if (water != null)
        {
            shouldMoveWater = true;
            waterMoveTimer = 0f;
            waterStartPosition = water.transform.position;
            Debug.Log("Starting water movement...");
        }
        
        // Find the FPSController and unlock teleport 5
        FPSController fpsController = FindObjectOfType<FPSController>();
        if (fpsController != null)
        {
            fpsController.UnlockTeleport5();
            Debug.Log("Teleport 5 unlocked!");
        }
        else
        {
            Debug.LogWarning("FPSController not found in scene!");
        }
        
        // Play sound if available
        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }
        
        // Schedule deactivation after delay
        Invoke("DeactivateObjects", deactivationDelay);
        
        Debug.Log("Teleport 5 unlock sequence started. Deactivation in " + deactivationDelay + " seconds.");
    }
    
    void MoveWaterDown()
    {
        waterMoveTimer += Time.deltaTime;
        
        // Calculate how far we should move (0 to 1)
        float progress = Mathf.Clamp01(waterMoveTimer / waterMoveDuration);
        
        // Move water downward using Lerp for smooth movement
        Vector3 targetPosition = waterStartPosition + Vector3.down * waterMoveDistance;
        water.transform.position = Vector3.Lerp(waterStartPosition, targetPosition, progress);
        
        // Alternative: Continuous movement (uncomment if you prefer this)
        // water.transform.Translate(Vector3.down * waterMoveSpeed * Time.deltaTime, Space.World);
        
        // Stop moving when duration is complete
        if (progress >= 1f)
        {
            shouldMoveWater = false;
            Debug.Log("Water movement completed.");
        }
    }
    
    void DeactivateObjects()
    {
        // Deactivate the text
        if (textObject != null)
        {
            textObject.SetActive(false);
            Debug.Log("Text deactivated.");
        }
        
        // Deactivate this object
        gameObject.SetActive(false);
        Debug.Log("Teleport 5 unlocker deactivated.");
    }
    
    // Public method to manually trigger unlocking (for testing)
    public void ManualUnlockTeleport5()
    {
        if (!hasBeenUsed)
        {
            UnlockTeleport5();
        }
    }
    
    // Reset the unlocker (for testing)
    public void ResetUnlocker()
    {
        hasBeenUsed = false;
        shouldMoveWater = false;
        waterMoveTimer = 0f;
        gameObject.SetActive(true);
        
        // Reset water position if needed
        if (water != null)
        {
            water.transform.position = waterStartPosition;
        }
        
        // Make sure text is deactivated on reset
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
        
        // Cancel any pending invocations
        CancelInvoke("DeactivateObjects");
    }
    
    // Check if this unlocker has been used
    public bool HasBeenUsed()
    {
        return hasBeenUsed;
    }
    
    // Set display time and delay
    public void SetDisplayTimes(float textTime, float deactivationTime)
    {
        textDisplayTime = textTime;
        deactivationDelay = deactivationTime;
    }
    
    // Set interact range
    public void SetInteractRange(float range)
    {
        interactRange = range;
    }
    
    // Set text object
    public void SetTextObject(GameObject newTextObject)
    {
        textObject = newTextObject;
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
    }
    
    // Set water movement parameters
    public void SetWaterMovement(float speed, float distance, float duration)
    {
        waterMoveSpeed = speed;
        waterMoveDistance = distance;
        waterMoveDuration = duration;
    }
}