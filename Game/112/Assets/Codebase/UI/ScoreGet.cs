using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreGet : MonoBehaviour
{
    // Ссылка на текстовый компонент UI
    public TMP_Text scoreText; // Или UnityEngine.UI.Text, если используете стандартный UI Text
    
    // Переменная для хранения текущего счета
    public static int score = 0;
    public static void SaveScore()
    {
        PlayerPrefs.SetInt("Score", score);
    }
    // Метод для увеличения счетчика
    public void AddScore()
    {
        score++; // Увеличиваем счетчик
        UpdateScoreText(); // Обновляем текст на экране
    }

    // Метод для обновления текста на экране
    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Спасено: " + score; // Обновляем текст
        }
        else
        {
            Debug.LogError("scoreText не назначен!");
        }
    }

    // Инициализация начального значения счетчика
    private void Start()
    {
        UpdateScoreText(); // Устанавливаем начальное значение
    }
}
