using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Systems.Pool
{
    /// <summary>
    /// 오브젝트 풀링 시스템
    /// v7.0: 버그 수정 완료
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [Header("Pools")]
        public List<Pool> pools;

        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

private void InitializePools()
{
    poolDictionary = new Dictionary<string, Queue<GameObject>>();

    // ⭐ null 체크 추가
    if (pools == null || pools.Count == 0)
    {
        Debug.LogWarning("[ObjectPoolManager] Pools 배열이 비어있습니다. 풀을 생성하지 않습니다.");
        return;
    }

    foreach (Pool pool in pools)
    {
        // ⭐ prefab null 체크 추가
        if (pool.prefab == null)
        {
            Debug.LogWarning($"[ObjectPoolManager] Pool '{pool.tag}'의 Prefab이 할당되지 않았습니다.");
            continue;
        }

        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < pool.size; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            objectPool.Enqueue(obj);
        }

        poolDictionary.Add(pool.tag, objectPool);
    }

    Debug.Log($"[ObjectPoolManager] {poolDictionary.Count}개 풀 초기화 완료");
}

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPoolManager] 풀 태그 '{tag}'를 찾을 수 없습니다.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPoolManager] 풀 태그 '{tag}'를 찾을 수 없습니다.");
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(transform);
        }
    }
}