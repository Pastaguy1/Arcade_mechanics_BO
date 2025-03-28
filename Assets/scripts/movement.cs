using UnityEngine;

public class Movement : MonoBehaviour
{
    public Camera playerCamera;  // De camera van de speler (dit moet een referentie naar de camera zijn)
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 20f;
    public float lookSpeed = 3.1f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0; // Vertical camera rotation (up/down)
    private float rotationY = 0; // Horizontal player rotation (left/right)
    private CharacterController characterController;

    private bool canMove = true;
    private bool isCrouching = false;

    void Start()
    {
        // Zorg ervoor dat de CharacterController aan de speler is gekoppeld
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("No CharacterController found on this object.");
        }

        // Zorg ervoor dat de camera correct is toegewezen (via de Inspector)
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned in the Inspector.");
        }

        // Lock de cursor in het midden van het scherm
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (characterController == null || playerCamera == null)
            return;

        // Beweging van de speler (vooruit, achteruit, links, rechts)
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        float curSpeedX = canMove ? currentSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? currentSpeed * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Crouch Logic (in- en uitzoomen)
        if (Input.GetKeyDown(KeyCode.R))
        {
            isCrouching = !isCrouching;
        }

        if (isCrouching)
        {
            characterController.height = Mathf.Lerp(characterController.height, crouchHeight, Time.deltaTime * 5);
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = Mathf.Lerp(characterController.height, defaultHeight, Time.deltaTime * 5);
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            // Muisbewegingen voor het draaien
            float mouseX = Input.GetAxisRaw("Mouse X") * lookSpeed; // Horizontale muisbeweging
            float mouseY = Input.GetAxisRaw("Mouse Y") * lookSpeed; // Verticale muisbeweging

            // Verticale rotatie van de camera (omhoog/omlaag)
            rotationX -= mouseY; // Negatief voor omhoog/omlaag
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // Beperk de rotatie
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Horizontale rotatie van de speler (links/rechts)
            rotationY += mouseX; // De rotatie van de speler rond de Y-as (360 graden mogelijk)
            transform.localRotation = Quaternion.Euler(0, rotationY, 0); // Draai de speler rond de Y-as
        }

        // Zorg ervoor dat de cursor altijd in het midden blijft
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}