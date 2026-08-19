using UnityEngine;
using UnityEngine.InputSystem;

public class DoorSystem : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public float distance = 3f;
    public Camera playerCamera;

    bool open;
    Quaternion closed, opened;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        closed = transform.localRotation;
        opened = closed * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            open ? opened : closed,
            Time.deltaTime * openSpeed
        );

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            playerCamera != null)
        {
            if (Physics.Raycast(
                playerCamera.transform.position,
                playerCamera.transform.forward,
                out RaycastHit hit,
                distance))
            {
                DoorSystem door =
                    hit.collider.GetComponentInParent<DoorSystem>();

                if (door == this)
                    open = !open;
            }
        }
    }

    void OnGUI()
    {
        if (playerCamera == null)
            return;

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out RaycastHit hit,
            distance))
        {
            DoorSystem door =
                hit.collider.GetComponentInParent<DoorSystem>();

            if (door == this)
            {
               

            }
        }
    }
}