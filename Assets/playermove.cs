using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 0.15f;

    private Rigidbody rb;
    private Vector3 movement;
    private float mouseX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed) z = 1f;
        if (Keyboard.current.sKey.isPressed) z = -1f;
        if (Keyboard.current.aKey.isPressed) x = -1f;
        if (Keyboard.current.dKey.isPressed) x = 1f;

        movement = new Vector3(x, 0f, z).normalized;

        mouseX = Mouse.current.delta.ReadValue().x * mouseSensitivity;
    }

    void FixedUpdate()
    {
        Vector3 moveDirection =
            transform.forward * movement.z +
            transform.right * movement.x;

        rb.MovePosition(
            rb.position + moveDirection * speed * Time.fixedDeltaTime
        );

        Quaternion rotation =
            Quaternion.Euler(0f, mouseX, 0f);

        rb.MoveRotation(rb.rotation * rotation);
    }
}