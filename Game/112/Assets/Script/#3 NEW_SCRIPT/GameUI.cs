using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text savedStudentsText;
    [SerializeField] private GameObject statsPanel;

    [Header("Animation")]
    [SerializeField] private Color bonusScoreColor = Color.yellow;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            // Передаем ссылки на UI элементы в GameManager
            GameManager.Instance.scoreText = scoreText;
            GameManager.Instance.timerText = timerText;
            GameManager.Instance.savedStudentsText = savedStudentsText;
        }
        else
        {
            Debug.LogWarning("GameManager не найден на сцене!");
        }
    }

    // Метод для анимации получения очков (можно реализовать позже)
    private void AnimateScoreChange(int oldScore, int newScore)
    {
        // Здесь можно добавить анимацию изменения очков
        // Например, плавное увеличение числа или эффект мигания
    }
} 
