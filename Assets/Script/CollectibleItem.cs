using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class CollectibleItem : MonoBehaviour
{
    [Header("Medicine")]
    public bool isCorrect = false;

    [Header("Interaction")]
    public float interactionDistance = 2.5f;

    private Transform player;
    private bool collected = false;
    private bool showResult = false;
    private string resultMessage = "";

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player Tag is missing!");
        }
    }

    void Update()
    {
        if (player == null || collected)
            return;

        float distance =
            Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            TakeMedicine();
        }
    }

    void TakeMedicine()
    {
        collected = true;

        // يخفي شكل الدواء
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        showResult = true;

        if (isCorrect)
        {
            resultMessage = "YOU WIN!";
        }
        else
        {
            resultMessage = "GAME OVER";
            StartCoroutine(RestartGame());
        }
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    void OnGUI()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(player.position, transform.position);

        GUIStyle promptStyle = new GUIStyle(GUI.skin.label);
        promptStyle.alignment = TextAnchor.MiddleCenter;
        promptStyle.fontSize = 26;
        promptStyle.normal.textColor = Color.white;

        // يظهر فقط عند الاقتراب من الدواء
        if (!collected && distance <= interactionDistance)
        {
            GUI.Box(
                new Rect(
                    Screen.width / 2 - 180,
                    Screen.height - 120,
                    360,
                    50
                ),
                ""
            );

            GUI.Label(
                new Rect(
                    Screen.width / 2 - 180,
                    Screen.height - 120,
                    360,
                    50
                ),
                "Press E to Take Medicine",
                promptStyle
            );
        }

        // شاشة الفوز أو الخسارة
        if (showResult)
        {
            GUIStyle resultStyle =
                new GUIStyle(GUI.skin.label);

            resultStyle.alignment =
                TextAnchor.MiddleCenter;

            resultStyle.fontSize = 60;
            resultStyle.normal.textColor =
                Color.white;

            GUI.Box(
                new Rect(
                    0,
                    Screen.height / 2 - 100,
                    Screen.width,
                    200
                ),
                ""
            );

            GUI.Label(
                new Rect(
                    0,
                    Screen.height / 2 - 100,
                    Screen.width,
                    200
                ),
                resultMessage,
                resultStyle
            );
        }
    }
}