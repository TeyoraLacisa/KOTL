using UnityEngine;
using HorrorGame.Player;

public class ObjectCollector : MonoBehaviour
{
    [Header("Collectible Objects")]
    [SerializeField] private GameObject fuelObject;
    [SerializeField] private GameObject gearObject;
    
    [Header("Collection Triggers")]
    [SerializeField] private GameObject trigger1;
    [SerializeField] private GameObject trigger2;
    [SerializeField] private GameObject trigger3;
    [SerializeField] private GameObject oldtask;
    [SerializeField] private GameObject newtask;
    [SerializeField] private GameObject newtask2;
    
    [Header("Activation Objects")]
    [SerializeField] private GameObject lightObject;
    [SerializeField] private GameObject newGearObject;
    
    [Header("Light Rotation Settings")]
    [SerializeField] private float lightRotationSpeed = 30f;
    [SerializeField] private bool rotateLightOnTrigger3 = true;
    
    [Header("Teleport Control")]
    [SerializeField] private bool blockTeleport1OnTrigger3 = true;
    [SerializeField] private bool unlockTeleport3OnTrigger3 = true; // NEW: Unlock teleport 3
    
    [Header("Collection Settings")]
    [SerializeField] private KeyCode collectKey = KeyCode.E;
    [SerializeField] private float collectRange = 3f;
    [SerializeField] private LayerMask collectLayerMask = -1;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioClip activateSound;
    
    private Camera playerCamera;
    private bool fuelCollected = false;
    private bool gearCollected = false;
    private bool trigger1Activated = false;
    private bool trigger2Activated = false;
    private bool trigger3Activated = false;
    private bool lightActivated = false;
    private bool newGearActivated = false;
    private bool shouldRotateLight = false;
    
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        
        // Initialize objects
        if (fuelObject != null)
            fuelObject.SetActive(true);
        if (gearObject != null)
            gearObject.SetActive(true);
        if (lightObject != null)
            lightObject.SetActive(false);
        if (newGearObject != null)
            newGearObject.SetActive(false);
            
