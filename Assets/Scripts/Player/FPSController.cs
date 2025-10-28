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
        [SerializeField] private bool teleport1Unlocked = false; // NEW: Lock state

        [Header("Teleport 2 Settings")]
        [SerializeField] private GameObject teleportObject2;
        [SerializeField] private GameObject teleportDestination2;
        [SerializeField] private bool teleport2Unlocked = false;

        [Header("Teleport 3 Settings")]
        [SerializeField] private GameObject teleportObject3;
        [SerializeField] private GameObject teleportDestination3;
        [SerializeField] private bool teleport3Unlocked = false;

        [Header("Teleport 4 Settings")]
        [SerializeField] private GameObject teleportObject4;
        [SerializeField] private GameObject teleportDestination4;
        [SerializeField] private bool teleport4Unlocked = false;

        [Header("Teleport 5 Settings")]
        [SerializeField] private GameObject teleportObject5;
        [SerializeField] private GameObject teleportDestination5;
        [SerializeField] private bool teleport5Unlocked = false;

        [Header("Teleport 6 Settings")]
        [SerializeField] private GameObject teleportObject6;
        [SerializeField] private GameObject teleportDestination6;
        [SerializeField] private bool teleport6Unlocked = false;

        [Header("NPC Interaction")]
        [SerializeField] private KeyCode npcInteractKey = KeyCode.E;

        [Header("Shared Teleport Settings")]
        [SerializeField] private float teleportRange = 3f;
        [SerializeField] private KeyCode teleportKey = KeyCode.E;
        [SerializeField] private AudioClip teleportSound;
        [SerializeField] private AudioClip lockedSound; // NEW: Sound when teleport is locked
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
            HandleNPCInteraction();
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

        void HandleNPCInteraction()
        {
            if (Input.GetKeyDown(npcInteractKey))
            {
                TryNPCInteraction();
            }
        }

        void TryNPCInteraction()
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, teleportRange, teleportLayerMask))
            {
                GameObject hitObject = hit.collider.gameObject;

                NPCController npcController = hitObject.GetComponent<NPCController>();
                if (npcController != null)
                {
                    npcController.StartMovementSequence();
                }
            }
        }

        void TryTeleport()
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, teleportRange, teleportLayerMask))
            {
                GameObject hitObject = hit.collider.gameObject;

                // Check each teleport object with lock state
                if (teleportObject1 != null && hitObject == teleportObject1)
                {
                    if (teleport1Unlocked)
                    {
                        if (teleportDestination1 != null)
                        {
                            Vector3 destination = teleportDestination1.transform.position + teleportDestination1.transform.forward * 1f;
                            TeleportPlayer(destination);
                        }
                    }
                    else
                    {
                        PlayLockedSound();
                    }
                }
                else if (teleportObject2 != null && hitObject == teleportObject2)
                {
                    if (teleport2Unlocked)
                    {
                        if (teleportDestination2 != null)
                        {
                            Vector3 destination = teleportDestination2.transform.position + teleportDestination2.transform.forward * 1f;
                            TeleportPlayer(destination);
                        }
                    }
                    else
                    {
                        PlayLockedSound();
                    }
                }
                else if (teleportObject3 != null && hitObject == teleportObject3)
                {
                    if (teleport3Unlocked)
                    {
                        if (teleportDestination3 != null)
                        {
                            Vector3 destination = teleportDestination3.transform.position + teleportDestination3.transform.forward * 1f;
                            TeleportPlayer(destination);
                        }
                    }
                    else
                    {
                        PlayLockedSound();
                    }
                }
                else if (teleportObject4 != null && hitObject == teleportObject4)
                {
                    if (teleport4Unlocked)
                    {
                        if (teleportDestination4 != null)
                        {
                            Vector3 destination = teleportDestination4.transform.position + teleportDestination4.transform.forward * 1f;
                            TeleportPlayer(destination);
                        }
                    }
                    else
                    {
                        PlayLockedSound();
                    }
                }
                else if (teleportObject5 != null && hitObject == teleportObject5)
                {
                    if (teleport5Unlocked)
                    {
                        if (teleportDestination5 != null)
                        {
                            Vector3 destination = teleportDestination5.transform.position + teleportDestination5.transform.forward * 1f;
                            TeleportPlayer(destination);
                        }
                    }
                    else
                    {
                        PlayLockedSound();
                    }
                }
                else if (teleportObject6 != null && hitObject == teleportObject6)
                {
                    if (teleport6Unlocked)
                    {
                        if (teleportDestination6 != null)
                        {
                            Vector3 destination = teleportDestination6.transform.position + teleportDestination6.transform.forward * 1f;
                            TeleportPlayer(destination);
                        }
                    }
                    else
                    {
                        PlayLockedSound();
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

        // NEW: Play sound when teleport is locked
        void PlayLockedSound()
        {
            if (audioSource != null && lockedSound != null)
            {
                audioSource.PlayOneShot(lockedSound);
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

        // NEW: Methods to unlock teleports (for key pickup)
        public void UnlockTeleport1()
        {
            teleport1Unlocked = true;
        }

        public void UnlockTeleport2()
        {
            teleport2Unlocked = true;
        }

        public void UnlockTeleport3()
        {
            teleport3Unlocked = true;
        }

        public void UnlockTeleport4()
        {
            teleport4Unlocked = true;
        }

        public void UnlockTeleport5()
        {
            teleport5Unlocked = true;
        }

        public void UnlockTeleport6()
        {
            teleport6Unlocked = true;
        }

        // NEW: Methods to lock teleports
        public void LockTeleport1()
        {
            teleport1Unlocked = false;
        }

        public void LockTeleport2()
        {
            teleport2Unlocked = false;
        }

        public void LockTeleport3()
        {
            teleport3Unlocked = false;
        }

        public void LockTeleport4()
        {
            teleport4Unlocked = false;
        }

        public void LockTeleport5()
        {
            teleport5Unlocked = false;
        }

        public void LockTeleport6()
        {
            teleport6Unlocked = false;
        }

        // NEW: Check if teleport is unlocked
        public bool IsTeleport1Unlocked()
        {
            return teleport1Unlocked;
        }

        public bool IsTeleport2Unlocked()
        {
            return teleport2Unlocked;
        }

        public bool IsTeleport3Unlocked()
        {
            return teleport3Unlocked;
        }

        public bool IsTeleport4Unlocked()
        {
            return teleport4Unlocked;
        }

        public bool IsTeleport5Unlocked()
        {
            return teleport5Unlocked;
        }

        public bool IsTeleport6Unlocked()
        {
            return teleport6Unlocked;
        }

        // NEW: Unlock by index
        public void UnlockTeleportByIndex(int index)
        {
            switch (index)
            {
                case 1: UnlockTeleport1(); break;
                case 2: UnlockTeleport2(); break;
                case 3: UnlockTeleport3(); break;
                case 4: UnlockTeleport4(); break;
                case 5: UnlockTeleport5(); break;
                case 6: UnlockTeleport6(); break;
            }
        }

        public void TriggerNPCMovement(GameObject npcObject)
        {
            NPCController npcController = npcObject.GetComponent<NPCController>();
            if (npcController != null)
            {
                npcController.StartMoving();
            }
        }

        public void SetTeleportObject1(GameObject newTeleportObject) { teleportObject1 = newTeleportObject; }
        public void SetTeleportDestination1(GameObject newDestination) { teleportDestination1 = newDestination; }
        public void SetTeleportObject2(GameObject newTeleportObject) { teleportObject2 = newTeleportObject; }
        public void SetTeleportDestination2(GameObject newDestination) { teleportDestination2 = newDestination; }
        public void SetTeleportObject3(GameObject newTeleportObject) { teleportObject3 = newTeleportObject; }
        public void SetTeleportDestination3(GameObject newDestination) { teleportDestination3 = newDestination; }
        public void SetTeleportObject4(GameObject newTeleportObject) { teleportObject4 = newTeleportObject; }
        public void SetTeleportDestination4(GameObject newDestination) { teleportDestination4 = newDestination; }
        public void SetTeleportObject5(GameObject newTeleportObject) { teleportObject5 = newTeleportObject; }
        public void SetTeleportDestination5(GameObject newDestination) { teleportDestination5 = newDestination; }
        public void SetTeleportObject6(GameObject newTeleportObject) { teleportObject6 = newTeleportObject; }
        public void SetTeleportDestination6(GameObject newDestination) { teleportDestination6 = newDestination; }
        public void SetTeleportRange(float newRange) { teleportRange = newRange; }
    }
}