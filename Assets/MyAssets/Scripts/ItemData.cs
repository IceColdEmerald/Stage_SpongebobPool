using UnityEngine;

[CreateAssetMenu(fileName = "New Grabable Item", menuName = "Spongebob/Item")]
public class ItemData : ScriptableObject
{
    [Header("Visuals")]
    [SerializeField] string itemName;
    [SerializeField] Sprite itemSprite;
    [SerializeField] float visualScale = 1f;

    [Header("Mechanics")]
    [SerializeField] int baseValue;
    [SerializeField] float weightModifier = 1f;

    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public float VisualScale => visualScale;
    public float WeightModifier => weightModifier;

    public int GetCalculatedValue(float progressionMultiplier, float itemUpgradeBonus)
    {
        float finalValue = (baseValue * progressionMultiplier) + itemUpgradeBonus;
        return Mathf.RoundToInt(finalValue);
    }
}
