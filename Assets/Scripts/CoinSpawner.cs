using UnityEngine;
using Photon.Pun;

public class CoinSpawner : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom, isMasterClient: " + PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnCoins();
        }
    }

    void SpawnCoins()
    {
        // 第一个位置
        Vector3 pos1 = new Vector3(-31.54f, -5.49f, 237.97f);
        Quaternion rot1 = Quaternion.Euler(-8.935f, 0f, -18.186f);
        PhotonNetwork.Instantiate("Coin", pos1, rot1);

        // 第二个位置
        Vector3 pos2 = new Vector3(-29.267f, -4.933f, 233.741f);
        Quaternion rot2 = Quaternion.Euler(-92.386f, 0f, -18.186f);
        PhotonNetwork.Instantiate("Coin", pos2, rot2);
    }
} 