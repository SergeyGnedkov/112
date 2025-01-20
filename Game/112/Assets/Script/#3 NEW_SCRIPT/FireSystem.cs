using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireSystem : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private float spreadInterval = 3f;        // Интервал распространения
    [SerializeField] private float spreadRadius = 2f;          // Радиус распространения
    [SerializeField] private float spreadChance = 0.5f;        // Шанс распространения (0-1)
    [SerializeField] private int maxFirePoints = 20;           // Максимальное количество точек огня
    [SerializeField] private GameObject firePointPrefab;       // Префаб точки огня
    
    [Header("Damage Settings")]
    [SerializeField] private float damagePerSecond = 10f;      // Урон в секунду
    [SerializeField] private float damageRadius = 1.5f;        // Радиус урона
    [SerializeField] private float damageDelay = 1f;          // Задержка перед началом урона
    [SerializeField] private LayerMask playerLayer;            // Слой игрока
    [SerializeField] private LayerMask obstacleLayer;          // Слой препятствий

    [Header("Visual Settings")]
    [SerializeField] private float startupTime = 1f;           // Время появления огня
    [SerializeField] private float maxFireScale = 1.5f;        // Максимальный размер

    private List<FirePoint> activeFirePoints = new List<FirePoint>();
    private bool isExtinguished;

    private void Start()
    {
        // Создаем первую точку огня
        CreateFirePoint(transform.position);
        StartCoroutine(SpreadFireRoutine());
    }

    private void CreateFirePoint(Vector2 position)
    {
        if (activeFirePoints.Count >= maxFirePoints || isExtinguished)
            return;

        // Проверяем, нет ли препятствий
        Collider2D obstacle = Physics2D.OverlapCircle(position, 0.5f, obstacleLayer);
        if (obstacle != null)
            return;

        // Проверяем, нет ли уже огня рядом
        foreach (var firePoint in activeFirePoints)
        {
            if (Vector2.Distance(firePoint.Position, position) < 1f)
                return;
        }

        GameObject fireObj = Instantiate(firePointPrefab, position, Quaternion.identity);
        fireObj.transform.parent = transform;
        
        FirePoint newFirePoint = new FirePoint
        {
            GameObject = fireObj,
            Position = position,
            IsFullyActive = false
        };

        activeFirePoints.Add(newFirePoint);
        StartCoroutine(FirePointStartupRoutine(newFirePoint));
    }

    private IEnumerator SpreadFireRoutine()
    {
        while (!isExtinguished)
        {
            yield return new WaitForSeconds(spreadInterval);

            List<FirePoint> currentPoints = new List<FirePoint>(activeFirePoints);
            foreach (var firePoint in currentPoints)
            {
                if (Random.value < spreadChance)
                {
                    // Выбираем случайное направление для распространения
                    float randomAngle = Random.Range(0f, 360f);
                    Vector2 spreadDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.right;
                    Vector2 newPosition = firePoint.Position + spreadDirection * spreadRadius;

                    CreateFirePoint(newPosition);
                }
            }
        }
    }

    private IEnumerator FirePointStartupRoutine(FirePoint firePoint)
    {
        float currentScale = 0;
        float elapsedTime = 0;

        while (elapsedTime < startupTime && !isExtinguished)
        {
            elapsedTime += Time.deltaTime;
            currentScale = Mathf.Lerp(0, maxFireScale, elapsedTime / startupTime);
            firePoint.GameObject.transform.localScale = Vector3.one * currentScale;
            yield return null;
        }

        firePoint.IsFullyActive = true;
        yield return new WaitForSeconds(damageDelay);
        
        // Начинаем проверку урона
        StartCoroutine(DamageCheckRoutine(firePoint));
    }

    private IEnumerator DamageCheckRoutine(FirePoint firePoint)
    {
        while (!isExtinguished && firePoint.IsFullyActive)
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(firePoint.Position, damageRadius, playerLayer);
            if (playerCollider != null)
            {
                PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
            yield return null;
        }
    }

    public void ExtinguishFire()
    {
        isExtinguished = true;
        StopAllCoroutines();

        foreach (var firePoint in activeFirePoints)
        {
            if (firePoint.GameObject != null)
            {
                StartCoroutine(ExtinguishFirePointRoutine(firePoint));
            }
        }
    }

    private IEnumerator ExtinguishFirePointRoutine(FirePoint firePoint)
    {
        float currentScale = firePoint.GameObject.transform.localScale.x;
        float elapsedTime = 0;
        
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * 2;
            float newScale = Mathf.Lerp(currentScale, 0, elapsedTime);
            firePoint.GameObject.transform.localScale = Vector3.one * newScale;
            yield return null;
        }

        activeFirePoints.Remove(firePoint);
        Destroy(firePoint.GameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация радиусов в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }

    // Класс для хранения информации о точке огня
    private class FirePoint
    {
        public GameObject GameObject;
        public Vector2 Position;
        public bool IsFullyActive;
    }
} 