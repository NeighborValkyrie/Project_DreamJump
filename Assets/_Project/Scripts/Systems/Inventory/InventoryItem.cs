using UnityEngine;

namespace PlatformerGame.Systems.Inventory
{
    /// <summary>
    /// 인벤토리 아이템 데이터
    /// v7.0: 새로 추가됨
    /// </summary>

    public class InventoryItem
    {
        public string itemID;
        public int quantity;
        public ItemType itemType;
        public string itemName;
        public string description;

        // 생성자
        public InventoryItem()
        {
            quantity = 1;
            itemType = ItemType.Currency;
        }

        public InventoryItem(string id, int qty, ItemType type, string name = "")
        {
            itemID = id;
            quantity = qty;
            itemType = type;
            itemName = string.IsNullOrEmpty(name) ? id : name;
        }
    }

    /// <summary>
    /// 아이템 타입 열거형
    /// </summary>
    public enum ItemType
    {
        Currency,   // 코인
        Consumable, // 소비 아이템 (포션)
        Key,        // 열쇠
        QuestItem,  // 퀘스트 아이템
        Equipment   // 장비
    }
}