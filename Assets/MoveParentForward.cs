using UnityEngine;

public class MoveParentForward : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDistance = 20f; // Increased default distance
    [SerializeField] private float deactivateDelay = 1f;
    [SerializeField] private bool useWorldSpace = false;
    
    private Vector3 startPosition;
    private bool isMoving = false;
    private float distanceMoved = 0f;
    
    void OnEnable()
    {
        // Reset when object is activated
        startPosition = transform.parent.position;
        isMoving = true;
        distanceMoved = 0f;
    }
    
    void Update()
    {
        if (isMoving)
        {
            // Get the correct forward direction
            Vector3 forwardDirection = useWorldSpace ? Vector3.forward : transform.parent.forward;
            
            // Calculate movement
            float movement = moveSpeed * Time.deltaTime;
            transform.parent.Translate(forwardDirection * movement, useWorldSpace ? Space.World : Space.Self);
            
            // Track distance moved
            distanceMoved += movement;
            
            // Check if we've moved far enough
            if (distanceMoved >= moveDistance)
            {
                isMoving = false;
                
                // Deactivate parent after delay
                if (deactivateDelay <= 0)
                {
                    transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    Invoke("DeactivateParent", deactivateDelay);
                }
            }
        }
    }
    
    void DeactivateParent()
    {
        transform.parent.gameObject.SetActive(false);
    }
    
    // Public method to start movement manually
    public void StartMovement()
    {
        startPosition = transform.parent.position;
        isMoving = true;
        distanceMoved = 0f;
    }
    
    // Public method to stop movement
    public void StopMovement()
    {
        isMoving = false;
    }
    
    // Check if currently moving
    public bool IsMoving()
    {
        return isMoving;
    }
    
    // Set movement parameters
    public void SetMovementParameters(float speed, float distance, float delay = 1f)
    {
        moveSpeed = speed;
        moveDistance = distance;
        deactivateDelay = delay;
    }
    
    // Set whether to use world space or local space
    public void SetUseWorldSpace(bool useWorld)
    {
        useWorldSpace = useWorld;
    }
    
    // Quick method to set just the distance
    public void SetMoveDistance(float distance)
    {
        moveDistance = distance;
    }
    
    // Quick method to set just the speed
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}