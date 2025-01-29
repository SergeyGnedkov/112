using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Extinguisher Settings")]
    [SerializeField] private float sprayRange = 5f;
    [SerializeField] private float sprayAngle = 30f;
    [SerializeField] private float sprayForce = 10f;
    [SerializeField] private GameObject foamPrefab;
    [SerializeField] private Transform sprayPoint;
    [SerializeField] private LayerMask fireLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float useDelay = 0.1f;
    [SerializeField] private int foamCount = 3;  // Количество частиц пены за выстрел

    private bool isUsing;
    private float lastUseTime;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Time.time - lastUseTime > useDelay)
        {
            Spray();
            lastUseTime = Time.time;
        }
    }

    private void Spray()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)sprayPoint.position).normalized;

        // Создаем несколько частиц пены в конусе распыления
        for (int i = 0; i < foamCount; i++)
        {
            float randomAngle = Random.Range(-sprayAngle / 2f, sprayAngle / 2f);
            Vector2 spreadDirection = Quaternion.Euler(0, 0, randomAngle) * direction;
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(sprayPoint.position, spreadDirection, sprayRange);
            
            bool hitObstacle = false;
            Vector2 foamPosition = (Vector2)sprayPoint.position + spreadDirection * sprayRange;

            foreach (RaycastHit2D hit in hits)
            {
                // Если попали в препятствие, останавливаем пену
                if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
                {
                    foamPosition = hit.point;
                    hitObstacle = true;
                    break;
                }
                
                // Если попали в огонь
                FireSystem fireSystem = hit.collider.GetComponent<FireSystem>();
                if (fireSystem != null)
                {
                    fireSystem.ExtinguishFire();
                    foamPosition = hit.point;
                }
            }

            if (!hitObstacle)
            {
                // Создаем пену
                GameObject foam = Instantiate(foamPrefab, foamPosition, Quaternion.identity);
                ExtinguisherFoam foamScript = foam.GetComponent<ExtinguisherFoam>();
                if (foamScript != null)
                {
                    foamScript.Initialize(spreadDirection * sprayForce);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (sprayPoint != null)
        {
            Gizmos.color = Color.blue;
            Vector2 direction = transform.right;
            Vector2 leftDirection = Quaternion.Euler(0, 0, sprayAngle) * direction;
            Vector2 rightDirection = Quaternion.Euler(0, 0, -sprayAngle) * direction;

            Gizmos.DrawLine(sprayPoint.position, sprayPoint.position + (Vector3)(leftDirection * sprayRange));
            Gizmos.DrawLine(sprayPoint.position, sprayPoint.position + (Vector3)(rightDirection * sprayRange));
            
            // Рисуем дополнительные линии для визуализации конуса распыления
            for (int i = 1; i < 5; i++)
            {
                float t = i / 5f;
                Vector2 middleDirection = Vector2.Lerp(leftDirection, rightDirection, t);
                Gizmos.DrawLine(sprayPoint.position, sprayPoint.position + (Vector3)(middleDirection * sprayRange));
            }
        }
    }
} 