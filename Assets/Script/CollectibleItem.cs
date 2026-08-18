using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleItem : MonoBehaviour
{


    [SerializeField] private GameObject text;
    [Header("تحديد هل هذا العنصر هو الصحيح؟")]
    public bool isCorrect = false;

    private void OnTriggerEnter(Collider other)
    {
        // التأكد من أن الذي اصطدم بالعنصر هو اللاعب
        if (other.CompareTag("Player"))
        {
            text.SetActive(true);
            if (isCorrect)
            {
                WinGame();
            }
            else
            {
                RestartGame();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        text.SetActive(false);
    }

    void WinGame()
    {
        Debug.Log("فوز! لقد اخترت العنصر الصحيح.");
        // أضف هنا كود الفوز (مثل فتح مرحلة جديدة أو إظهار واجهة الفوز)
    }

    void RestartGame()
    {
        Debug.Log("إجابة خاطئة! إعادة تشغيل المرحلة...");
        // إعادة تحميل المشهد الحالي
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}