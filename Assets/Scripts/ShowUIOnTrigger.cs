using UnityEngine;

public class ShowUIOnTrigger : MonoBehaviour
{
    [Header("要显示的UI面板")]
    public GameObject uiPanel;

    // 记录原始鼠标状态
    private CursorLockMode prevLockState;
    private bool prevCursorVisible;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiPanel.SetActive(true);
            // 记录原始状态
            prevLockState = Cursor.lockState;
            prevCursorVisible = Cursor.visible;
            // 解锁并显示鼠标
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideUIAndLockCursor();
        }
    }

    // 供UI按钮调用的方法
    public void HideUIAndLockCursor()
    {
        uiPanel.SetActive(false);
        // 恢复原始鼠标状态
        Cursor.lockState = prevLockState;
        Cursor.visible = prevCursorVisible;
    }
}
