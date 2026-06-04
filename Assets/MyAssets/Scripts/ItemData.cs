using UnityEngine;

[CreateAssetMenu(fileName = "New Grabable Item", menuName = "Spongebob/Item")]
public class ItemData : ScriptableObject
{
    [Header("Visuals")]
    [SerializeField] string itemName;
    [SerializeField] Sprite itemSprite;

    [Header("Audio")]
    [SerializeField] AudioClip grabSound;

    [Header("Mechanics")]
    [SerializeField] int baseValue;
    [SerializeField] float weightModifier = 1f;

    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public AudioClip GrabSound => grabSound;
    public float WeightModifier => weightModifier;

    public int GetCalculatedValue(float itemUpgradeBonus)
    {
        float finalValue = baseValue + itemUpgradeBonus;
        return Mathf.RoundToInt(finalValue);
    }
}