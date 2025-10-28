using UnityEngine;
using HorrorGame.Player;

public class NPCController : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool startPaused = true;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [Header("Movement Sequence")]
    [SerializeField] private float turnAngle = 90f;
    [SerializeField] private float moveForwardDistance = 3f;
    [SerializeField] private float turnDuration = 1f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float startDelay = 2f;
    
    [Header("Text Display")]
    [SerializeField] private GameObject textObject;
    [SerializeField] private GameObject textObjectA;
    [SerializeField] private GameObject textObjectB;
    [SerializeField] private float textDisplayTime = 2f;
    
    [Header("NPC Replacement")]
    [SerializeField] private GameObject replacementNPC;
    [SerializeField] private Transform destinationPoint;
    
    [Header("Additional Object Sequence")]
    [SerializeField] private GameObject additionalObject;
    [SerializeField] private float objectMoveDistance = 5f;
    [SerializeField] private float objectMoveDuration = 3f;
    [SerializeField] private float objectStartDelay = 0.5f;
    
    [Header("Teleport Unlock")] // NEW: Teleport unlock settings
    [SerializeField] private bool unlockTeleportOnInteract = true;
    [SerializeField] private int teleportToUnlock = 1;
    
    private bool isMoving = false;
    private bool isPaused = true;
    private bool isExecutingSequence = false;
    private bool isInStartDelay = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float sequenceTimer = 0f;
    private float startDelayTimer = 0f;
    private SequenceState currentState = SequenceState.Idle;
    
    private Vector3 objectStartPosition;
    private bool isObjectMoving = false;
    private float objectMoveTimer = 0f;
    
    private enum SequenceState
    {
        Idle,
        StartDelay,
        Turning,
        MovingForward,
        Complete
    }
    
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        startPosition = transform.position;
        startRotation = transform.rotation;
            
        if (additionalObject != null)
        {
            objectStartPosition = additionalObject.transform.position;
        }
            
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
            
        if (startPaused)
        {
            PauseAnimation();
        }
        else
        {
            ResumeAnimation();
        }
        
        if (replacementNPC != null)
        {
            replacementNPC.SetActive(false);
        }
    }
    
    void Update()
    {
        if (isInStartDelay)
        {
            HandleStartDelay();
            textObjectA.SetActive(false);
            textObjectB.SetActive(true);
        }
        else if (isExecutingSequence)
        {
            ExecuteMovementSequence();
        }
        else if (isMoving && !isPaused)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        
        if (isObjectMoving)
        {
            ExecuteObjectMovement();
        }
    }
    
    // Start the complex movement sequence
    public void StartMovementSequence()
    {
        if (!isExecutingSequence && !isInStartDelay)
        {
            isExecutingSequence = true;
            isInStartDelay = true;
            startDelayTimer = 0f;
            currentState = SequenceState.StartDelay;
            
            // Activate text immediately
            if (textObject != null)
            {
                textObject.SetActive(true);
            }
            
            // NEW: Unlock teleport immediately when E is pressed
            UnlockTeleport();
        }
    }
    
    // NEW: Unlock teleport method
    void UnlockTeleport()
    {
        if (unlockTeleportOnInteract)
        {
            // Find the FPSController in the scene
            FPSController fpsController = FindObjectOfType<FPSController>();
            if (fpsController != null)
            {
                switch (teleportToUnlock)
                {
                    case 1:
                        fpsController.UnlockTeleport1();
                        break;
                    case 2:
                        fpsController.UnlockTeleport2();
                        break;
                    case 3:
                        fpsController.UnlockTeleport3();
                        break;
                    case 4:
                        fpsController.UnlockTeleport4();
                        break;
                    case 5:
                        fpsController.UnlockTeleport5();
                        break;
                    case 6:
                        fpsController.UnlockTeleport6();
                        break;
                    default:
                        fpsController.UnlockTeleport1();
                        break;
                }
                Debug.Log($"Teleport {teleportToUnlock} unlocked!");
            }
            else
            {
                Debug.LogWarning("FPSController not found in scene!");
            }
        }
    }
    
    void HandleStartDelay()
    {
        startDelayTimer += Time.deltaTime;
        
        if (startDelayTimer >= startDelay)
        {
            isInStartDelay = false;
            isMoving = true;
            ResumeAnimation();
            currentState = SequenceState.Turning;
            sequenceTimer = 0f;
            
            if (textObject != null)
            {
                textObject.SetActive(false);
            }
        }
    }
    
    void ExecuteMovementSequence()
    {
        sequenceTimer += Time.deltaTime;
        
        switch (currentState)
        {
            case SequenceState.Turning:
                ExecuteTurn();
                break;
                
            case SequenceState.MovingForward:
                ExecuteMoveForward();
                break;
                
            case SequenceState.Complete:
                CompleteSequence();
                break;
        }
    }
    
    void ExecuteTurn()
    {
        float turnProgress = sequenceTimer / turnDuration;
        
        if (turnProgress < 1f)
        {
            float targetYRotation = startRotation.eulerAngles.y + turnAngle;
            float currentYRotation = Mathf.Lerp(startRotation.eulerAngles.y, targetYRotation, turnProgress);
            transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
        }
        else
        {
            sequenceTimer = 0f;
            currentState = SequenceState.MovingForward;
            startPosition = transform.position;
        }
    }
    
    void ExecuteMoveForward()
    {
        float moveProgress = sequenceTimer / moveDuration;
        
        if (moveProgress < 1f)
        {
            Vector3 targetPosition = startPosition + transform.forward * moveForwardDistance;
            transform.position = Vector3.Lerp(startPosition, targetPosition, moveProgress);
        }
        else
        {
            sequenceTimer = 0f;
            currentState = SequenceState.Complete;
        }
    }
    
    void CompleteSequence()
    {
        if (replacementNPC != null && destinationPoint != null)
        {
            replacementNPC.transform.position = destinationPoint.position;
            replacementNPC.transform.rotation = destinationPoint.rotation;
            replacementNPC.SetActive(true);
            
            NPCController replacementController = replacementNPC.GetComponent<NPCController>();
            if (replacementController != null)
            {
                replacementController.StartMoving();
            }
        }
        
        if (additionalObject != null)
        {
            Invoke("StartObjectMovement", objectStartDelay);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
        isExecutingSequence = false;
        currentState = SequenceState.Idle;
    }
    
    void StartObjectMovement()
    {
        if (additionalObject != null)
        {
            isObjectMoving = true;
            objectMoveTimer = 0f;
            objectStartPosition = additionalObject.transform.position;
            additionalObject.SetActive(true);
        }
        
        gameObject.SetActive(false);
    }
    
    void ExecuteObjectMovement()
    {
        if (additionalObject == null) return;
        
        objectMoveTimer += Time.deltaTime;
        float moveProgress = objectMoveTimer / objectMoveDuration;
        
        if (moveProgress < 1f)
        {
            Vector3 targetPosition = objectStartPosition + additionalObject.transform.forward * objectMoveDistance;
            additionalObject.transform.position = Vector3.Lerp(objectStartPosition, targetPosition, moveProgress);
        }
        else
        {
            additionalObject.SetActive(false);
            isObjectMoving = false;
        }
    }
    
    // Start simple moving and resume animation
    public void StartMoving()
    {
        isMoving = true;
        ResumeAnimation();
    }
    
    // Stop moving and pause animation
    public void StopMoving()
    {
        isMoving = false;
        PauseAnimation();
    }
    
    public void PauseAnimation()
    {
        if (animator != null)
        {
            animator.speed = 0f;
        }
        isPaused = true;
    }
    
    public void ResumeAnimation()
    {
        if (animator != null)
        {
            animator.speed = 1f;
        }
        isPaused = false;
    }
    
    public bool IsMoving()
    {
        return isMoving && !isPaused;
    }
    
    public bool IsAnimationPaused()
    {
        return isPaused;
    }
    
    public bool IsExecutingSequence()
    {
        return isExecutingSequence;
    }
    
    public bool IsObjectMoving()
    {
        return isObjectMoving;
    }
    
    public bool IsInStartDelay()
    {
        return isInStartDelay;
    }
    
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    
    public void SetStartDelay(float delay)
    {
        startDelay = delay;
    }
    
    public void SetTextObject(GameObject textObj)
    {
        textObject = textObj;
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
    }
    
    // NEW: Set which teleport to unlock
    public void SetTeleportToUnlock(int teleportNumber)
    {
        teleportToUnlock = Mathf.Clamp(teleportNumber, 1, 6);
    }
    
    // NEW: Enable/disable teleport unlocking
    public void SetUnlockTeleportOnInteract(bool unlock)
    {
        unlockTeleportOnInteract = unlock;
    }
    
    public void SetReplacementNPC(GameObject newReplacementNPC)
    {
        replacementNPC = newReplacementNPC;
        if (replacementNPC != null)
        {
            replacementNPC.SetActive(false);
        }
    }
    
    public void SetDestinationPoint(Transform newDestinationPoint)
    {
        destinationPoint = newDestinationPoint;
    }
    
    public void SetAdditionalObject(GameObject newAdditionalObject)
    {
        additionalObject = newAdditionalObject;
        if (additionalObject != null)
        {
            objectStartPosition = additionalObject.transform.position;
        }
    }
    
    public void SetObjectMovementParameters(float distance, float duration, float delay)
    {
        objectMoveDistance = distance;
        objectMoveDuration = duration;
        objectStartDelay = delay;
    }
    
    public void ResetNPC()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        isMoving = false;
        isExecutingSequence = false;
        isInStartDelay = false;
        isObjectMoving = false;
        currentState = SequenceState.Idle;
        sequenceTimer = 0f;
        startDelayTimer = 0f;
        objectMoveTimer = 0f;
        gameObject.SetActive(true);
        PauseAnimation();
        
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
        
        if (additionalObject != null)
        {
            additionalObject.transform.position = objectStartPosition;
            additionalObject.SetActive(true);
        }
        
        if (replacementNPC != null)
        {
            replacementNPC.SetActive(false);
        }
    }
    
    public void ManualStartObjectMovement()
    {
        if (additionalObject != null)
        {
            StartObjectMovement();
        }
    }
}