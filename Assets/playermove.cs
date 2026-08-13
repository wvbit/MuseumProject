using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("Jump")]
    public float jumpForce = 6f;

    [Header("Mouse")]
    public float mouseSensitivity = 0.15f;

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private Vector3 movement;
    private float mouseX;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float x = 0f;
        float z = 0f;

        // Movement
        if (Keyboard.current.wKey.isPressed) z = 1f;
        if (Keyboard.current.sKey.isPressed) z = -1f;
        if (Keyboard.current.aKey.isPressed) x = -1f;
        if (Keyboard.current.dKey.isPressed) x = 1f;

        movement = new Vector3(x, 0f, z).normalized;

        // Mouse
        mouseX += Mouse.current.delta.ReadValue().x * mouseSensitivity;

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Walk or Sprint
        float currentSpeed = Keyboard.current.leftShiftKey.isPressed
            ? sprintSpeed
            : walkSpeed;

        Vector3 moveDirection =
            transform.forward * movement.z +
            transform.right * movement.x;

        rb.MovePosition(
            rb.position +
            moveDirection * currentSpeed * Time.fixedDeltaTime
        );

        // Rotate player
        Quaternion rotation = Quaternion.Euler(0f, mouseX, 0f);
        rb.MoveRotation(rb.rotation * rotation);

        // Reset mouse movement
        mouseX = 0f;

        // Check ground
        CheckGround();
    }

    void CheckGround()
    {
        Vector3 origin = capsule.bounds.center;

        float distance = capsule.bounds.extents.y + 0.15f;

        isGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            distance
        );
    }
} 