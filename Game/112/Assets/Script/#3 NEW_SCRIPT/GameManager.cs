using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int pointsPerStudent = 100;    // Очки за каждого спасенного студента

    [Header("UI References")]
    public Text scoreText;                // UI текст для отображения очков
    public Text timerText;                // UI текст для отображения времени
    public Text savedStudentsText;        // UI текст для отображения количества спасенных

    [Header("Time Bonus")]
    [SerializeField] private bool useTimeBonus = true;      // Использовать ли бонус за время
    [SerializeField] private float timeBonusThreshold = 60f;// Время для получения бонуса
    [SerializeField] private int timeBonus = 50;            // Дополнительные очки за быстрое прохождение

    private int totalScore = 0;
    private float levelTimer = 0f;
    private bool isTimerRunning = true;
    private int savedStudentsCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            levelTimer += Time.deltaTime;
            UpdateUI();
        }
    }

    public void AddSavedStudents(int count)
    {
        savedStudentsCount += count;
        int baseScore = count * pointsPerStudent;
        
        // Добавляем бонус за время, если успели быстро
        if (useTimeBonus && levelTimer < timeBonusThreshold)
        {
            totalScore += baseScore + timeBonus;
        }
        else
        {
            totalScore += baseScore;
        }
        
        UpdateUI();
    }

    public void StartNewLevel()
    {
        levelTimer = 0f;
        isTimerRunning = true;
        savedStudentsCount = 0;
        UpdateUI();
    }

    public int GetSavedStudentsCount()
    {
        return savedStudentsCount;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }

        if (timerText != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(levelTimer);
            timerText.text = $"Time: {time.Minutes:00}:{time.Seconds:00}";
        }

        if (savedStudentsText != null)
        {
            savedStudentsText.text = $"Saved: {savedStudentsCount}";
        }
    }
} 
