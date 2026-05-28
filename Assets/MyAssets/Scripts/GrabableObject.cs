using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] ItemData itemData;

    SpriteRenderer spriteRenderer;
    CircleCollider2D circleCollider;
    public float WeightModifier => itemData != null ? itemData.WeightModifier : 1f;
    public string ItemName => itemData != null ? itemData.ItemName : "Unknown";

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (itemData != null)
        {
            InitializeObject();
        }
    }

    void InitializeObject()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.ItemSprite;
        }

        transform.localScale = Vector3.one * itemData.VisualScale;
    }

    public int DeliverValue(float progressionMultiplier, float itemUpgradeBonus)
    {
        if (itemData == null) return 1;
        return itemData.GetCalculatedValue(progressionMultiplier, itemUpgradeBonus);
    }
}
