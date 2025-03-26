using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireSystem : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private float spreadInterval = 1.5f;        // Уменьшил интервал для более частого распространения
    [SerializeField] private float spreadRadius = 2f;
    [SerializeField] private float spreadChance = 0.7f;         // Увеличил шанс распространения
    [SerializeField] private float neighborBonus = 0.1f;
    [SerializeField] private float neighborCheckRadius = 3f;
    [SerializeField] private int maxFireCount = 50;             // Увеличил максимальное количество
    
    [Header("Visual Settings")]
    [SerializeField] private float fadeSpeed = 2f;              // Скорость затухания при тушении
    [SerializeField] private float startScale = 1f;             // Начальный размер огня
    [SerializeField] private float growthSpeed = 2f;            // Скорость роста огня

    [Header("Damage Settings")]
    [SerializeField] private float damagePerSecond = 25f;
    [SerializeField] private float damageRadius = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private static List<FireSystem> allFirePoints = new List<FireSystem>();
    private bool isMainFire;
    private bool isExtinguished;
    private SpriteRenderer spriteRenderer;
    private float currentScale;
    private bool isBeingExtinguished;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentScale = 0f;
        transform.localScale = Vector3.zero;

        // Настраиваем Rigidbody2D для Top Down игры
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }
        rb2d.gravityScale = 0f;      // Отключаем гравитацию
        rb2d.isKinematic = true;     // Включаем кинематику
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll; // Замораживаем все движения
    }

    private void OnEnable()
    {
        // Подписываемся на событие смены сцены
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Отписываемся от события
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Очищаем список при загрузке новой сцены
        allFirePoints.Clear();
        
        // Переинициализируем огонь
        InitializeFire();
    }

    private void InitializeFire()
    {
        StopAllCoroutines();
        
        if (!allFirePoints.Contains(this))
        {
            allFirePoints.Add(this);
        }

        if (allFirePoints.Count == 1)
        {
            isMainFire = true;
            StartCoroutine(SpreadFireRoutine());
        }

        StartCoroutine(DamageRoutine());
        StartCoroutine(GrowthRoutine());
    }

    private void Start()
    {
        InitializeFire();
    }

    private IEnumerator GrowthRoutine()
    {
        while (!isExtinguished && currentScale < startScale)
        {
            currentScale = Mathf.MoveTowards(currentScale, startScale, growthSpeed * Time.deltaTime);
            transform.localScale = Vector3.one * currentScale;
            yield return null;
        }
    }

    private void CreateFirePoint(Vector2 position)
    {
        if (allFirePoints.Count >= maxFireCount || isExtinguished)
            return;

        // Проверяем стены через raycast от текущего огня до новой позиции
        RaycastHit2D hit = Physics2D.Raycast(transform.position, position - (Vector2)transform.position, 
            Vector2.Distance(transform.position, position), obstacleLayer);
        
        if (hit.collider != null)
            return; // Если луч попал в стену, не создаём огонь

        // Проверяем, нет ли препятствий в точке создания
        Collider2D obstacle = Physics2D.OverlapCircle(position, 0.3f, obstacleLayer);
        if (obstacle != null)
            return;

        // Проверяем минимальное расстояние до других огней
        foreach (var fire in allFirePoints)
        {
            if (Vector2.Distance((Vector2)fire.transform.position, position) < 1f)
                return;
        }

        // Создаем новый огонь на указанной позиции
        GameObject newFire = Instantiate(gameObject, new Vector3(position.x, position.y, 0f), Quaternion.identity);
        FireSystem fireSystem = newFire.GetComponent<FireSystem>();
        fireSystem.isMainFire = false;
    }

    private IEnumerator SpreadFireRoutine()
    {
        while (!isExtinguished)
        {
            if (!isMainFire) break;

            foreach (var fire in new List<FireSystem>(allFirePoints))
            {
                if (fire == null || fire.isExtinguished) continue;

                float spreadChanceWithBonus = CalculateSpreadChance(fire.transform.position);
                
                if (Random.value < spreadChanceWithBonus)
                {
                    int spreadAttempts = Random.Range(1, 4);
                    for (int i = 0; i < spreadAttempts; i++)
                    {
                        float randomAngle = Random.Range(0f, 360f);
                        float randomDistance = Random.Range(1f, spreadRadius);
                        Vector2 spreadDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.right;
                        Vector2 newPosition = (Vector2)fire.transform.position + spreadDirection * randomDistance;

                        CreateFirePoint(newPosition);
                    }
                }
            }

            yield return new WaitForSeconds(spreadInterval);
        }
    }

    public void ExtinguishFire()
    {
        if (isBeingExtinguished) return;
        
        isBeingExtinguished = true;
        StartCoroutine(ExtinguishRoutine());
    }

    private IEnumerator ExtinguishRoutine()
    {
        while (currentScale > 0)
        {
            currentScale = Mathf.MoveTowards(currentScale, 0, fadeSpeed * Time.deltaTime);
            transform.localScale = Vector3.one * currentScale;
            
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = currentScale / startScale;
                spriteRenderer.color = color;
            }

            if (currentScale <= 0)
            {
                isExtinguished = true;
                
                if (isMainFire)
                {
                    foreach (var fire in new List<FireSystem>(allFirePoints))
                    {
                        if (fire != this && fire != null && !fire.isBeingExtinguished)
                        {
                            fire.ExtinguishFire();
                        }
                    }
                }
                
                allFirePoints.Remove(this);
                Destroy(gameObject);
                break;
            }
            
            yield return null;
        }
    }

    private float CalculateSpreadChance(Vector2 position)
    {
        float finalChance = spreadChance;
        int neighborCount = 0;

        foreach (var fire in allFirePoints)
        {
            if (fire != this)
            {
                float distance = Vector2.Distance(position, fire.transform.position);
                if (distance <= neighborCheckRadius)
                {
                    neighborCount++;
                }
            }
        }

        finalChance += neighborCount * neighborBonus;
        return Mathf.Clamp01(finalChance);
    }

    private IEnumerator DamageRoutine()
    {
        while (!isExtinguished)
        {
            // Проверяем, есть ли игрок в радиусе урона
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, damageRadius, playerLayer);
            
            foreach (Collider2D hitCollider in hitColliders)
            {
                PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
            
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса урона
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
        
        // Визуализация радиуса распространения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }
} 