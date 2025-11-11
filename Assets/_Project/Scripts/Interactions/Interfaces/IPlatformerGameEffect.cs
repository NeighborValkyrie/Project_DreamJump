using UnityEngine;

namespace PlatformerGame.Interactions.Interfaces
{
    /// <summary>
    /// 플랫포머 게임 효과 인터페이스
    /// </summary>
    public interface IPlatformerGameEffect
    {
        void ApplyEffect(GameObject target);
        void RemoveEffect(GameObject target);
    }
}