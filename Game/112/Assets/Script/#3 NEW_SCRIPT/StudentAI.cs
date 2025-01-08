using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

    [Header("Following Settings")]
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float interactionRadius = 2f;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool isFollowingPlayer;
    private Transform playerTransform;
    private bool isWandering;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = rb.position;
        StartCoroutine(WanderRoutine());
    }

    private void Update()
    {
        // Проверяем взаимодействие с игроком
        if (!isFollowingPlayer && Input.GetKeyDown(KeyCode.E))
        {
            CheckPlayerInteraction();
        }

        // Обновляем движение
        if (isFollowingPlayer && playerTransform != null)
        {
            FollowPlayer();
        }
        else if (!isWandering)
        {
            MoveToTarget();
        }
    }

    private void CheckPlayerInteraction()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                isFollowingPlayer = true;
                playerTransform = collider.transform;
                StopCoroutine(WanderRoutine());
                break;
            }
        }
    }

    private void FollowPlayer()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > followDistance)
        {
            Vector2 targetPos = (Vector2)playerTransform.position - directionToPlayer.normalized * followDistance;
            Vector2 movement = ((targetPos - rb.position).normalized * followSpeed);
            rb.velocity = movement;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void MoveToTarget()
    {
        if ((targetPosition - rb.position).sqrMagnitude > 0.1f)
        {
            Vector2 movement = (targetPosition - rb.position).normalized * moveSpeed;
            rb.velocity = movement;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
} 