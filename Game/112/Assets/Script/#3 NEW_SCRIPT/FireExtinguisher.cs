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
    [SerializeField] private float useDelay = 0.1f;

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
        
        RaycastHit2D[] hits = Physics2D.CircleCastAll(sprayPoint.position, 0.5f, direction, sprayRange, fireLayer);
        
        foreach (RaycastHit2D hit in hits)
        {
            float angle = Vector2.Angle(direction, (hit.point - (Vector2)sprayPoint.position).normalized);
            
            if (angle <= sprayAngle)
            {
                FireSystem fireSystem = hit.collider.GetComponent<FireSystem>();
                if (fireSystem != null)
                {
                    fireSystem.ExtinguishFire();
                }

                GameObject foam = Instantiate(foamPrefab, hit.point, Quaternion.identity);
                ExtinguisherFoam foamScript = foam.GetComponent<ExtinguisherFoam>();
                if (foamScript != null)
                {
                    foamScript.Initialize(direction * sprayForce);
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
        }
    }
} 