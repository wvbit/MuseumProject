using UnityEngine;
using UnityEngine.InputSystem;

public class DoorOpen : MonoBehaviour
{
    [Header("Door")]
    public Transform door;
    public float openAngle = 90f;
    public float openSpeed = 3f;

    [Header("Interaction")]
    public float interactionDistance = 3f;
    public Transform player;

    [Header("Sound")]
    public AudioSource doorAudio;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = door.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        if (player == null || Keyboard.current == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            isOpen = !isOpen;

            if (doorAudio != null)
            {
                doorAudio.Stop();
                doorAudio.Play();
            }
        }

        Quaternion targetRotation =
            isOpen ? openRotation : closedRotation;

        door.localRotation = Quaternion.Slerp(
            door.localRotation,
            targetRotation,
            openSpeed * Time.deltaTime
        );
    }

    void OnGUI()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 28;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            string message =
                isOpen ? "Press E to Close" : "Press E to Open";

            GUI.Label(
                new Rect(
                    Screen.width / 2 - 200,
                    Screen.height - 120,
                    400,
                    50
                ),
                message,
                style
            );
        }
    }
}