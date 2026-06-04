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

    [Header("Inventory")]
    [SerializeField] int explosivesCount = 3;

    Vector2 originalPosition;
    GameObject grabbedObject = null;

    void Start()
    {
        originalPosition = transform.position;
        currentRetractSpeed = baseRetractSpeed;
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
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentState == HookState.Swinging)
        {
            currentState = HookState.Extending;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && currentState == HookState.Grabbing && grabbedObject != null && explosivesCount > 0)
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
            weightModifier = grabable.WeightModifier;
        }
        StartRetracting(weightModifier);
    }

    void HandleRetracting()
    {
        transform.position = Vector2.MoveTowards(transform.position, originalPosition, currentRetractSpeed * Time.deltaTime);

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
        explosivesCount--;

        if (grabbedObject != null)
        {
            Destroy(grabbedObject);
            grabbedObject = null;
        }

        StartRetracting(1f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState != HookState.Extending) return;
    
        if (((1 << collision.gameObject.layer) & borderLayer) != 0)
        {
            StartRetracting(1f);
        }
        else if (((1 << collision.gameObject.layer) & grabbableLayer) != 0)
        {
            grabbedObject = collision.gameObject;
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
        if (grabbedObject != null)
        {
            if (grabbedObject.TryGetComponent(out GrabableObject liveItem))
            {
                float rockBookBonus = GameManager.Instance.RockCollectorBonus;

                int finalPayout = liveItem.DeliverValue(rockBookBonus);
                GameManager.Instance.AddMoney(finalPayout);
            }
            else
            {
                GameManager.Instance.AddMoney(1);
            }

            Destroy(grabbedObject);
        }
        
        grabbedObject = null;
        currentRetractSpeed = baseRetractSpeed;
        currentState = HookState.Swinging;
    }
}
