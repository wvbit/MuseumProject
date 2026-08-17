using UnityEngine;
using UnityEngine.InputSystem;

public class ReadNote : MonoBehaviour
{
    public GameObject notePanel;

    private bool playerNear = false;
    private bool reading = false;

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (playerNear && !reading && Keyboard.current.eKey.wasPressedThisFrame)
        {
            reading = true;
            notePanel.SetActive(true);

            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;
        }
        else if (reading &&
                (Keyboard.current.eKey.wasPressedThisFrame ||
                 Keyboard.current.escapeKey.wasPressedThisFrame))
        {
            reading = false;
            notePanel.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}