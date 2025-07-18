using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.Demo.PunBasics;

public class CoinManager : MonoBehaviourPunCallbacks
{
    public int totalCoinsRequired = 10;
    private int currentCoins = 0;

    public Level1UIManager uiManager;

    void Awake()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<Level1UIManager>();
        }
    }

    public void AddCoin()
    {
        currentCoins++;

        if (uiManager != null)
            uiManager.UpdateCoinText(currentCoins, totalCoinsRequired);

        if (currentCoins >= totalCoinsRequired && uiManager != null)
        {
            uiManager.ShowWinPanel();
        }
    }
}
