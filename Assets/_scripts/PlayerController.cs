using UnityEngine;
using PurrNet;

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 3f;
    public float runningSpeed = 6f;
    public float jumpSpeed = 7.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;

    CharacterController characterController;
    public NetworkAnimator animator;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;
    public bool cursorLocked = false;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    public void ChangeCursorLock()
    {
        if (!cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cursorLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cursorLocked = false;
        }
    }

    void Start()
    {
        enabled = isOwner;
        
        // Sets camera to each local player
        playerCamera = Camera.main;
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
        playerCamera.transform.SetParent(transform);

        characterController = GetComponent<CharacterController>();

        // Lock cursor
        ChangeCursorLock();
    }

    void Update()
    {
        if (cursorLocked)
        {
            // Movement and running input
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float moveInputVertical = Input.GetAxis("Vertical"); // Forward/backward
            float moveInputHorizontal = Input.GetAxis("Horizontal"); // Left/right

            // Calculate forward and right movement
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * moveInputVertical : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * moveInputHorizontal : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            // Jump logic
            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            // Apply gravity if not grounded
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            // Move the character controller
            characterController.Move(moveDirection * Time.deltaTime);

            // Handle camera rotation
            if (canMove && playerCamera != null)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }

            // Update animator parameters
            UpdateAnimatorParameters(moveInputVertical, moveInputHorizontal, isRunning);

            // Unlock cursor with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeCursorLock();
            }
        }
    }

    private void UpdateAnimatorParameters(float vertical, float horizontal, bool isRunning)
    {
        // Set the Animator parameters based on input and running state
        float speedMultiplier = isRunning ? 1 : 0.5f; // Slow down for walking
        animator.SetFloat("x", horizontal * speedMultiplier); // Left/Right movement
        animator.SetFloat("y", vertical * speedMultiplier);  // Forward/Backward movement
    }
}

