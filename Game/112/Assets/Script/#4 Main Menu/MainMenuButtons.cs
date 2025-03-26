using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public TMP_Text scoreText;
    [SerializeField] public TMP_Text timerText;
    // Метод для кнопки "Старт"
    public void PlayGame()
    {
        // Загрузить следующую сцену (можно изменить индекс сцены)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Метод для кнопки "Настройки"
    public void OpenSettings()
    {
        // Здесь можно открыть окно настроек
        Debug.Log("Открыть настройки!");
        // В этом примере просто выводим сообщение в консоль
    }

    // Метод для кнопки "Выход"
    public void QuitGame()
    {
        Debug.Log("Игра закрыта!");
        // Закрыть игру (будет работать только в билде, не в редакторе)
        Application.Quit();
    }
    public void DrawScore()
    {
        if (PlayerPrefs.HasKey("Score"))
        {
            scoreText.text = "Лучший счёт: " + PlayerPrefs.GetInt("Score").ToString();
        }
        else
        {
            scoreText.text = "Лучший счёт: " + "0";
        }
    }

    public void DrawTimer()
    {
        if (PlayerPrefs.HasKey("Timer"))
        {
            int minutes = Mathf.FloorToInt(PlayerPrefs.GetFloat("Timer")/60);
            int seconds = Mathf.FloorToInt(PlayerPrefs.GetFloat("Timer") % 60);
            timerText.text = "Лучшее время: " + minutes.ToString() +":"+ seconds.ToString();
        }
        else
        {
            timerText.text = "Лучшее время: " + "0";
        }
    }
    public void Awake()
    {
        DrawScore();
        DrawTimer();
    }
}