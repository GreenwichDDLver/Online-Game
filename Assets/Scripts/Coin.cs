using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviourPunCallbacks
{
    private bool isCollected = false;

    // 新增：被抓取时调用
    public void GrabCoin()
    {
        if (isCollected) return;
        isCollected = true;
        photonView.RPC("CollectCoin", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void CollectCoin()
    {
        if (gameObject != null)
        {
            FindObjectOfType<CoinManager>().AddCoin();
            Destroy(gameObject);
        }
    }
}
