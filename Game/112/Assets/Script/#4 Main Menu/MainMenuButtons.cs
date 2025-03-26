using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
}