using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator; // �����ɫ�� Animator

    private void Update()
    {
        // 1. ��ȡ������루WASD / ��ͷ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. �����ƶ��ٶȣ����ڿ��� idle/walk��
        float speed = new Vector3(h, 0, v).magnitude;
        animator.SetFloat("Speed", speed);

        // 3. ���ʰȡ���루���簴�� E ����
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Pickup");
        }
    }
}
