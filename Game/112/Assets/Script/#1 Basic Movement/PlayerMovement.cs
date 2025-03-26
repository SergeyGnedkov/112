using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera mainCamera;

    // Публичные переменные для других скриптов
    public bool moving { get; private set; }

    private Vector2 movement;
    private Vector2 mousePosition;
    private bool isSprinting;

    private void Start()
    {
        // Получаем компоненты, если они не назначены
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (mainCamera == null) mainCamera = Camera.main;

        // Настраиваем Rigidbody2D для Top Down движения
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        // Получаем ввод для движения
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize(); // Нормализуем вектор для диагонального движения

        // Обновляем состояние движения
        moving = movement.magnitude > 0.1f;

        // Получаем позицию мыши в мировых координатах
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Проверяем спринт
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    private void FixedUpdate()
    {
        // Движение персонажа
        float currentSpeed = isSprinting ? moveSpeed * sprintSpeedMultiplier : moveSpeed;
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);

        // Поворот персонажа к мыши
        Vector2 lookDirection = mousePosition - rb.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime);
    }

    // Метод для внешнего управления движением
    public void setMoving(bool val)
    {
        moving = val;
        if (!moving)
        {
            movement = Vector2.zero;
        }
    }

    // Получить текущую скорость движения
    public float GetCurrentSpeed()
    {
        return isSprinting ? moveSpeed * sprintSpeedMultiplier : moveSpeed;
    }
}