using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthRegenRate = 5f;
    [SerializeField] private float regenDelay = 3f;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged;
    public UnityEvent<float> onDamageTaken;

    private float lastDamageTime;
    private bool isDead;

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

        currentHealth -= damage;
        lastDamageTime = Time.time;

        onDamageTaken?.Invoke(damage);
        onHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
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

        // Здесь дописывать. Можно добавить дополнительную логику смерти
        // перезагрузку уровня или другие эффекты
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
} 