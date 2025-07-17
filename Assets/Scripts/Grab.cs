using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [Header("抓取设置")]
    public float grabRange = 2.5f;
    public float grabForce = 10f;
    public float dropForce = 3f; // 丢出力度
    public LayerMask grabableLayer = -1;
    public Transform holdPoint;
    public Transform dropPosition;
    public Camera playerCamera;
    
    [Header("UI设置")]
    public GameObject grabPrompt;
    public GameObject itemInfoPanel;
    public GameObject crosshair; // 准心UI
    
    [Header("高亮设置")]
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;
    public Material highlightMaterial; // 高亮材质
    
    [Header("物品信息")]
    public string currentItemName = "";
    public int currentItemID = 0;
    
    private Rigidbody heldObject;
    private Collider heldObjectCollider;
    private bool isHolding = false;
    private Move playerMove;
    private Backpack backpack;
    private GameObject currentHighlightedObject;
    private Material originalMaterial;
    private Renderer originalRenderer;
    
    // 公共属性
    public bool IsHolding => isHolding;
    public Rigidbody HeldObject => heldObject;
    
    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        playerMove = FindObjectOfType<Move>();
        backpack = FindObjectOfType<Backpack>();
        
        // 如果没有指定holdPoint，自动创建并设置为FPS风格位置
        if (holdPoint == null)
        {
            GameObject holdObj = new GameObject("HoldPoint");
            holdObj.transform.SetParent(Camera.main.transform);
            holdObj.transform.localPosition = new Vector3(0.5f, -0.2f, 1.0f); // 屏幕右下角
            holdObj.transform.localRotation = Quaternion.identity;
            holdPoint = holdObj.transform;
        }
        
        // 如果没有指定放下位置，创建一个
        if (dropPosition == null)
        {
            GameObject dropPos = new GameObject("DropPosition");
            dropPosition = dropPos.transform;
            dropPosition.SetParent(transform);
            dropPosition.localPosition = new Vector3(0, 0, 1f);
        }
        
        // 隐藏UI
        if (grabPrompt != null) grabPrompt.SetActive(false);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(false);
        if (crosshair != null) crosshair.SetActive(false);
        
        // 如果没有指定高亮材质，创建一个
        if (highlightMaterial == null)
        {
            CreateHighlightMaterial();
        }
    }
    
    void CreateHighlightMaterial()
    {
        // 创建一个简单的高亮材质
        highlightMaterial = new Material(Shader.Find("Standard"));
        highlightMaterial.color = highlightColor;
        highlightMaterial.SetFloat("_Metallic", 0.5f);
        highlightMaterial.SetFloat("_Glossiness", 0.8f);
        highlightMaterial.EnableKeyword("_EMISSION");
        highlightMaterial.SetColor("_EmissionColor", highlightColor * highlightIntensity);
    }
    
    void Update()
    {
        HandleGrabInput();
        UpdateHeldObject();
        CheckGrabableObjects();
    }
    
    void HandleGrabInput()
    {
        // 左键抓取/丢出物品
        if (Input.GetMouseButtonDown(0))
        {
            if (isHolding)
            {
                DropItemWithForce(); // 丢出物品
            }
            else
            {
                TryGrabItem();
            }
        }
        
        // 右键存入背包
        if (Input.GetMouseButtonDown(1) && isHolding)
        {
            StoreItemInBackpack();
        }
        
        // 按F键查看物品信息
        if (Input.GetKeyDown(KeyCode.F) && isHolding)
        {
            ShowItemInfo();
        }
    }
    
    void CheckGrabableObjects()
    {
        if (isHolding) return;
        
        RaycastHit hit;
        Vector3 rayStart = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;
        
        if (Physics.Raycast(rayStart, rayDirection, out hit, grabRange, grabableLayer))
        {
            // 检查是否是可抓取的物品
            if (hit.collider.GetComponent<Rigidbody>() != null)
            {
                ShowGrabPrompt(hit.collider.gameObject);
                HighlightObject(hit.collider.gameObject);
            }
            else
            {
                HideGrabPrompt();
                RemoveHighlight();
            }
        }
        else
        {
            HideGrabPrompt();
            RemoveHighlight();
        }
    }
    
    void HighlightObject(GameObject obj)
    {
        // 如果高亮的对象没有改变，直接返回
        if (currentHighlightedObject == obj) return;
        
        // 移除之前的高亮
        RemoveHighlight();
        
        // 设置新的高亮对象
        currentHighlightedObject = obj;
        Renderer renderer = obj.GetComponent<Renderer>();
        
        if (renderer != null)
        {
            originalRenderer = renderer;
            originalMaterial = renderer.material;
            
            // 应用高亮材质
            renderer.material = highlightMaterial;
        }
    }
    
    void RemoveHighlight()
    {
        if (currentHighlightedObject != null && originalRenderer != null)
        {
            // 恢复原始材质
            originalRenderer.material = originalMaterial;
            currentHighlightedObject = null;
            originalRenderer = null;
            originalMaterial = null;
        }
    }
    
    public void TryGrabItem()
    {
        RaycastHit hit;
        Vector3 rayStart = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;
        
        if (Physics.Raycast(rayStart, rayDirection, out hit, grabRange, grabableLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                GrabItem(rb);
            }
        }
    }
    
    void GrabItem(Rigidbody itemRb)
    {
        heldObject = itemRb;
        heldObjectCollider = itemRb.GetComponent<Collider>();
        
        // 移除高亮
        RemoveHighlight();
        
        // 禁用物理和碰撞
        heldObject.isKinematic = true;
        if (heldObjectCollider != null)
        {
            heldObjectCollider.enabled = false;
        }
        
        // 设置父对象
        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
        
        isHolding = true;
        HideGrabPrompt();
        
        // 获取物品信息
        GetItemInfo(heldObject.gameObject);
        
        // 播放抓取音效或动画
        PlayGrabEffect();
    }
    
    public void DropItemWithForce()
    {
        if (heldObject == null) return;
        
        // 恢复物理和碰撞
        heldObject.isKinematic = false;
        if (heldObjectCollider != null)
        {
            heldObjectCollider.enabled = true;
        }
        
        // 设置放下位置
        heldObject.transform.SetParent(null);
        heldObject.transform.position = dropPosition.position;
        
        // 应用丢出力（较小力度）
        Vector3 dropDirection = playerCamera.transform.forward + playerCamera.transform.up * 0.1f;
        heldObject.velocity = dropDirection * dropForce;
        
        isHolding = false;
        heldObject = null;
        heldObjectCollider = null;
        
        // 清除物品信息
        ClearItemInfo();
        
        // 播放丢出音效或动画
        PlayDropEffect();
    }
    
    void StoreItemInBackpack()
    {
        if (heldObject == null || backpack == null) 
        {
            Debug.Log("无法存入背包：物品为空或背包为空");
            return;
        }
        
        // 获取物品信息
        ItemInfo itemInfo = heldObject.GetComponent<ItemInfo>();
        string itemName = itemInfo != null ? itemInfo.itemName : heldObject.name;
        int itemID = itemInfo != null ? itemInfo.itemID : 0;
        
        Debug.Log($"尝试存入背包：{itemName} (ID: {itemID})");
        
        // 尝试存入背包
        if (backpack.AddItemToBackpack(heldObject.gameObject, itemName, itemID))
        {
            Debug.Log($"成功存入背包：{itemName}");
            
            // 成功存入背包，隐藏物品（不要销毁）
            heldObject.gameObject.SetActive(false);
            
            // 重置物品的物理状态
            heldObject.isKinematic = false;
            if (heldObjectCollider != null)
            {
                heldObjectCollider.enabled = true;
            }
            
            // 重置物品的父对象
            heldObject.transform.SetParent(null);
            
            isHolding = false;
            heldObject = null;
            heldObjectCollider = null;
            
            // 清除物品信息
            ClearItemInfo();
            
            // 播放存入背包音效或动画
            PlayStoreEffect();
        }
        else
        {
            Debug.Log("背包已满，丢出物品");
            // 背包已满，丢出物品
            DropItemWithForce();
        }
    }
    
    void UpdateHeldObject()
    {
        if (!isHolding || heldObject == null) return;
        
        // 平滑移动物品到抓取位置
        heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, holdPoint.position, Time.deltaTime * 10f);
        heldObject.transform.rotation = Quaternion.Slerp(heldObject.transform.rotation, holdPoint.rotation, Time.deltaTime * 10f);
    }
    
    void ShowGrabPrompt(GameObject item)
    {
        if (grabPrompt != null)
        {
            grabPrompt.SetActive(true);
            // 可以在这里更新提示文本
        }
        
        // 显示准心
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }
    
    void HideGrabPrompt()
    {
        if (grabPrompt != null)
        {
            grabPrompt.SetActive(false);
        }
        
        // 隐藏准心
        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }
    }
    
    void ShowItemInfo()
    {
        if (itemInfoPanel != null)
        {
            itemInfoPanel.SetActive(true);
            // 可以在这里更新物品信息文本
        }
    }
    
    void HideItemInfo()
    {
        if (itemInfoPanel != null)
        {
            itemInfoPanel.SetActive(false);
        }
    }
    
    void GetItemInfo(GameObject item)
    {
        // 获取物品信息（可以从物品组件中读取）
        ItemInfo itemInfo = item.GetComponent<ItemInfo>();
        if (itemInfo != null)
        {
            currentItemName = itemInfo.itemName;
            currentItemID = itemInfo.itemID;
        }
        else
        {
            currentItemName = item.name;
            currentItemID = 0;
        }
    }
    
    void ClearItemInfo()
    {
        currentItemName = "";
        currentItemID = 0;
        HideItemInfo();
    }
    
    void PlayGrabEffect()
    {
        // 播放抓取音效或粒子效果
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            // audioSource.PlayOneShot(grabSound);
        }
    }
    
    void PlayDropEffect()
    {
        // 播放丢出音效或粒子效果
    }
    
    void PlayStoreEffect()
    {
        // 播放存入背包音效或粒子效果
    }
    
    // 在编辑器中绘制抓取范围
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 rayStart = playerCamera.transform.position;
            Vector3 rayEnd = rayStart + playerCamera.transform.forward * grabRange;
            Gizmos.DrawRay(rayStart, playerCamera.transform.forward * grabRange);
            
            if (holdPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(holdPoint.position, 0.1f);
            }
        }
    }
    
    void OnDestroy()
    {
        // 清理高亮材质
        if (highlightMaterial != null)
        {
            DestroyImmediate(highlightMaterial);
        }
    }
}

// 物品信息组件已移至独立的ItemInfo.cs文件
