using UnityEngine;
using System.Collections;

public class ExtinguisherFoam : MonoBehaviour
{
    [Header("Foam Settings")]
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float expandSpeed = 2f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float fadeSpeed = 1f;

    private Vector2 moveDirection;
    private SpriteRenderer spriteRenderer;
    private float currentLifetime;
    private bool isFading;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.zero;
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction;
        StartCoroutine(FoamLifecycle());
    }

    private void Update()
    {
        if (currentLifetime < lifetime * 0.2f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, 
                Vector3.one * maxScale, expandSpeed * Time.deltaTime);
        }
        
        if (isFading)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a = Mathf.MoveTowards(currentColor.a, 0f, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = currentColor;
            
            if (currentColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator FoamLifecycle()
    {
        currentLifetime = 0f;
        
        while (currentLifetime < lifetime)
        {
            currentLifetime += Time.deltaTime;
            
            if (currentLifetime >= lifetime * 0.7f && !isFading)
            {
                isFading = true;
            }
            
            yield return null;
        }
    }
} 