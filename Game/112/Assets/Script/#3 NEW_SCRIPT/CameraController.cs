using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Following Settings")]
    [SerializeField] private Transform target; // Игрок, за которым следует камера
    [SerializeField] private float smoothSpeed = 5f; // Скорость сглаживания движения
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Смещение камеры относительно игрока
    
    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = false; // Использовать ли границы для камеры
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;

    private void Start()
    {
        // Если цель не назначена, попробуем найти игрока
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        // Вычисляем желаемую позицию камеры
        Vector3 desiredPosition = target.position + offset;
        
        // Если используются границы, ограничиваем позицию камеры
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Плавно перемещаем камеру к желаемой позиции
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPosition;
    }

    // Метод для установки цели слежения
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Метод для установки границ камеры
    public void SetBoundaries(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        useBoundaries = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (useBoundaries)
        {
            // Визуализация границ в редакторе
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
} 