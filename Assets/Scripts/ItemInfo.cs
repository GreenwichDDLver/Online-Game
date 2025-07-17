using UnityEngine;

[DisallowMultipleComponent]
public class ItemInfo : MonoBehaviour
{
    [Header("物品唯一编号（与ItemSystem数据库同步）")]
    public int itemID;
    [Header("物品名称（与ItemSystem数据库同步）")]
    public string itemName;
    [Header("物品描述（可选）")]
    [TextArea]
    public string description;
}
