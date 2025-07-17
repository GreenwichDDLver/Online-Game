using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator; // 拖入角色的 Animator

    private void Update()
    {
        // 1. 获取玩家输入（WASD / 箭头键）
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. 计算移动速度（用于控制 idle/walk）
        float speed = new Vector3(h, 0, v).magnitude;
        animator.SetFloat("Speed", speed);

        // 3. 检测拾取输入（比如按下 E 键）
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Pickup");
        }
    }
}
