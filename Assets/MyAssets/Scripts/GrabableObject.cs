using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] ItemData itemData;

    public ItemData ItemData => itemData;

    SpriteRenderer spriteRenderer;
    float customWeight = -1f;

    public float WeightModifier => customWeight > 0 ? customWeight : (itemData != null ? itemData.WeightModifier : 1f);
    public string ItemName => itemData != null ? itemData.ItemName : "Unknown";
    public AudioClip GrabSound => itemData != null ? itemData.GrabSound : null;
    public float GrabSoundVolume => itemData != null ? itemData.GrabSoundVolume : 1f;

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
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (itemData != null) InitializeObject();
    }

    void InitializeObject()
    {
        if (spriteRenderer != null) spriteRenderer.sprite = itemData.ItemSprite;
    }

    public void SetCustomWeight(float newWeight)
    {
        customWeight = newWeight;
    }

    public int DeliverValue(float itemUpgradeBonus)
    {
        if (itemData == null) return 1;
        return itemData.GetCalculatedValue(itemUpgradeBonus);
    }
}