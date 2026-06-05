using UnityEngine;

public class Hook : MonoBehaviour
{
    enum HookState { Swinging, Extending, Grabbing, Retracting }
    HookState currentState = HookState.Swinging;

    [Header("Swinging Settings")]
    [SerializeField] float swingSpeed = 2f;
    [SerializeField] float swingAngleLimit = 70f;

    [Header("Movement Settings")]
    [SerializeField] float shootSpeed = 10f;
    [SerializeField] float baseRetractSpeed = 8f;
    float currentRetractSpeed;

    [Header("Bounds & Layer Setup")]
    [SerializeField] float maxLineLength = 15f;
    [SerializeField] LayerMask borderLayer;
    [SerializeField] LayerMask grabbableLayer;

    [Header("Visual (Rope)")]
    [SerializeField] LineRenderer lineRenderer;

    Vector2 originalPosition;
    GameObject grabbedObject = null;
    bool isInitialized = false;

    void Awake()
    {
        originalPosition = transform.position;
        currentRetractSpeed = baseRetractSpeed;
    }

    void OnEnable()
    {
        if (!isInitialized)
        {
            originalPosition = transform.position;
            isInitialized = true;
        }

        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;

        currentState = HookState.Swinging;
        currentRetractSpeed = baseRetractSpeed;
        grabbedObject = null;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopHookShoot();

        UpdateRopeVisual();
    }

    void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopHookShoot();
    }

    void Start()
    {
        currentRetractSpeed = baseRetractSpeed;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
            lineRenderer.positionCount = 2;
    }

    void Update()
    {
        HandleInput();

        switch (currentState)
        {
            case HookState.Swinging:
                HandleSwinging();
                break;
            case HookState.Extending:
                HandleExtending();
                break;
            case HookState.Grabbing:
                HandleGrabbing();
                break;
            case HookState.Retracting:
                HandleRetracting();
                break;
        }

        UpdateRopeVisual();
    }

    void UpdateRopeVisual()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, originalPosition);
            lineRenderer.SetPosition(1, transform.position);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentState == HookState.Swinging)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayHookShoot();

            currentState = HookState.Extending;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) &&
            currentState == HookState.Retracting &&
            grabbedObject != null &&
            GameManager.Instance.ExplosivesCount > 0)
        {
            UseExplosive();
        }
    }

    void HandleSwinging()
    {
        float swingAngle = Mathf.Sin(Time.time * swingSpeed) * swingAngleLimit;
        transform.rotation = Quaternion.Euler(0, 0, swingAngle);
    }

    void HandleExtending()
    {
        transform.Translate(shootSpeed * Time.deltaTime * Vector2.down);

        if (Vector2.Distance(originalPosition, transform.position) >= maxLineLength)
        {
            StartRetracting(1f);
        }
    }

    void HandleGrabbing()
    {
        if (grabbedObject == null)
        {
            StartRetracting(1f);
            return;
        }

        if (grabbedObject.TryGetComponent(out Collider2D grabbedCollider))
        {
            grabbedCollider.enabled = false;
        }

        float weightModifier = 1f;

        if (grabbedObject.TryGetComponent(out GrabableObject grabable))
        {
            weightModifier = GameManager.Instance.HasIceCream ? 1f : grabable.WeightModifier;
        }

        StartRetracting(weightModifier);
    }

    void HandleRetracting()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            originalPosition,
            currentRetractSpeed * Time.deltaTime
        );

        if (grabbedObject != null)
        {
            grabbedObject.transform.position = transform.position;
        }

        if (Vector2.Distance(transform.position, originalPosition) < 0.05f)
        {
            transform.position = originalPosition;
            ResetHookState();
        }
    }

    void UseExplosive()
    {
        if (grabbedObject == null || GameManager.Instance.ExplosivesCount <= 0) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayExplosion();

        GameManager.Instance.UseExplosivePie();

        FindFirstObjectByType<ExplosivesUI>()?.UpdateDisplay();

        Destroy(grabbedObject);
        grabbedObject = null;
        StartRetracting(1f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState != HookState.Extending) return;

        if (collision.TryGetComponent(out Puff puff))
        {
            puff.Explode();
            StartRetracting(1f);

            if (GameManager.Instance != null)
                GameManager.Instance.AddMoney(1);

            return;
        }

        if (((1 << collision.gameObject.layer) & borderLayer) != 0)
        {
            StartRetracting(1f);
        }
        else if (((1 << collision.gameObject.layer) & grabbableLayer) != 0)
        {
            grabbedObject = collision.gameObject;

            if (grabbedObject.TryGetComponent(out GrabableObject grabable))
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayCustomSFX(
                        grabable.GrabSound,
                        grabable.GrabSoundVolume
                    );
            }

            if (grabbedObject.TryGetComponent(out PlanktonMovement plankton))
            {
                plankton.Freeze();
            }

            currentState = HookState.Grabbing;
        }
    }

    void StartRetracting(float weightMultiplier)
    {
        float safeWeight = Mathf.Max(0.1f, weightMultiplier);
        currentRetractSpeed = baseRetractSpeed / safeWeight;
        currentState = HookState.Retracting;
    }

    void ResetHookState()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopHookShoot();

        if (grabbedObject != null)
        {
            if (grabbedObject.TryGetComponent(out Garry garry))
            {
                garry.RevealMystery(this);
            }
            else if (grabbedObject.TryGetComponent(out GrabableObject liveItem))
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayItemDeliver();

                string itemName = liveItem.gameObject.name;

                if (liveItem.ItemData != null)
                    itemName = liveItem.ItemData.ItemName;

                GameManager.Instance.ProcessItemDelivery(
                    itemName,
                    liveItem.DeliverValue(0f)
                );
            }

            Destroy(grabbedObject);
        }

        grabbedObject = null;
        currentRetractSpeed = baseRetractSpeed;
        currentState = HookState.Swinging;
    }
}