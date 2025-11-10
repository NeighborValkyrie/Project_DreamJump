using UnityEngine;

namespace PlatformerGame.Interactions.Items
{
    /// <summary>
    /// 사용 가능한 아이템 (포션 등)
    /// v7.0: 새로 추가됨
    /// </summary>
    public class UsableItem : MonoBehaviour, Interfaces.IItem
    {
        [Header("Item Settings")]
        [SerializeField] private string itemID = "health_potion";
        [SerializeField] private string itemName = "체력 포션";

        [Header("Effects")]
        [SerializeField] private int healAmount = 50;
        [SerializeField] private string useSFX = "ItemUse";

        public string ItemID => itemID;
        public string ItemName => itemName;

        public void Use()
        {
            // 체력 회복 (향후 HealthSystem 구현 시 연결)
            Debug.Log($"[UsableItem] {itemName} 사용! 체력 {healAmount} 회복");

            // SFX 재생
            if (Systems.Audio.AudioManager.Instance != null)
            {
                Systems.Audio.AudioManager.Instance.Play(useSFX);
            }

            // 인벤토리에서 제거
            if (Systems.Inventory.InventoryManager.Instance != null)
            {
                Systems.Inventory.InventoryManager.Instance.RemoveItem(itemID, 1);
            }

            // 이벤트 발생
            if (Systems.Events.GameEventManager.Instance != null)
            {
                Systems.Events.GameEventManager.Instance.TriggerItemUsed(itemID, 1);
            }
        }
    }
}