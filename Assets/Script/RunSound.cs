using UnityEngine;
using UnityEngine.InputSystem;

public class RunSound : MonoBehaviour
{
    public AudioSource walkAudio;
    public AudioSource runAudio;

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

        // ركض
        if (isRunning)
        {
            if (walkAudio != null && walkAudio.isPlaying)
                walkAudio.Stop();

            if (runAudio != null && !runAudio.isPlaying)
                runAudio.Play();
        }

        // مشي
        else if (isMoving)
        {
            if (runAudio != null && runAudio.isPlaying)
                runAudio.Stop();

            if (walkAudio != null && !walkAudio.isPlaying)
                walkAudio.Play();
        }

        // واقف
        else
        {
            if (walkAudio != null && walkAudio.isPlaying)
                walkAudio.Stop();

            if (runAudio != null && runAudio.isPlaying)
                runAudio.Stop();
        }
    }
}