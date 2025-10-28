using UnityEngine;
using HorrorGame.Player;

public class Teleport5Unlocker : MonoBehaviour
{
    [Header("Teleport Unlock Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayerMask = -1;
    
    [Header("Text Display")]
    [SerializeField] private GameObject textObject; // Text to display
    [SerializeField] private float textDisplayTime = 2f; // How long to show text
    [SerializeField] private GameObject oldtask;
    [SerializeField] private GameObject newtask;
    [Header("Deactivation Delay")]
    [SerializeField] private float deactivationDelay = 2f; // Delay before object disappears
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;
    
    private Camera playerCamera;
    private bool hasBeenUsed = false;
    
    void Start()
    {
        playerCamera = Camera.main;
        
        // Make sure the object is active at start
        gameObject.SetActive(true);
        hasBeenUsed = false;
        
        // Deactivate text on start
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(interactKey) && !hasBeenUsed)
        {
            TryUnlockTeleport5();
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
        gameObject.SetActive(true);
        
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
}