        // Deactivate all triggers at start
        if (trigger1 != null)
            trigger1.SetActive(false);
        if (trigger2 != null)
            trigger2.SetActive(false);
        if (trigger3 != null)
            trigger3.SetActive(false);
    }
    
    void Update()
    {
        HandleObjectCollection();
        CheckTriggerActivation();
        RotateLight();
    }
    
    void HandleObjectCollection()
    {
        if (Input.GetKeyDown(collectKey))
        {
            TryCollectObject();
        }
    }
    
    void TryCollectObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, collectRange, collectLayerMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            if (!fuelCollected && fuelObject != null && hitObject == fuelObject)
            {
                CollectFuel();
            }
            else if (!gearCollected && gearObject != null && hitObject == gearObject)
            {
                CollectGear();
            }
            else if (IsTriggerObject(hitObject))
            {
                HandleTriggerActivation(hitObject);
            }
        }
    }
    
    void CollectFuel()
    {
        fuelObject.SetActive(false);
        fuelCollected = true;
        PlayCollectSound();
        Debug.Log("Fuel collected! Trigger 1 is now available.");
        
        CheckTriggerActivation();
        OnFuelCollected();
    }
    
    void CollectGear()
    {
        gearObject.SetActive(false);
        gearCollected = true;
        PlayCollectSound();
        Debug.Log("Gear collected! Trigger 2 is now available.");
        
        CheckTriggerActivation();
        OnGearCollected();
    }
    
    bool IsTriggerObject(GameObject obj)
    {
        return obj == trigger1 || obj == trigger2 || obj == trigger3;
    }
    
    void HandleTriggerActivation(GameObject trigger)
    {
        if (trigger == trigger1)
        {
            if (fuelCollected && !trigger1Activated)
            {
                ActivateTrigger1();
            }
        }
        else if (trigger == trigger2)
        {
            if (gearCollected && !trigger2Activated)
            {
                ActivateTrigger2();
            }
        }
        else if (trigger == trigger3)
        {
            if (trigger3Activated)
            {
                ActivateTrigger3();
            }
        }
    }
    
    void CheckTriggerActivation()
    {
        if (fuelCollected && trigger1 != null && !trigger1.activeInHierarchy)
        {
            trigger1.SetActive(true);
            Debug.Log("Trigger 1 activated and available for interaction!");
        }
        
        if (gearCollected && trigger2 != null && !trigger2.activeInHierarchy)
        {
            trigger2.SetActive(true);
            Debug.Log("Trigger 2 activated and available for interaction!");
        }
        
        if (trigger1Activated && trigger2Activated && trigger3 != null && !trigger3.activeInHierarchy)
        {
            trigger3.SetActive(true);
            oldtask.SetActive(false);
            newtask.SetActive(true);
            trigger3Activated = true;
            Debug.Log("Trigger 3 activated and available for interaction!");
        }
    }
    
    void ActivateTrigger1()
    {
        trigger1Activated = true;
        PlayActivateSound();
        Debug.Log("Trigger 1 used! Light activated.");
        
        if (lightObject != null)
        {
            lightObject.SetActive(true);
            lightActivated = true;
        }
        
        CheckTriggerActivation();
        OnTrigger1Activated();
    }
    
    void ActivateTrigger2()
    {
        trigger2Activated = true;
        PlayActivateSound();
        Debug.Log("Trigger 2 used! New gear activated.");
        
        if (newGearObject != null)
        {
            newGearObject.SetActive(true);
            newGearActivated = true;
        }
        
        CheckTriggerActivation();
        OnTrigger2Activated();
    }
    
    void ActivateTrigger3()
    {
        PlayActivateSound();
        newtask.SetActive(false);
        newtask2.SetActive(true);
        Debug.Log("Trigger 3 used! Teleport 2 & 3 unlocked, Teleport 1 blocked, and light rotation started.");
        
        // Start rotating the light
        if (rotateLightOnTrigger3 && lightObject != null && lightActivated)
        {
            shouldRotateLight = true;
            Debug.Log("Light rotation started!");
        }
        
        // Control teleports
        ControlTeleports();
        
        OnTrigger3Activated();
    }
    
    void ControlTeleports()
    {
        FPSController fpsController = FindObjectOfType<FPSController>();
        if (fpsController != null)
        {
            // Unlock teleport 2
            fpsController.UnlockTeleport2();
            
            // NEW: Unlock teleport 3
            if (unlockTeleport3OnTrigger3)
            {
                fpsController.UnlockTeleport3();
                Debug.Log("Teleport 3 unlocked!");
            }
            
            // Block teleport 1 if enabled
            if (blockTeleport1OnTrigger3)
            {
                fpsController.LockTeleport1();
                Debug.Log("Teleport 1 has been blocked!");
            }
        }
        else
        {
            Debug.LogWarning("FPSController not found in scene!");
        }
    }
    
    void RotateLight()
    {
        if (shouldRotateLight && lightObject != null)
        {
            lightObject.transform.Rotate(0f, lightRotationSpeed * Time.deltaTime, 0f, Space.World);
        }
    }
    
    void PlayCollectSound()
    {
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
    }
    
    void PlayActivateSound()
    {
        if (audioSource != null && activateSound != null)
        {
            audioSource.PlayOneShot(activateSound);
        }
    }
    
    // Event methods
    protected virtual void OnFuelCollected()
    {
        // Override for custom behavior
    }
    
    protected virtual void OnGearCollected()
    {
        // Override for custom behavior
    }
    
    protected virtual void OnTrigger1Activated()
    {
        // Override for custom behavior
    }
    
    protected virtual void OnTrigger2Activated()
    {
        // Override for custom behavior
    }
    
    protected virtual void OnTrigger3Activated()
    {
        // Override for custom behavior
    }
    
    // Control light rotation
    public void StartLightRotation()
    {
        if (lightObject != null && lightActivated)
        {
            shouldRotateLight = true;
        }
    }
    
    public void StopLightRotation()
    {
        shouldRotateLight = false;
    }
    
    public void SetLightRotationSpeed(float speed)
    {
        lightRotationSpeed = speed;
    }
    
    public bool IsLightRotating()
    {
        return shouldRotateLight;
    }
    
    // Teleport control methods
    public void BlockTeleport1()
    {
        FPSController fpsController = FindObjectOfType<FPSController>();
        if (fpsController != null)
        {
            fpsController.LockTeleport1();
        }
    }
    
    public void UnblockTeleport1()
    {
        FPSController fpsController = FindObjectOfType<FPSController>();
        if (fpsController != null)
        {
            fpsController.UnlockTeleport1();
        }
    }
    
    // NEW: Unlock teleport 3 manually
    public void UnlockTeleport3()
    {
        FPSController fpsController = FindObjectOfType<FPSController>();
        if (fpsController != null)
        {
            fpsController.UnlockTeleport3();
        }
    }
    
    public void SetBlockTeleport1OnTrigger3(bool block)
    {
        blockTeleport1OnTrigger3 = block;
    }
    
    // NEW: Set unlock teleport 3 on trigger 3
    public void SetUnlockTeleport3OnTrigger3(bool unlock)
    {
        unlockTeleport3OnTrigger3 = unlock;
    }
    
    // Public methods to check status
    public bool IsFuelCollected()
    {
        return fuelCollected;
    }
    
    public bool IsGearCollected()
    {
        return gearCollected;
    }
    
    public bool IsTrigger1Activated()
    {
        return trigger1Activated;
    }
    
    public bool IsTrigger2Activated()
    {
        return trigger2Activated;
    }
    
    public bool IsTrigger3Activated()
    {
        return trigger3Activated;
    }
    
    public bool IsLightActivated()
    {
        return lightActivated;
    }
    
    public bool IsNewGearActivated()
    {
        return newGearActivated;
    }
    
    public bool AreAllObjectsCollected()
    {
        return fuelCollected && gearCollected;
    }
    
    public bool AreAllTriggersActivated()
    {
        return trigger1Activated && trigger2Activated && trigger3Activated;
    }
    
    // Manual control methods (for testing)
    public void CollectFuelManually()
    {
        if (!fuelCollected && fuelObject != null)
        {
            CollectFuel();
        }
    }
    
    public void CollectGearManually()
    {
        if (!gearCollected && gearObject != null)
        {
            CollectGear();
        }
    }
    
    public void ActivateTrigger1Manually()
    {
        if (!trigger1Activated && trigger1 != null)
        {
            if (!trigger1.activeInHierarchy)
                trigger1.SetActive(true);
            ActivateTrigger1();
        }
    }
    
    public void ActivateTrigger2Manually()
    {
        if (!trigger2Activated && trigger2 != null)
        {
            if (!trigger2.activeInHierarchy)
                trigger2.SetActive(true);
            ActivateTrigger2();
        }
    }
    
    public void ActivateTrigger3Manually()
    {
        if (trigger3Activated && trigger3 != null)
        {
            ActivateTrigger3();
        }
    }
    
    // Reset everything
    public void ResetAll()
    {
        fuelCollected = false;
        gearCollected = false;
        trigger1Activated = false;
        trigger2Activated = false;
        trigger3Activated = false;
        lightActivated = false;
        newGearActivated = false;
        shouldRotateLight = false;
        
        if (fuelObject != null)
            fuelObject.SetActive(true);
        if (gearObject != null)
            gearObject.SetActive(true);
        if (lightObject != null)
            lightObject.SetActive(false);
        if (newGearObject != null)
            newGearObject.SetActive(false);
            
        if (trigger1 != null)
            trigger1.SetActive(false);
        if (trigger2 != null)
            trigger2.SetActive(false);
        if (trigger3 != null)
            trigger3.SetActive(false);
    }
    
    // Set objects programmatically
    public void SetFuelObject(GameObject newFuelObject)
    {
        fuelObject = newFuelObject;
        fuelCollected = false;
    }
    
    public void SetGearObject(GameObject newGearObject)
    {
        gearObject = newGearObject;
        gearCollected = false;
    }
    
    public void SetTrigger1(GameObject newTrigger1)
    {
        trigger1 = newTrigger1;
        trigger1Activated = false;
        if (trigger1 != null)
            trigger1.SetActive(false);
    }
    
    public void SetTrigger2(GameObject newTrigger2)
    {
        trigger2 = newTrigger2;
        trigger2Activated = false;
        if (trigger2 != null)
            trigger2.SetActive(false);
    }
    
    public void SetTrigger3(GameObject newTrigger3)
    {
        trigger3 = newTrigger3;
        trigger3Activated = false;
        if (trigger3 != null)
            trigger3.SetActive(false);
    }
    
    public void SetLightObject(GameObject newLightObject)
    {
        lightObject = newLightObject;
        lightActivated = false;
        shouldRotateLight = false;
    }
    
    public void SetNewGearObject(GameObject newNewGearObject)
    {
        newGearObject = newNewGearObject;
        newGearActivated = false;
    }
}