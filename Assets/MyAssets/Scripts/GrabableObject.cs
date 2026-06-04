using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] ItemData itemData;

    SpriteRenderer spriteRenderer;
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

    void OnValidate()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (itemData == null)
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
    }

    public int DeliverValue(float itemUpgradeBonus)
    {
        if (itemData == null) return 1;
        return itemData.GetCalculatedValue(itemUpgradeBonus);
    }
}
