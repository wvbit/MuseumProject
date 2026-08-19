using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // أضف اسم المشهد الخاص باللعبة من الـ Inspector
    [SerializeField] private string gameSceneName = "GameScene";

    // زر بدء اللعبة
    public void PlayGame()
    {
        // ينتقل لمشهد اللعبة
        SceneManager.LoadScene(gameSceneName);
    }

    // زر الخروج
    public void QuitGame()
    {
        Debug.Log("تم إغلاق اللعبة!"); // يظهر في الـ Console أثناء التجربة

        // إغلاق اللعبة بعد التصدير (Build)
        Application.Quit();
    }
}