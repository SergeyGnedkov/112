using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 movement;
    private Vector2 mousePosition;
    private bool isRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Получаем ввод для движения
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        // Проверяем бег
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Получаем позицию мыши в мировых координатах
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        // Движение
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.velocity = movement * currentSpeed;

        // Поворот к мыши
        Vector2 lookDirection = mousePosition - rb.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime);
    }
} 