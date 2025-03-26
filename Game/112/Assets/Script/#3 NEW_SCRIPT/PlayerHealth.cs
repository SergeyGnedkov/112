using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthRegenRate = 5f;
    [SerializeField] private float regenDelay = 3f;
    [SerializeField] private float fireResistance = 1f; // Множитель сопротивления урону от огня (1 = обычный урон)

    [Header("Visual Feedback")]
    [SerializeField] private float damageFlashDuration = 0.2f;
    [SerializeField] private Color damageFlashColor = Color.red;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged;
    public UnityEvent<float> onDamageTaken;

    private float lastDamageTime;
    private bool isDead;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    private void Update()
    {
        if (isDead) return;

        // Регенерация здоровья после определенного времени без урона
        if (Time.time - lastDamageTime > regenDelay && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + (healthRegenRate * Time.deltaTime), maxHealth);
            onHealthChanged?.Invoke(currentHealth / maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // Применяем сопротивление к урону от огня
        float actualDamage = damage * fireResistance;
        currentHealth -= actualDamage;
        lastDamageTime = Time.time;

        // Визуальный эффект получения урона
        StartCoroutine(DamageFlashRoutine());

        onDamageTaken?.Invoke(actualDamage);
        onHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(damageFlashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        onDeath?.Invoke();

        // Анимация смерти или другие эффекты
        StartCoroutine(DeathSequence());
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        // Можно добавить анимацию смерти или эффекты
        yield return new WaitForSeconds(1f);
        
        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
} 