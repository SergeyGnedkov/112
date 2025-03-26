using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitZone : MonoBehaviour
{
    [Header("Настройки перехода")]
    [SerializeField] private string nextSceneName = "";  // Имя сцены, куда нужно перейти
    [SerializeField] private float transitionDelay = 0.5f;  // Задержка перед переходом
    
    [Header("Визуализация")]
    [SerializeField] private bool showTriggerZone = true;
    [SerializeField] private Color gizmoColor = Color.green;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что в триггер вошел игрок
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вошел в зону перехода!");
            
            // Если указано имя следующей сцены, переходим на неё
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                Invoke("LoadNextScene", transitionDelay);
            }
            else
            {
                Debug.LogError("Не указано имя следующей сцены!");
            }
        }
    }

    private void LoadNextScene()
    {
        Debug.Log($"Загружаем сцену: {nextSceneName}");

        // Находим все системы огня на текущей сцене
        FireSystem[] fireSystems = FindObjectsOfType<FireSystem>();
        foreach (FireSystem fireSystem in fireSystems)
        {
            if (fireSystem != null)
            {
                fireSystem.StopAllCoroutines();
                Destroy(fireSystem.gameObject);
            }
        }

        if (nextSceneName == "MainMenu")
        {
            ScoreGet.SaveScore();
            GameTimer.SaveTimer();
        }
        // Загружаем новую сцену
        SceneManager.LoadScene(nextSceneName);
    }

    // Отрисовка зоны триггера в редакторе
    private void OnDrawGizmos()
    {
        if (!showTriggerZone) return;

        Gizmos.color = gizmoColor;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        
        if (boxCollider != null)
        {
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
    }
} 