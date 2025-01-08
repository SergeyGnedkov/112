using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private float closeSpeed = 2f;
    [SerializeField] private float playerDetectionRadius = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
    private bool isPlayerNear;
    private float targetAngle;
    private Transform player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = false;
        targetAngle = closeAngle;
    }

    private void Update()
    {
        // Проверяем наличие игрока рядом
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer);
        isPlayerNear = playerCollider != null;

        if (isPlayerNear)
        {
            // Определяем, с какой стороны находится игрок
            player = playerCollider.transform;
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float dot = Vector2.Dot(transform.right, directionToPlayer);
            targetAngle = dot > 0 ? openAngle : -openAngle;
        }
        else
        {
            targetAngle = closeAngle;
        }

        // Плавно поворачиваем дверь
        float currentSpeed = isPlayerNear ? openSpeed : closeSpeed;
        float newRotation = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * currentSpeed);
        rb.MoveRotation(newRotation);
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса обнаружения в редакторе
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
} 