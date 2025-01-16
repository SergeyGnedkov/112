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
    private bool isExtinguished;

    private void Start()
    {
        StartCoroutine(FireTimer());
    }

    private IEnumerator FireTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireSpreadInterval);

            if (!isExtinguished)
            {
                ActivateFire();
                yield return new WaitForSeconds(fireSpreadInterval / 2f);
                DeactivateFire();
            }
        }
    }

    private void ActivateFire()
    {
        if (isExtinguished) return;
        
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

    public void ExtinguishFire()
    {
        isExtinguished = true;
        isFireActive = false;
        if (currentFireEffect != null)
        {
            Destroy(currentFireEffect);
            currentFireEffect = null;
        }
        StopAllCoroutines();
    }

    private void Update()
    {
        if (isFireActive && !isExtinguished)
        {
            if (currentFireEffect != null && currentScale < maxFireScale)
            {
                currentScale = Mathf.MoveTowards(currentScale, maxFireScale, growthSpeed * Time.deltaTime);
                currentFireEffect.transform.localScale = Vector3.one * currentScale;
            }

            Collider2D playerInFire = Physics2D.OverlapCircle(transform.position, damageRadius * currentScale, playerLayer);
            if (playerInFire != null)
            {
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
} 