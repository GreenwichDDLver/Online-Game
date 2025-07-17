using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("移动设置")]
    public float walkSpeed = 3f;
    public float crouchSpeed = 1.5f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    
    [Header("下蹲设置")]
    public float crouchHeight = 0.5f;
    public float standHeight = 2f;
    public float crouchSmoothTime = 0.2f;
    
    [Header("平滑设置")]
    public float velocitySmoothTime = 0.05f;
    
    private Rigidbody rb;
    private Animator animator;
    private CapsuleCollider playerCollider;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;
    private Vector3 velocitySmoothVelocity;
    private bool isCrouching = false;
    private bool isGrounded;
    private float currentHeight;
    private float targetHeight;
    private float heightSmoothVelocity;
    
    // 公共属性，供其他脚本访问
    public bool IsCrouching => isCrouching;
    public bool IsGrounded => isGrounded;
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取组件
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
        
        // 如果没有Rigidbody，添加一个
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // 如果没有CapsuleCollider，添加一个
        if (playerCollider == null)
        {
            playerCollider = gameObject.AddComponent<CapsuleCollider>();
        }
        
        // 配置Rigidbody（减少惯性）
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.drag = 1f; // 增加阻力减少惯性
        rb.angularDrag = 5f;
        
        // 初始化高度
        currentHeight = standHeight;
        targetHeight = standHeight;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        HandleInput();
        HandleCrouch();
    }
    
    void FixedUpdate()
    {
        ApplyMovement();
    }
    
    void CheckGrounded()
    {
        // 检测是否在地面上
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        float checkDistance = 0.2f;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, checkDistance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    void HandleInput()
    {
        // 获取输入
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // 计算移动方向（相对于玩家朝向）
        Vector3 forward = transform.forward * verticalInput;
        Vector3 right = transform.right * horizontalInput;
        Vector3 inputDirection = (forward + right).normalized;
        
        // 计算目标速度
        if (inputDirection.magnitude > 0.1f)
        {
            float currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
            targetVelocity = inputDirection * currentSpeed;
        }
        else
        {
            // 没有输入时快速减速（解谜游戏需要精确控制）
            targetVelocity = Vector3.zero;
        }
        
        // 平滑速度变化
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref velocitySmoothVelocity, velocitySmoothTime);
        
        // 更新动画
        if (animator != null)
        {
            float speed = currentVelocity.magnitude / walkSpeed;
            animator.SetBool("IsMoving", speed > 0.1f);
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsCrouching", isCrouching);
        }
    }
    
    void HandleCrouch()
    {
        // 按C键下蹲
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouch();
        }
        
        // 平滑下蹲高度变化
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightSmoothVelocity, crouchSmoothTime);
        
        // 更新碰撞器高度
        if (playerCollider != null)
        {
            playerCollider.height = currentHeight;
            playerCollider.center = new Vector3(0, currentHeight * 0.5f, 0);
        }
    }
    
    void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        targetHeight = isCrouching ? crouchHeight : standHeight;
        
        // 通知摄像头下蹲状态改变
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetCrouching(isCrouching);
        }
    }
    
    void ApplyMovement()
    {
        // 计算最终速度
        Vector3 finalVelocity = currentVelocity;
        
        // 保持Y轴速度（重力影响）
        finalVelocity.y = rb.velocity.y;
        
        // 应用移动
        rb.velocity = finalVelocity;
    }
    
    // 在编辑器中绘制地面检测线
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawRay(rayStart, Vector3.down * 0.2f);
    }
}
