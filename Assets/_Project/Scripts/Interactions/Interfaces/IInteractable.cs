using UnityEngine;

namespace PlatformerGame.Interactions.Interfaces
{
    /// <summary>
    /// 상호작용 가능한 오브젝트 인터페이스
    /// </summary>
    public interface IInteractable
    {
        void Interact(GameObject interactor);
        string GetInteractionPrompt();
    }
}