using UnityEngine;

public class Puff : MonoBehaviour
{
    [Header("Explosion Metrics")]
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] LayerMask targetsLayer;

    bool hasExploded = false;

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider2D[] casualties = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetsLayer);

        foreach (Collider2D casualty in casualties)
        {
            if (casualty.gameObject == gameObject) continue;

            if (casualty.TryGetComponent(out Puff neighbourPuff))
            {
                neighbourPuff.Explode();
            }
            else 
            {
                Destroy(casualty.gameObject);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
