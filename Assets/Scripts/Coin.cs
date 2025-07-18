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

            // ֪ͨ�����˽���ѱ��ռ�
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
