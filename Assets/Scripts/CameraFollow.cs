using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("第一人称设置")]
    public Transform playerBody;
    public Transform playerHead;
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;
    
    [Header("下蹲视角设置")]
    public float standHeight = 1.8f;
    public float crouchHeight = 0.9f;
    public float heightSmoothTime = 0.2f;
    
    [Header("平滑设置")]
    public float rotationSmoothTime = 0.05f;
    public float positionSmoothTime = 0.02f;
    
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private float targetRotationX = 0f;
    private float targetRotationY = 0f;
    private Vector3 rotationSmoothVelocity;
    private Vector3 positionSmoothVelocity;
    private bool isMouseControl = false;
    private Vector3 targetPosition;
    private bool isCrouching = false;
    private float currentHeight;
    private float targetHeight;
    private float heightSmoothVelocity;
    
    void Start()
    {
        // 如果没有指定玩家身体，尝试找到带有Move脚本的对象
        if (playerBody == null)
        {
            Move playerMove = FindObjectOfType<Move>();
            if (playerMove != null)
            {
                playerBody = playerMove.transform;
            }
    }

        // 如果没有指定头部，使用玩家身体作为头部
        if (playerHead == null)
        {
            playerHead = playerBody;
        }
        
        // 初始化旋转角度（添加安全检查）
        InitializeRotation();
        
        // 初始化高度
        currentHeight = standHeight;
        targetHeight = standHeight;
        
        // 初始化目标位置
        if (playerHead != null)
        {
            targetPosition = playerHead.position + Vector3.up * currentHeight;
        }
        
        // 锁定鼠标到屏幕中心
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void InitializeRotation()
    {
        // 安全地初始化旋转角度
        if (playerHead != null)
        {
            Vector3 direction = transform.position - playerHead.position;
            float distance = direction.magnitude;
            
            if (distance > 0.001f) // 避免除零
            {
                // 计算水平角度
                currentRotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                
                // 计算垂直角度（限制在有效范围内）
                float verticalAngle = Mathf.Asin(Mathf.Clamp(direction.y / distance, -1f, 1f)) * Mathf.Rad2Deg;
                currentRotationX = -verticalAngle;
            }
            else
            {
                // 如果距离太近，使用默认角度
                currentRotationX = 0f;
                currentRotationY = 0f;
            }
        }
        else
        {
            // 如果没有目标，使用当前旋转
            Vector3 eulerAngles = transform.localEulerAngles;
            currentRotationX = NormalizeAngle(eulerAngles.x);
            currentRotationY = eulerAngles.y;
        }
        
        // 设置目标旋转
        targetRotationX = currentRotationX;
        targetRotationY = currentRotationY;
    }
    
    // 标准化角度到-180到180度范围
    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    void Update()
    {
        HandleInput();
        HandleMouseRotation();
        HandleCrouchHeight();
    }
    
    void LateUpdate()
    {
        UpdateCameraPosition();
    }
    
    void HandleInput()
    {
        // 按ESC键切换鼠标控制
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMouseControl = !isMouseControl;
            Cursor.lockState = isMouseControl ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isMouseControl;
        }
    }
    
    void HandleMouseRotation()
    {
        if (isMouseControl) return;
        
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // 更新目标旋转角度
        targetRotationY += mouseX;
        targetRotationX -= mouseY;
        targetRotationX = Mathf.Clamp(targetRotationX, minVerticalAngle, maxVerticalAngle);
        
        // 检查角度是否有效
        if (float.IsNaN(targetRotationX) || float.IsNaN(targetRotationY))
        {
            // 如果角度无效，重置为安全值
            targetRotationX = 0f;
            targetRotationY = 0f;
        }
        
        // 平滑旋转
        currentRotationX = Mathf.SmoothDamp(currentRotationX, targetRotationX, ref rotationSmoothVelocity.x, rotationSmoothTime);
        currentRotationY = Mathf.SmoothDamp(currentRotationY, targetRotationY, ref rotationSmoothVelocity.y, rotationSmoothTime);
        
        // 再次检查平滑后的角度
        if (float.IsNaN(currentRotationX) || float.IsNaN(currentRotationY))
        {
            currentRotationX = 0f;
            currentRotationY = 0f;
        }
        
        // 应用旋转到相机
        Quaternion newRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);
        
        // 检查四元数是否有效
        if (IsValidQuaternion(newRotation))
        {
            transform.localRotation = newRotation;
        }
        else
        {
            // 如果四元数无效，使用安全的默认旋转
            transform.localRotation = Quaternion.identity;
        }
        
        // 应用水平旋转到玩家身体（平滑）
        if (playerBody != null)
        {
            Quaternion targetBodyRotation = Quaternion.Euler(0f, targetRotationY, 0f);
            if (IsValidQuaternion(targetBodyRotation))
            {
                playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetBodyRotation, rotationSmoothTime * 2f);
            }
        }
    }
    
    // 检查四元数是否有效
    bool IsValidQuaternion(Quaternion q)
    {
        return !float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z) && !float.IsNaN(q.w) &&
               !float.IsInfinity(q.x) && !float.IsInfinity(q.y) && !float.IsInfinity(q.z) && !float.IsInfinity(q.w);
    }
    
    void HandleCrouchHeight()
    {
        // 平滑高度变化
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightSmoothVelocity, heightSmoothTime);
        
        // 检查高度是否有效
        if (float.IsNaN(currentHeight))
        {
            currentHeight = standHeight;
        }
    }
    
    void UpdateCameraPosition()
    {
        // 更新目标位置
        if (playerHead != null)
        {
            targetPosition = playerHead.position + Vector3.up * currentHeight;
        }
        
        // 检查位置是否有效
        if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y) || float.IsNaN(targetPosition.z))
        {
            targetPosition = transform.position;
        }
        
        // 平滑移动相机位置
        Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref positionSmoothVelocity, positionSmoothTime);
        
        // 检查新位置是否有效
        if (!float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y) && !float.IsNaN(newPosition.z))
        {
            transform.position = newPosition;
        }
    }
    
    // 公共方法：设置下蹲状态
    public void SetCrouching(bool crouching)
    {
        isCrouching = crouching;
        targetHeight = crouching ? crouchHeight : standHeight;
    }
    
    // 公共方法：设置鼠标灵敏度
    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }
    
    // 公共方法：重置视角
    public void ResetView()
    {
        targetRotationX = 0f;
        targetRotationY = playerBody != null ? playerBody.eulerAngles.y : 0f;
        
        // 重置当前旋转
        currentRotationX = 0f;
        currentRotationY = targetRotationY;
    }
}
