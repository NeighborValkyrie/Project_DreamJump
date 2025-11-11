using UnityEngine;

// [v5.2] 네임스페이스를 사용하여 다른 에셋과의 충돌을 방지합니다.
namespace PlatformerGame.Interactions.Interfaces
{
    /// <summary>
    /// 아이템 인터페이스
    /// </summary>
    public interface IItem
    {
        string ItemID { get; }
        string ItemName { get; }
        void Use();
    }
}