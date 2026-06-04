using UnityEngine;

public class Garry : MonoBehaviour
{
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
        int rolledReward = Random.Range(0, 3);

        switch (rolledReward)
        {
            case 0:
                int randomCashPayout = Random.Range(8, 801);
                GameManager.Instance.AddMoney(randomCashPayout);
                break;
            case 1:
                playerHook.AddExplosive(1);
                break;
            case 2:
                playerHook.ActivateStrengthBuff();
                break;
        }
    }
}
