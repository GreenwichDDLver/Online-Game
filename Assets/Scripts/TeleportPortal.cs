using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    [Header("传送目标位置")]
    public Transform destination;

    [Header("传送冷却时间（防止立刻重复传送）")]
    public float cooldown = 1f;

    private bool isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        // 判断是否为玩家
        if (other.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(Teleport(other.transform));
        }
    }

    private System.Collections.IEnumerator Teleport(Transform player)
    {
        isTeleporting = true;

        // 传送前可以加特效或延时
        yield return new WaitForSeconds(0.1f);

        // 设置玩家的新位置（可加偏移）
        player.position = destination.position;

        // 冷却，防止立刻触发目标传送门
        yield return new WaitForSeconds(cooldown);

        isTeleporting = false;
    }
}
