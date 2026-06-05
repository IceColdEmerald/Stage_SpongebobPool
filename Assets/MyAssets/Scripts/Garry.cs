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

        switch (outcome)
        {
            case 1:
                int minMoney = isLucky ? bowlMinMoney : normalMinMoney;
                int maxMoney = isLucky ? bowlMaxMoney : normalMaxMoney;

                int randomCashPayout = Random.Range(minMoney, maxMoney);
                GameManager.Instance.AddMoney(randomCashPayout);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayGerritCoins();

                break;

            case 2:
                GameManager.Instance.BuyExplosivePie();

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayGerritPie();

                break;

            case 3:
                GameManager.Instance.SetStrengthBuff(true);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayGerritStrength();

                break;
        }
    }
}