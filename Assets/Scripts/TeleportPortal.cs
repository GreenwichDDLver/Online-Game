using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    [Header("����Ŀ��λ��")]
    public Transform destination;

    [Header("������ȴʱ�䣨��ֹ�����ظ����ͣ�")]
    public float cooldown = 1f;

    private bool isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        // �ж��Ƿ�Ϊ���
        if (other.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(Teleport(other.transform));
        }
    }

    private System.Collections.IEnumerator Teleport(Transform player)
    {
        isTeleporting = true;

        // ����ǰ���Լ���Ч����ʱ
        yield return new WaitForSeconds(0.1f);

        // ������ҵ���λ�ã��ɼ�ƫ�ƣ�
        player.position = destination.position;

        // ��ȴ����ֹ���̴���Ŀ�괫����
        yield return new WaitForSeconds(cooldown);

        isTeleporting = false;
    }
}
