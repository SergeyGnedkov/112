using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameTimer : MonoBehaviour
{
    // ������ �� ��������� ��������� UI
    public TMP_Text timerText; // ��� UnityEngine.UI.Text, ���� ����������� ����������� UI Text

    // ����� � ��������
    public static float elapsedTime = 0f; // ��������� �������� �������
    private bool isTimerRunning = true; // ���� ��� ���������� ��������

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime; // ����������� �����
            UpdateTimerText(); // ��������� ����� �� ������
        }
    }

    // ����� ��� ���������� ������ �������
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("�����: {0:00}:{1:00}", minutes, seconds); // ����������� ����� ��� MM:SS
        }
        else
        {
            Debug.LogError("timerText �� ��������!");
        }
    }

    // ����� ��� ��������� �����������
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // ����� ��� ������� �����������
    public void StartTimer()
    {
        isTimerRunning = true;
    }

    // ����� ��� ������ �����������
    public void ResetTimer()
    {
        elapsedTime = 0f; // ���������� �����
        UpdateTimerText(); // ��������� ����� �� ������
    }
    public static void SaveTimer() => PlayerPrefs.SetFloat("Timer", elapsedTime);
}