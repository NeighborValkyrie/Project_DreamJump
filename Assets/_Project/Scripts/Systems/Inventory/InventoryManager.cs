using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.Systems.Inventory;
namespace PlatformerGame.Systems.Inventory



{
    /// <summary>
    /// 인벤토리 시스템 관리
    /// v7.0: 개선 및 확장
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Inventory Settings")]
        [SerializeField] private int maxSlots = 20;

        private Dictionary<string, InventoryItem> items;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                items = new Dictionary<string, InventoryItem>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public bool AddItem(string itemID, int quantity = 1)
        {
            if (items.ContainsKey(itemID))
            {
                items[itemID].quantity += quantity;
            }
            else
            {
                if (items.Count >= maxSlots)
                {
                    Debug.LogWarning("[InventoryManager] 인벤토리가 가득 찼습니다!");
                    return false;
                }

                items.Add(itemID, new InventoryItem
                {
                    itemID = itemID,
                    quantity = quantity
                });
            }

            // 이벤트 발생
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerItemCollected(itemID, quantity);
            }

            Debug.Log($"[InventoryManager] 아이템 추가: {itemID} x{quantity}");
            return true;
        }

        public bool RemoveItem(string itemID, int quantity = 1)
        {
            if (!items.ContainsKey(itemID))
            {
                Debug.LogWarning($"[InventoryManager] 아이템 '{itemID}'가 없습니다.");
                return false;
            }

            items[itemID].quantity -= quantity;

            if (items[itemID].quantity <= 0)
            {
                items.Remove(itemID);
            }

            Debug.Log($"[InventoryManager] 아이템 제거: {itemID} x{quantity}");
            return true;
        }

        public bool HasItem(string itemID, int quantity = 1)
        {
            if (!items.ContainsKey(itemID))
                return false;

            return items[itemID].quantity >= quantity;
        }

        public int GetItemQuantity(string itemID)
        {
            if (!items.ContainsKey(itemID))
                return 0;

            return items[itemID].quantity;
        }

        public void ClearInventory()
        {
            items.Clear();
            Debug.Log("[InventoryManager] 인벤토리 초기화");
        }

        public Dictionary<string, InventoryItem> GetAllItems()
        {
            return items;
        }

        // 저장/로드용
        public List<InventoryItem> GetItemList()
        {
            List<InventoryItem> itemList = new List<InventoryItem>();
            foreach (var item in items.Values)
            {
                itemList.Add(item);
            }
            return itemList;
        }

        public void LoadItems(List<InventoryItem> itemList)
        {
            items.Clear();
            foreach (var item in itemList)
            {
                items.Add(item.itemID, item);
            }
            Debug.Log($"[InventoryManager] {itemList.Count}개 아이템 로드 완료");
        }
    }
}