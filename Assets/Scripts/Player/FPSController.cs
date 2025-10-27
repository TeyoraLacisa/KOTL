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
        
        [Header("Teleport IN Settings")]
        [SerializeField] private GameObject teleportObjectIN;
        [SerializeField] private Vector3 teleportDestinationIN = new Vector3(0, 0, 0);
        
        [Header("Teleport OUT Settings")]
        [SerializeField] private GameObject teleportObjectOUT;
        [SerializeField] private Vector3 teleportDestinationOUT = new Vector3(0, 0, 0);
        
        [Header("Shared Teleport Settings")]
        [SerializeField] private float teleportRange = 3f;
        [SerializeField] private KeyCode teleportKey = KeyCode.E;
        [SerializeField] private AudioClip teleportSound;
        [SerializeField] private LayerMask teleportLayerMask = -1;
        
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
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        void Update()
        {
            HandleMouseLook();
            HandleMovement();
            HandleFootsteps();
            HandleTeleport();
        }
        
        void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            transform.Rotate(0, mouseX, 0);
            
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
        
        void HandleMovement()
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);
            
            if (characterController.isGrounded)
            {
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 right = transform.TransformDirection(Vector3.right);
                
                float currentSpeed = isRunning ? runSpeed : walkSpeed;
                float curSpeedX = currentSpeed * vertical;
                float curSpeedY = currentSpeed * horizontal;
                
                moveDirection = (forward * curSpeedX) + (right * curSpeedY);
                
                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpSpeed;
                }
            }
            
            moveDirection.y -= gravity * Time.deltaTime;
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
        
        void HandleTeleport()
        {
            if (Input.GetKeyDown(teleportKey))
            {
                TryTeleport();
            }
        }
        
        void TryTeleport()
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, teleportRange, teleportLayerMask))
            {
                if (teleportObjectIN != null && hit.collider.gameObject == teleportObjectIN)
                {
                    TeleportPlayer(teleportDestinationIN, "IN");
                }
                else if (teleportObjectOUT != null && hit.collider.gameObject == teleportObjectOUT)
                {
                    TeleportPlayer(teleportDestinationOUT, "OUT");
                }
            }
        }
        
        void TeleportPlayer(Vector3 destination, string teleportType)
        {
            characterController.enabled = false;
            transform.position = destination;
            
            // Rotate 180 degrees for BOTH IN and OUT
            RotateCamera180();
            
            characterController.enabled = true;
            
            if (audioSource != null && teleportSound != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }
            
            Debug.Log($"Teleported {teleportType} to coordinates: {destination}");
        }
        
        void RotateCamera180()
        {
            transform.Rotate(0, 180f, 0);
            rotationX = -rotationX;
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
        
        void PlayFootstepSound()
        {
            if (audioSource != null && footstepSounds.Length > 0)
            {
                AudioClip footstepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                audioSource.PlayOneShot(footstepClip);
            }
        }
        
        public void SetCursorLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
        
        public bool IsRunning()
        {
            return isRunning;
        }
        
        public void SetTeleportObjectIN(GameObject newTeleportObject)
        {
            teleportObjectIN = newTeleportObject;
        }
        
        public void SetTeleportDestinationIN(Vector3 newDestination)
        {
            teleportDestinationIN = newDestination;
        }
        
        public void SetTeleportObjectOUT(GameObject newTeleportObject)
        {
            teleportObjectOUT = newTeleportObject;
        }
        
        public void SetTeleportDestinationOUT(Vector3 newDestination)
        {
            teleportDestinationOUT = newDestination;
        }
        
        public void SetTeleportRange(float newRange)
        {
            teleportRange = newRange;
        }
    }
}