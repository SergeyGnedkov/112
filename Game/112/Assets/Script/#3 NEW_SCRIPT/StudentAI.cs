using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class StudentAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minWanderWaitTime = 1f;
    [SerializeField] private float maxWanderWaitTime = 3f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float rotationSpeed = 5f;         // Скорость поворота
    [SerializeField] private float avoidanceRadius = 1.5f;     // Радиус избегания препятствий

    [Header("Following Settings")]
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float interactionRadius = 3f;     // Увеличил радиус взаимодействия
    [SerializeField] private float maxFollowDistance = 10f;
    [SerializeField] private float teleportOffset = 1f;
    [SerializeField] private float pathUpdateRate = 0.2f;      // Как часто обновляем путь
    
    [Header("Visual Effects")]
    [SerializeField] private float teleportFlashDuration = 0.2f;
    [SerializeField] private Color teleportFlashColor = Color.cyan;
    [SerializeField] private bool showDebugGizmos = true;     // Для отладки

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool isFollowingPlayer;
    private Transform playerTransform;
    private bool isWandering;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector2 currentVelocity;
    private float currentRotation;
    private float pathUpdateTimer;
    private ScoreGet scoreManager;
    [SerializeField] private SpriteRenderer AIspriteRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Animator animator;
    // Публичное свойство для доступа к isFollowingPlayer
    public bool IsFollowingPlayer => isFollowingPlayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        targetPosition = rb.position;
        StartCoroutine(WanderRoutine());

        // Находим игрока сразу при старте
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        scoreManager = FindObjectOfType<ScoreGet>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreGet не найден в сцене!");
        }
    }

    private void Update()
    {
        // Проверяем взаимодействие с игроком
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckPlayerInteraction();
        }

        // Обновляем путь
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateRate)
        {
            pathUpdateTimer = 0f;
            UpdateTargetPosition();
        }
    }

    private void FixedUpdate()
    {
        // Движение в физическом обновлении
        if (isFollowingPlayer && playerTransform != null)
        {
            FollowPlayer();
            animator.enabled = true;
        }
        else if (!isWandering)
        {
            MoveToTarget();
            animator.enabled = true;
        }
        else
        {
                animator.enabled = false;
                spriteRenderer.sprite = idleSprite;
        }

        // Обновляем поворот
        UpdateRotation();

    }

    private void UpdateTargetPosition()
    {
        if (isFollowingPlayer && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > maxFollowDistance)
            {
                TeleportToPlayer();
            }
        }
    }

    public void CheckPlayerInteraction()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= interactionRadius)
        {
            if (!isFollowingPlayer && scoreManager != null)
            {
                scoreManager.AddScore(); // Увеличиваем счетчик
            }
            isFollowingPlayer = !isFollowingPlayer;
            if (isFollowingPlayer)
            {
                StopCoroutine(WanderRoutine());
                StartCoroutine(TeleportFlashEffect());
            }
            else
            {
                StartCoroutine(WanderRoutine());
            }
        }
    }

    private void FollowPlayer()
    {
        if (playerTransform == null) return;

        Vector2 directionToPlayer = ((Vector2)playerTransform.position - rb.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > followDistance)
        {
            // Вычисляем желаемую позицию
            Vector2 desiredPosition = (Vector2)playerTransform.position - directionToPlayer.normalized * followDistance;
            
            // Проверяем препятствия и избегаем их
            Vector2 avoidanceForce = CalculateAvoidanceForce();
            
            // Комбинируем силы
            Vector2 moveDirection = (desiredPosition - rb.position).normalized;
            Vector2 finalVelocity = (moveDirection + avoidanceForce).normalized * followSpeed;

            // Плавно меняем скорость
            rb.velocity = Vector2.SmoothDamp(rb.velocity, finalVelocity, ref currentVelocity, 0.1f);
        }
        else
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref currentVelocity, 0.1f);
        }
    }

    private Vector2 CalculateAvoidanceForce()
    {
        Vector2 avoidanceForce = Vector2.zero;
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius, obstacleLayer);

        foreach (Collider2D obstacle in obstacles)
        {
            Vector2 directionToObstacle = (Vector2)obstacle.transform.position - rb.position;
            float distance = directionToObstacle.magnitude;
            avoidanceForce += -directionToObstacle.normalized * (avoidanceRadius - distance) / avoidanceRadius;
        }

        return avoidanceForce.normalized;
    }

    private void UpdateRotation()
    {
        if (rb.velocity.sqrMagnitude > 0.1f)
        {
            float targetRotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg - 90f;
            currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }

    private void TeleportToPlayer()
    {
        if (playerTransform == null) return;

        // Находим позицию для телепортации
        Vector2 directionToPlayer = ((Vector2)playerTransform.position - rb.position).normalized;
        Vector2 teleportPosition = (Vector2)playerTransform.position - directionToPlayer * teleportOffset;

        // Проверяем, нет ли препятствий в точке телепортации
        Collider2D obstacle = Physics2D.OverlapCircle(teleportPosition, 0.5f, obstacleLayer);
        if (obstacle == null)
        {
            transform.position = teleportPosition;
            StartCoroutine(TeleportFlashEffect());
        }
    }

    private IEnumerator TeleportFlashEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = teleportFlashColor;
            yield return new WaitForSeconds(teleportFlashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    private void MoveToTarget()
    {
        if ((targetPosition - rb.position).sqrMagnitude > 0.1f)
        {
            Vector2 direction = (targetPosition - rb.position).normalized;
            
            // Проверяем препятствия
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, obstacleLayer);
            if (hit.collider == null)
            {
                rb.velocity = direction * moveSpeed;
            }
            else
            {
                // Если встретили препятствие, выбираем новую точку
                targetPosition = rb.position;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            isWandering = false;
            yield return new WaitForSeconds(Random.Range(minWanderWaitTime, maxWanderWaitTime));

            if (!isFollowingPlayer)
            {
                isWandering = true;
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                Vector2 potentialTarget = (Vector2)transform.position + randomDirection * wanderRadius;

                // Проверяем, нет ли препятствий
                RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDirection, wanderRadius, obstacleLayer);
                if (!hit)
                {
                    targetPosition = potentialTarget;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Визуализация всех важных радиусов
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        if (isFollowingPlayer && playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxFollowDistance);
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
} 