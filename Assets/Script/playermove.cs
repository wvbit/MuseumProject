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

    // الجديد فقط
    public Transform playerCamera;
    public float maxLookAngle = 80f;

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private Vector3 movement;

    private float playerRotation;
    private float cameraRotation;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        // اللاعب لا يدور بسبب الاصطدامات أو الفيزياء
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;

        rb.angularVelocity = Vector3.zero;

        playerRotation = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float x = 0f;
        float z = 0f;

        // Movement
        if (Keyboard.current.wKey.isPressed)
            z = 1f;

        if (Keyboard.current.sKey.isPressed)
            z = -1f;

        if (Keyboard.current.aKey.isPressed)
            x = -1f;

        if (Keyboard.current.dKey.isPressed)
            x = 1f;

        movement = new Vector3(x, 0f, z).normalized;

        // Mouse
        Vector2 mouseInput = Mouse.current.delta.ReadValue();

        // يمين ويسار
        playerRotation += mouseInput.x * mouseSensitivity;

        // فوق وتحت
        cameraRotation -= mouseInput.y * mouseSensitivity;

        // منع الكاميرا من الانقلاب
        cameraRotation = Mathf.Clamp(
            cameraRotation,
            -maxLookAngle,
            maxLookAngle
        );

        // تحريك الكاميرا فوق وتحت
        if (playerCamera != null)
        {
            playerCamera.localRotation =
                Quaternion.Euler(cameraRotation, 0f, 0f);
        }

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Walk / Sprint
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

        // دوران اللاعب يمين ويسار
        Quaternion rotation =
            Quaternion.Euler(0f, playerRotation, 0f);

        rb.MoveRotation(rotation);

        // منع أي دوران ناتج عن الاصطدام
        rb.angularVelocity = Vector3.zero;

        // Check Ground
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