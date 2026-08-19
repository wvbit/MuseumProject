using UnityEngine;
using UnityEngine.InputSystem;

public class BreathingSound : MonoBehaviour
{
    public AudioSource breathingAudio;

    void Update()
    {
        if (Keyboard.current == null)
            return;

        bool isMoving =
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed;

        bool isRunning =
            isMoving && Keyboard.current.leftShiftKey.isPressed;

        if (isRunning)
        {
            if (!breathingAudio.isPlaying)
                breathingAudio.Play();
        }
        else
        {
            if (breathingAudio.isPlaying)
                breathingAudio.Stop();
        }
    }
}