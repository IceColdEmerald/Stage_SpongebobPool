using UnityEngine;

public class PlanktonMovement : MonoBehaviour
{
    [Header("Movement Control")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float boundaryRange = 4f;

    Vector2 spawnOrigin;
    int movingDirection = 1;
    bool clearToMove = true;
    SpriteRenderer customRenderer;

    void Start()
    {
        spawnOrigin = transform.position;
        customRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!clearToMove) return;

        transform.Translate(Vector2.right * movingDirection * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - spawnOrigin.x) >= boundaryRange)
        {
            movingDirection *= -1;

            if (customRenderer != null)
            {
                customRenderer.flipX = (movingDirection < 0);
            }
        }
    }

    public void Freeze()
    {
        clearToMove = false;
    }
}
