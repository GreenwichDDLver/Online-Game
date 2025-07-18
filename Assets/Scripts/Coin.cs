using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviourPunCallbacks
{
    private bool isCollected = false;

    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;

            // 通知所有人金币已被收集
            photonView.RPC("CollectCoin", RpcTarget.AllBuffered);
        }
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
