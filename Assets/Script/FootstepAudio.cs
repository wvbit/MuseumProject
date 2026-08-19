using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] footstepSounds;

    public float minimumSpeed = 0.2f;
    public float slowStepInterval = 0.55f;
    public float fastStepInterval = 0.28f;
    public float fastSpeed = 6f;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 movement = transform.position - lastPosition;

        float speed = movement.magnitude / Time.deltaTime;

        lastPosition = transform.position;

        // تجاهل الحركة العمودية
        movement.y = 0;

        if (speed > minimumSpeed)
        {
            float speedPercent = Mathf.Clamp01(speed / fastSpeed);

            float interval = Mathf.Lerp(
                slowStepInterval,
                fastStepInterval,
                speedPercent
            );

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = interval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0)
            return;

        AudioClip clip =
            footstepSounds[Random.Range(0, footstepSounds.Length)];

        audioSource.PlayOneShot(clip);
    }
}