using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.Demo.PunBasics;

public class CoinManager : MonoBehaviourPunCallbacks
{
    public int totalCoinsRequired = 10;
    private int currentCoins = 0;

    public Level1UIManager uiManager;

    public void AddCoin()
    {
        currentCoins++;

        uiManager.UpdateCoinText(currentCoins, totalCoinsRequired);

        if (currentCoins >= totalCoinsRequired)
        {
            uiManager.ShowWinPanel();
        }
    }
}
