using UnityEngine;

public class ExplosivesUI : MonoBehaviour
{
    [Header("Pie Image Object in Order (1 to 3)")]
    [SerializeField] GameObject[] pieIcons;

    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        int currentPies = GameManager.Instance.ExplosivesCount;

        currentPies = Mathf.Clamp(currentPies, 0, pieIcons.Length);

        for (int i = 0; i < pieIcons.Length; i++)
        {
            pieIcons[i].SetActive(i < currentPies);
        }
    }
}
