using UnityEngine;
using UnityEngine.InputSystem;

public class Medicine : MonoBehaviour
{
    public bool isCorrect = false;
    public float distance = 3f;
    public Camera playerCamera;

    bool gameOver = false;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (gameOver || playerCamera == null)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (Physics.Raycast(
                playerCamera.transform.position,
                playerCamera.transform.forward,
                out RaycastHit hit,
                distance))
            {
                if (hit.collider.GetComponentInParent<Medicine>() == this)
                {
                    TakeMedicine();
                }
            }
        }
    }

    void TakeMedicine()
    {
        if (isCorrect)
        {
            Debug.Log("CORRECT MEDICINE! YOU WIN!");
            gameOver = true;
        }
        else
        {
            Debug.Log("WRONG MEDICINE! GAME OVER!");

            gameOver = true;
            Time.timeScale = 0f;

            gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        if (playerCamera == null)
            return;

        // نص أخذ الدواء
        if (!gameOver &&
            Physics.Raycast(
                playerCamera.transform.position,
                playerCamera.transform.forward,
                out RaycastHit hit,
                distance))
        {
            if (hit.collider.GetComponentInParent<Medicine>() == this)
            {
                GUI.color = Color.white;

                GUI.Label(
                    new Rect(
                        Screen.width / 2 - 150,
                        Screen.height - 180,
                        300,
                        50
                    ),
                    "PRESS E TO TAKE"
                );
            }
        }

        // شاشة الخسارة
        if (Time.timeScale == 0f)
        {
            GUI.color = Color.red;

            GUI.Label(
                new Rect(
                    Screen.width / 2 - 200,
                    Screen.height / 2 - 50,
                    400,
                    100
                ),
                "GAME OVER"
            );

            GUI.color = Color.white;
        }
    }
}