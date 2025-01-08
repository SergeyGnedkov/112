using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitZone : MonoBehaviour
{
    [Header("Exit Settings")]
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private float transitionDelay = 1f;
    [SerializeField] private bool requireAllStudents = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (requireAllStudents)
            {
                // Проверяем, все ли студенты следуют за игроком
                StudentAI[] allStudents = FindObjectsOfType<StudentAI>();
                bool allStudentsFollowing = true;

                foreach (StudentAI student in allStudents)
                {
                    if (!student.isFollowingPlayer)
                    {
                        allStudentsFollowing = false;
                        break;
                    }
                }

                if (allStudentsFollowing)
                {
                    LoadMainMenu();
                }
            }
            else
            {
                LoadMainMenu();
            }
        }
    }

    private void LoadMainMenu()
    {
        // Можно добавить здесь эффекты перехода или анимацию
        Invoke("LoadMenu", transitionDelay);
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnDrawGizmos()
    {
        // Визуализация зоны выхода в редакторе
        Gizmos.color = Color.green;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
    }
} 