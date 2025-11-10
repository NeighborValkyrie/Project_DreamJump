using UnityEngine;

namespace PlatformerGame.Interactions.Items
{
    /// <summary>
    /// 수집 가능한 아이템
    /// v7.0: 새로 추가됨
    /// </summary>
    public class CollectibleItem : MonoBehaviour
    {
        [Header("Item Settings")]
        [SerializeField] private string itemID = "coin";
        [SerializeField] private string itemName = "코인";
        [SerializeField] private Systems.Inventory.ItemType itemType = Systems.Inventory.ItemType.Currency;
        [SerializeField] private int value = 1;

        [Header("Effects")]
        [SerializeField] private string collectSFX = "ItemCollect";
        [SerializeField] private GameObject collectEffect;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CollectItem();
            }
        }

        private void CollectItem()
        {
            // 인벤토리에 추가
            if (Systems.Inventory.InventoryManager.Instance != null)
            {
                bool success = Systems.Inventory.InventoryManager.Instance.AddItem(itemID, value);

                if (!success)
                {
                    Debug.LogWarning("[CollectibleItem] 인벤토리가 가득 찼습니다!");
                    return;
                }
            }

            // SFX 재생
            if (Systems.Audio.AudioManager.Instance != null)
            {
                Systems.Audio.AudioManager.Instance.Play(collectSFX);
            }

            // 수집 이펙트
            if (collectEffect != null)
            {
                if (Systems.Pool.ObjectPoolManager.Instance != null)
                {
                    Systems.Pool.ObjectPoolManager.Instance.SpawnFromPool(
                        "ItemEffect",
                        transform.position,
                        Quaternion.identity
                    );
                }
                else
                {
                    Instantiate(collectEffect, transform.position, Quaternion.identity);
                }
            }

            // 오브젝트 제거
            Destroy(gameObject);
        }

        public string GetItemID() => itemID;
        public string GetItemName() => itemName;
        public int GetValue() => value;
    }
}