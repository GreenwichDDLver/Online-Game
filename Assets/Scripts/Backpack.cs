using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Backpack : MonoBehaviour
{
    [Header("背包设置")]
    public int maxSlots = 8;
    public GameObject backpackUI;
    
    [Header("UI设置")]
    public TMP_Text[] slotTexts = new TMP_Text[8]; // 8个槽位的文本
    public TMP_Text currentItemText; // 当前物品名称显示
    
    [Header("物品系统")]
    public List<BackpackSlot> slots = new List<BackpackSlot>();
    
    private int currentSlotIndex = 0;
    private bool isBackpackOpen = false;
    private Grab grabSystem;
    private float originalTimeScale;
    
    // 公共属性
    public bool IsBackpackOpen => isBackpackOpen;
    public int CurrentSlotIndex => currentSlotIndex;
    public BackpackSlot CurrentSlot => slots.Count > 0 ? slots[currentSlotIndex] : null;
    
    [System.Serializable]
    public class BackpackSlot
    {
        public GameObject item;
        public string itemName;
        public int itemID; // 物品独立编号
        public bool isEmpty = true;
    }
    
    void Start()
    {
        grabSystem = FindObjectOfType<Grab>();
        InitializeBackpack();
        
        // 隐藏背包UI
        if (backpackUI != null)
        {
            backpackUI.SetActive(false);
        }
        
        // 保存原始时间缩放
        originalTimeScale = Time.timeScale;
    }
    
    void Update()
    {
        HandleBackpackInput();
        UpdateCurrentItemDisplay();
        
        // R键从背包取出当前物品（仅背包关闭时有效）
        if (!isBackpackOpen && Input.GetKeyDown(KeyCode.R))
        {
            TakeItemFromBackpack();
        }
    }
    
    void InitializeBackpack()
    {
        // 创建背包槽位
        for (int i = 0; i < maxSlots; i++)
        {
            BackpackSlot slot = new BackpackSlot();
            slots.Add(slot);
        }
        
        // 默认选中第一个槽位
        currentSlotIndex = 0;
        UpdateSlotDisplay();
    }
    
    void HandleBackpackInput()
    {
        // Tab键打开/关闭背包
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleBackpack();
        }
        
        // Q键切换物品槽位（只在背包关闭时有效）
        if (!isBackpackOpen && Input.GetKeyDown(KeyCode.Q))
        {
            SwitchToNextSlot();
        }
        
        // 数字键快速选择槽位（只在背包打开时有效）
        if (isBackpackOpen)
        {
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < slots.Count)
                {
                    SelectSlot(i);
                }
            }
        }
    }
    
    public void SwitchToNextSlot()
    {
        if (GetItemCount() == 0) return; // 没有物品不切换

        int startIndex = currentSlotIndex;
        do
        {
            currentSlotIndex = (currentSlotIndex + 1) % slots.Count;
            if (!slots[currentSlotIndex].isEmpty)
                break;
        } while (currentSlotIndex != startIndex);
        UpdateSlotDisplay();
    }
    
    public void SelectSlot(int index)
    {
        if (index >= 0 && index < slots.Count && !slots[index].isEmpty)
        {
            currentSlotIndex = index;
            UpdateSlotDisplay();
        }
    }
    
    public void UpdateSlotDisplay()
    {
        // 更新所有槽位的显示
        for (int i = 0; i < slots.Count && i < slotTexts.Length; i++)
        {
            if (slotTexts[i] != null)
            {
                if (slots[i].isEmpty)
                {
                    slotTexts[i].text = $"槽位 {i + 1}";
                    slotTexts[i].color = Color.gray;
                }
                else
                {
                    slotTexts[i].text = $"{slots[i].itemName} (ID:{slots[i].itemID})";
                    slotTexts[i].color = i == currentSlotIndex ? Color.yellow : Color.white;
                }
            }
        }
    }
    
    void UpdateCurrentItemDisplay()
    {
        // 更新当前物品显示（屏幕右侧）
        if (currentItemText != null)
        {
            if (currentSlotIndex >= 0 && currentSlotIndex < slots.Count)
            {
                BackpackSlot slot = slots[currentSlotIndex];
                if (!slot.isEmpty)
                {
                    currentItemText.text = $"{slot.itemName} (ID:{slot.itemID})";
                    currentItemText.color = Color.white;
                }
                else
                {
                    currentItemText.text = "空槽位";
                    currentItemText.color = Color.gray;
                }
            }
        }
    }
    
    public bool AddItemToBackpack(GameObject item, string itemName, int itemID)
    {
        // 优先用物品身上的ItemInfo
        var info = item.GetComponent<ItemInfo>();
        int trueID = info != null ? info.itemID : itemID;
        string trueName = info != null ? info.itemName : itemName;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isEmpty)
            {
                slots[i].item = item;
                slots[i].itemName = trueName;
                slots[i].itemID = trueID;
                slots[i].isEmpty = false;
                UpdateSlotDisplay();
                return true;
            }
        }
        return false;
    }
    
    public GameObject RemoveItemFromCurrentSlot()
    {
        if (currentSlotIndex >= 0 && currentSlotIndex < slots.Count)
        {
            BackpackSlot slot = slots[currentSlotIndex];
            if (!slot.isEmpty)
            {
                GameObject item = slot.item;
                
                // 清空槽位
                slot.item = null;
                slot.itemName = "";
                slot.itemID = 0;
                slot.isEmpty = true;
                
                // 更新UI
                UpdateSlotDisplay();
                
                return item;
            }
        }
        
        return null;
    }
    
    public bool IsCurrentSlotEmpty()
    {
        if (currentSlotIndex >= 0 && currentSlotIndex < slots.Count)
        {
            return slots[currentSlotIndex].isEmpty;
        }
        return true;
    }
    
    public void ToggleBackpack()
    {
        isBackpackOpen = !isBackpackOpen;
        
        if (backpackUI != null)
        {
            backpackUI.SetActive(isBackpackOpen);
        }
        
        // 时间暂停/恢复
        if (isBackpackOpen)
        {
            Time.timeScale = 0f; // 暂停时间
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = originalTimeScale; // 恢复时间
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    // 公共方法：获取当前槽位的物品信息
    public BackpackSlot GetCurrentSlot()
    {
        if (currentSlotIndex >= 0 && currentSlotIndex < slots.Count)
        {
            return slots[currentSlotIndex];
        }
        return null;
    }
    
    // 公共方法：根据ID查找物品
    public BackpackSlot FindItemByID(int itemID)
    {
        foreach (BackpackSlot slot in slots)
        {
            if (!slot.isEmpty && slot.itemID == itemID)
            {
                return slot;
            }
        }
        return null;
    }
    
    // 公共方法：检查背包是否已满
    public bool IsBackpackFull()
    {
        foreach (BackpackSlot slot in slots)
        {
            if (slot.isEmpty)
            {
                return false;
            }
        }
        return true;
    }
    
    // 公共方法：获取物品数量
    public int GetItemCount()
    {
        int count = 0;
        foreach (BackpackSlot slot in slots)
        {
            if (!slot.isEmpty)
            {
                count++;
            }
        }
        return count;
    }
    
    public void TakeItemFromBackpack()
    {
        if (GetItemCount() == 0) return;
        if (currentSlotIndex < 0 || currentSlotIndex >= slots.Count) return;
        BackpackSlot slot = slots[currentSlotIndex];
        if (slot.isEmpty || slot.item == null) return;

        // 激活物品
        slot.item.SetActive(true);
        // 放在玩家前方
        Transform playerTransform = transform;
        Vector3 dropPosition = playerTransform.position + playerTransform.forward * 2f + Vector3.up * 0.5f;
        slot.item.transform.position = dropPosition;
        slot.item.transform.SetParent(null);
        // 物理状态
        Rigidbody rb = slot.item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        Collider col = slot.item.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        // 清空槽位
        slot.item = null;
        slot.itemName = "";
        slot.itemID = 0;
        slot.isEmpty = true;
        UpdateSlotDisplay();
        UpdateCurrentItemDisplay();
    }
    
    void OnDestroy()
    {
        // 确保退出时恢复时间
        Time.timeScale = originalTimeScale;
    }
}
