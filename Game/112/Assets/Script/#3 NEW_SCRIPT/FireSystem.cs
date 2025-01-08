using UnityEngine;
using System.Collections;

public class FireSystem : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private float fireSpreadInterval = 5f;
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float damageRadius = 2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject fireEffectPrefab;

    [Header("Visual Settings")]
    [SerializeField] private float maxFireScale = 1.5f;
    [SerializeField] private float growthSpeed = 2f;

    private bool isFireActive;
    private GameObject currentFireEffect;
    private float currentScale;

    private void Start()
    {
        StartCoroutine(FireTimer());
    }

    private IEnumerator FireTimer()
    {
        while (true)
        {
            // Ждем заданный интервал
            yield return new WaitForSeconds(fireSpreadInterval);

            // Активируем огонь
            ActivateFire();

            // Ждем некоторое время, пока огонь активен
            yield return new WaitForSeconds(fireSpreadInterval / 2f);

            // Деактивируем огонь
            DeactivateFire();
        }
    }

    private void ActivateFire()
    {
        isFireActive = true;
        if (fireEffectPrefab != null && currentFireEffect == null)
        {
            currentFireEffect = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
            currentFireEffect.transform.parent = transform;
            currentScale = 0f;
        }
    }

    private void DeactivateFire()
    {
        isFireActive = false;
        if (currentFireEffect != null)
        {
            Destroy(currentFireEffect);
            currentFireEffect = null;
        }
    }

    private void Update()
    {
        if (isFireActive)
        {
            // Увеличиваем масштаб эффекта огня
            if (currentFireEffect != null && currentScale < maxFireScale)
            {
                currentScale = Mathf.MoveTowards(currentScale, maxFireScale, growthSpeed * Time.deltaTime);
                currentFireEffect.transform.localScale = Vector3.one * currentScale;
            }

            // Проверяем попадание игрока в зону огня
            Collider2D playerInFire = Physics2D.OverlapCircle(transform.position, damageRadius * currentScale, playerLayer);
            if (playerInFire != null)
            {
                // Наносим урон игроку
                PlayerHealth playerHealth = playerInFire.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса урона в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
} 