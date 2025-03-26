using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameTimer : MonoBehaviour
{
    // Ссылка на текстовый компонент UI
    public TMP_Text timerText; // Или UnityEngine.UI.Text, если используете стандартный UI Text

    // Время в секундах
    public static float elapsedTime = 0f; // Начальное значение времени
    private bool isTimerRunning = true; // Флаг для управления таймером

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime; // Увеличиваем время
            UpdateTimerText(); // Обновляем текст на экране
        }
    }

    // Метод для обновления текста таймера
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("Время: {0:00}:{1:00}", minutes, seconds); // Форматируем время как MM:SS
        }
        else
        {
            Debug.LogError("timerText не назначен!");
        }
    }

    // Метод для остановки секундомера
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // Метод для запуска секундомера
    public void StartTimer()
    {
        isTimerRunning = true;
    }

    // Метод для сброса секундомера
    public void ResetTimer()
    {
        elapsedTime = 0f; // Сбрасываем время
        UpdateTimerText(); // Обновляем текст на экране
    }
    public static void SaveTimer() => PlayerPrefs.SetFloat("Timer", elapsedTime);
}