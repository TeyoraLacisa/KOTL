using UnityEngine;

namespace HorrorGame.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpSpeed = 8f;
        [SerializeField] private float gravity = 20f;
        [SerializeField] private float mouseSensitivity = 2f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] footstepSounds;
        [SerializeField] private float footstepInterval = 0.5f;
        
        private CharacterController characterController;
        private Camera playerCamera;
        private Vector3 moveDirection = Vector3.zero;
        private float rotationX = 0;
        private float lastFootstepTime;
        private bool isRunning;
        
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerCamera = GetComponentInChildren<Camera>();
            
            // Lock cursor to center of screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        void Update()
        {
            HandleMouseLook();
            HandleMovement();
            HandleFootsteps();
        }
        
        void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            // Rotate the player body left/right
            transform.Rotate(0, mouseX, 0);
            
            // Rotate the camera up/down
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
        
        void HandleMovement()
        {
            // Check if running
            isRunning = Input.GetKey(KeyCode.LeftShift);
            
            if (characterController.isGrounded)
            {
                // Get input
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                
                // Calculate movement direction
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 right = transform.TransformDirection(Vector3.right);
                
                float currentSpeed = isRunning ? runSpeed : walkSpeed;
                float curSpeedX = currentSpeed * vertical;
                float curSpeedY = currentSpeed * horizontal;
                
                moveDirection = (forward * curSpeedX) + (right * curSpeedY);
                
                // Jump
                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpSpeed;
                }
            }
            
            // Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;
            
            // Move the controller
            characterController.Move(moveDirection * Time.deltaTime);
        }
        
        void HandleFootsteps()
        {
            if (characterController.isGrounded && 
                (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) &&
                Time.time - lastFootstepTime > footstepInterval)
            {
                PlayFootstepSound();
                lastFootstepTime = Time.time;
            }
        }
        
        void PlayFootstepSound()
        {
            if (audioSource != null && footstepSounds.Length > 0)
            {
                AudioClip footstepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                audioSource.PlayOneShot(footstepClip);
            }
        }
        
        // Public method to unlock cursor (for UI interactions)
        public void SetCursorLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
        
        // Public method to check if player is running
        public bool IsRunning()
        {
            return isRunning;
        }
    }
}