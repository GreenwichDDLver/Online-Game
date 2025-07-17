using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : MonoBehaviour
{
    [Header("测试设置")]
    public string itemName = "测试物品";
    public int itemID = 1;
    public bool isImportant = false;
    
    void Start()
    {
        // 自动添加ItemInfo组件
        ItemInfo itemInfo = GetComponent<ItemInfo>();
        if (itemInfo == null)
        {
            itemInfo = gameObject.AddComponent<ItemInfo>();
        }
        
        // 设置物品信息
        itemInfo.itemName = itemName;
        itemInfo.itemID = itemID;
        
        // 确保有Rigidbody组件
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // 确保有Collider组件
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // 根据物体类型添加合适的Collider
            if (GetComponent<MeshRenderer>() != null)
            {
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = true; // Rigidbody需要convex的MeshCollider
                }
                else
                {
                    gameObject.AddComponent<BoxCollider>();
                }
            }
            else
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }
        
        Debug.Log($"测试物品已设置：{itemName} (ID: {itemID})");
    }
    
    void Update()
    {
        // 按T键测试物品信息
        if (Input.GetKeyDown(KeyCode.T))
        {
            ItemInfo itemInfo = GetComponent<ItemInfo>();
            if (itemInfo != null)
            {
                Debug.Log($"物品信息：{itemInfo.itemName} (ID: {itemInfo.itemID})");
            }
        }
    }
} 