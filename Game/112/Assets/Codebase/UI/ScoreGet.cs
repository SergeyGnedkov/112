using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreGet : MonoBehaviour
{
    // ������ �� ��������� ��������� UI
    public TMP_Text scoreText; // ��� UnityEngine.UI.Text, ���� ����������� ����������� UI Text
    
    // ���������� ��� �������� �������� �����
    public static int score = 0;
    public static void SaveScore()
    {
        PlayerPrefs.SetInt("Score", score);
    }
    // ����� ��� ���������� ��������
    public void AddScore()
    {
        score++; // ����������� �������
        UpdateScoreText(); // ��������� ����� �� ������
    }

    // ����� ��� ���������� ������ �� ������
    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "�������: " + score; // ��������� �����
        }
        else
        {
            Debug.LogError("scoreText �� ��������!");
        }
    }

    // ������������� ���������� �������� ��������
    private void Start()
    {
        UpdateScoreText(); // ������������� ��������� ��������
    }
}
