using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public int itemID;
        public string itemName;
        public string description;
        public bool isImportant;
        public GameObject itemPrefab;
    }
    
    [Header("物品数据库")]
    public List<ItemData> itemDatabase = new List<ItemData>();
    
    [Header("自动编号设置")]
    public bool autoAssignIDs = true;
    public int nextItemID = 1;
    
    private static ItemSystem instance;
    public static ItemSystem Instance => instance;
    
    public bool autoSpawnAll = true; // 新增：是否自动生成所有物品
    public int spawnCountPerItem = 1; // 每种物品生成几个
    public Vector3 spawnAreaMin = new Vector3(-5, 1, -5);
    public Vector3 spawnAreaMax = new Vector3(5, 1, 5);
    
    void Awake()
    {
        // 单例模式
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (autoAssignIDs)
        {
            AssignItemIDs();
        }
        if (autoSpawnAll)
        {
            SpawnAllItems();
        }
    }
    
    // 自动分配物品ID
    void AssignItemIDs()
    {
        for (int i = 0; i < itemDatabase.Count; i++)
        {
            if (itemDatabase[i].itemID == 0)
            {
                itemDatabase[i].itemID = nextItemID++;
            }
        }
    }
    
    // 根据ID获取物品数据
    public ItemData GetItemByID(int itemID)
    {
        foreach (ItemData item in itemDatabase)
        {
            if (item.itemID == itemID)
            {
                return item;
            }
        }
        return null;
    }
    
    // 根据名称获取物品数据
    public ItemData GetItemByName(string itemName)
    {
        foreach (ItemData item in itemDatabase)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        return null;
    }
    
    // 添加新物品到数据库
    public void AddItemToDatabase(string itemName, string description, bool isImportant = false, GameObject itemPrefab = null)
    {
        ItemData newItem = new ItemData
        {
            itemID = nextItemID++,
            itemName = itemName,
            description = description,
            isImportant = isImportant,
            itemPrefab = itemPrefab
        };
        
        itemDatabase.Add(newItem);
    }
    
    // 创建物品实例
    public GameObject CreateItemInstance(int itemID, Vector3 position, Quaternion rotation)
    {
        ItemData itemData = GetItemByID(itemID);
        if (itemData != null && itemData.itemPrefab != null)
        {
            GameObject instance = Instantiate(itemData.itemPrefab, position, rotation);
            
            // 添加ItemInfo组件
            ItemInfo itemInfo = instance.GetComponent<ItemInfo>();
            if (itemInfo == null)
            {
                itemInfo = instance.AddComponent<ItemInfo>();
            }
            
            itemInfo.itemName = itemData.itemName;
            itemInfo.itemID = itemData.itemID;
            
            return instance;
        }
        return null;
    }
    
    // 检查物品是否存在
    public bool ItemExists(int itemID)
    {
        return GetItemByID(itemID) != null;
    }
    
    // 获取所有重要物品
    public List<ItemData> GetImportantItems()
    {
        List<ItemData> importantItems = new List<ItemData>();
        foreach (ItemData item in itemDatabase)
        {
            if (item.isImportant)
            {
                importantItems.Add(item);
            }
        }
        return importantItems;
    }
    
    // 获取数据库中的物品数量
    public int GetItemCount()
    {
        return itemDatabase.Count;
    }
    
    // 清空数据库
    public void ClearDatabase()
    {
        itemDatabase.Clear();
        nextItemID = 1;
    }
    
    // 保存数据库到PlayerPrefs（简单存储）
    public void SaveDatabase()
    {
        string json = JsonUtility.ToJson(new { items = itemDatabase, nextID = nextItemID });
        PlayerPrefs.SetString("ItemDatabase", json);
        PlayerPrefs.Save();
    }
    
    // 从PlayerPrefs加载数据库
    public void LoadDatabase()
    {
        if (PlayerPrefs.HasKey("ItemDatabase"))
        {
            string json = PlayerPrefs.GetString("ItemDatabase");
            var data = JsonUtility.FromJson<DatabaseData>(json);
            itemDatabase = data.items;
            nextItemID = data.nextID;
        }
    }
    
    [System.Serializable]
    private class DatabaseData
    {
        public List<ItemData> items;
        public int nextID;
    }
    
    // 在编辑器中显示物品数据库
    void OnValidate()
    {
        if (autoAssignIDs)
        {
            AssignItemIDs();
        }
    }

    public void SpawnAllItems()
    {
        foreach (var item in itemDatabase)
        {
            for (int i = 0; i < spawnCountPerItem; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                    Random.Range(spawnAreaMin.z, spawnAreaMax.z)
                );
                CreateItemInstance(item.itemID, pos, Quaternion.identity);
            }
        }
    }
}
