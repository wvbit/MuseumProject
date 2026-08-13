using UnityEngine;
using System.Collections.Generic;

public class DoorSystem : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 3f;

    private bool playerNear = false;
    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        if (isOpen)
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                openRotation,
                Time.deltaTime * speed
            );
        else
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                closedRotation,
                Time.deltaTime * speed
            );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }

    void OnGUI()
    {
        if (playerNear && !isOpen)
        {
            GUI.Label(
                new Rect(Screen.width / 2 - 100, Screen.height - 150, 250, 50),
                "Press E to open"
            );
        }
    }
}