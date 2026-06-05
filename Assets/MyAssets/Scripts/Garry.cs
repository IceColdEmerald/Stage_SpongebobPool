using UnityEngine;

public class Garry : MonoBehaviour
{
    [Header("Mystery Value Ranges")]
    [SerializeField] int normalMinMoney = 5;
    [SerializeField] int normalMaxMoney = 500;

    [Header("Gary Bowl Ranges")]
    [SerializeField] int bowlMinMoney = 300;
    [SerializeField] int bowlMaxMoney = 800;

    void Start()
    {
        if (TryGetComponent(out GrabableObject grabable))
        {
            int randomWeight = Random.Range(2, 11);
            grabable.SetCustomWeight(randomWeight);
        }
    }

    public void RevealMystery(Hook playerHook)
    {

        bool isLucky = GameManager.Instance.HasGarryBowl;
        int outcome = Random.Range(1, 4);


        if (isLucky)
        {
            switch (outcome)
            {
                case 1:
                    int randomCashPayout = Random.Range(normalMinMoney, normalMaxMoney);
                    GameManager.Instance.AddMoney(randomCashPayout);
                    break;
                case 2:
                    GameManager.Instance.BuyExplosivePie();
                    break;
                case 3:
                    GameManager.Instance.SetStrengthBuff(true);
                    break;
            }
        }
        else
        {
            switch (outcome)
            {
                case 1:
                    int randomCashPayout = Random.Range(bowlMinMoney, bowlMaxMoney);
                    GameManager.Instance.AddMoney(randomCashPayout);
                    break;
                case 2:
                    GameManager.Instance.BuyExplosivePie();
                    break;
                case 3:
                    GameManager.Instance.SetStrengthBuff(true);
                    break;
            }
        }
    }
}
