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
        
        [Header("Teleport 1 Settings")]
        [SerializeField] private GameObject teleportObject1;
        [SerializeField] private GameObject teleportDestination1;
        
        [Header("Teleport 2 Settings")]
        [SerializeField] private GameObject teleportObject2;
        [SerializeField] private GameObject teleportDestination2;
        
        [Header("Teleport 3 Settings")]
        [SerializeField] private GameObject teleportObject3;
        [SerializeField] private GameObject teleportDestination3;
        
        [Header("Teleport 4 Settings")]
        [SerializeField] private GameObject teleportObject4;
        [SerializeField] private GameObject teleportDestination4;
        
        [Header("Teleport 5 Settings")]
        [SerializeField] private GameObject teleportObject5;
        [SerializeField] private GameObject teleportDestination5;
        
        [Header("Teleport 6 Settings")]
        [SerializeField] private GameObject teleportObject6;
        [SerializeField] private GameObject teleportDestination6;
        
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
                GameObject hitObject = hit.collider.gameObject;
                
                if (teleportObject1 != null && hitObject == teleportObject1)
                {
                    if (teleportDestination1 != null)
                    {
                        Vector3 destination = teleportDestination1.transform.position + teleportDestination1.transform.forward * 1f;
                        TeleportPlayer(destination);
                    }
                }
                else if (teleportObject2 != null && hitObject == teleportObject2)
                {
                    if (teleportDestination2 != null)
                    {
                        Vector3 destination = teleportDestination2.transform.position + teleportDestination2.transform.forward * 1f;
                        TeleportPlayer(destination);
                    }
                }
                else if (teleportObject3 != null && hitObject == teleportObject3)
                {
                    if (teleportDestination3 != null)
                    {
                        Vector3 destination = teleportDestination3.transform.position + teleportDestination3.transform.forward * 1f;
                        TeleportPlayer(destination);
                    }
                }
                else if (teleportObject4 != null && hitObject == teleportObject4)
                {
                    if (teleportDestination4 != null)
                    {
                        Vector3 destination = teleportDestination4.transform.position + teleportDestination4.transform.forward * 1f;
                        TeleportPlayer(destination);
                    }
                }
                else if (teleportObject5 != null && hitObject == teleportObject5)
                {
                    if (teleportDestination5 != null)
                    {
                        Vector3 destination = teleportDestination5.transform.position + teleportDestination5.transform.forward * 1f;
                        TeleportPlayer(destination);
                    }
                }
                else if (teleportObject6 != null && hitObject == teleportObject6)
                {
                    if (teleportDestination6 != null)
                    {
                        Vector3 destination = teleportDestination6.transform.position + teleportDestination6.transform.forward * 1f;
                        TeleportPlayer(destination);
                    }
                }
            }
        }
        
        void TeleportPlayer(Vector3 destination)
        {
            characterController.enabled = false;
            transform.position = destination;
            
            RotateCamera180();
            
            characterController.enabled = true;
            
            if (audioSource != null && teleportSound != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }
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
        
        public void SetTeleportObject1(GameObject newTeleportObject)
        {
            teleportObject1 = newTeleportObject;
        }
        
        public void SetTeleportDestination1(GameObject newDestination)
        {
            teleportDestination1 = newDestination;
        }
        
        public void SetTeleportObject2(GameObject newTeleportObject)
        {
            teleportObject2 = newTeleportObject;
        }
        
        public void SetTeleportDestination2(GameObject newDestination)
        {
            teleportDestination2 = newDestination;
        }
        
        public void SetTeleportObject3(GameObject newTeleportObject)
        {
            teleportObject3 = newTeleportObject;
        }
        
        public void SetTeleportDestination3(GameObject newDestination)
        {
            teleportDestination3 = newDestination;
        }
        
        public void SetTeleportObject4(GameObject newTeleportObject)
        {
            teleportObject4 = newTeleportObject;
        }
        
        public void SetTeleportDestination4(GameObject newDestination)
        {
            teleportDestination4 = newDestination;
        }
        
        public void SetTeleportObject5(GameObject newTeleportObject)
        {
            teleportObject5 = newTeleportObject;
        }
        
        public void SetTeleportDestination5(GameObject newDestination)
        {
            teleportDestination5 = newDestination;
        }
        
        public void SetTeleportObject6(GameObject newTeleportObject)
        {
            teleportObject6 = newTeleportObject;
        }
        
        public void SetTeleportDestination6(GameObject newDestination)
        {
            teleportDestination6 = newDestination;
        }
        
        public void SetTeleportRange(float newRange)
        {
            teleportRange = newRange;
        }
    }
}