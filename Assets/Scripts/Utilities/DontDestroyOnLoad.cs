using UnityEngine;

namespace PlatformerGame.Utilities
{
    /// <summary>
    /// 씬 전환 시 오브젝트 유지
    /// v7.0: 새로 추가됨
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}