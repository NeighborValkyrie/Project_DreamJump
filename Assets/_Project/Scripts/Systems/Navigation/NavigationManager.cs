using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Systems.Navigation
{
    /// <summary>
    /// 웨이포인트 추적 시스템
    /// v7.0: 기존 코드 유지
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        public static NavigationManager Instance { get; private set; }

        [System.Serializable]
        public class Waypoint
        {
            public string id;
            public Transform transform;
        }

        [Header("Waypoints")]
        [SerializeField] private List<Waypoint> waypoints = new List<Waypoint>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterWaypoint(string id, Transform waypointTransform)
        {
            Waypoint existingWaypoint = waypoints.Find(w => w.id == id);

            if (existingWaypoint != null)
            {
                existingWaypoint.transform = waypointTransform;
            }
            else
            {
                waypoints.Add(new Waypoint { id = id, transform = waypointTransform });
            }
        }

        public Transform GetWaypoint(string id)
        {
            Waypoint waypoint = waypoints.Find(w => w.id == id);
            return waypoint?.transform;
        }

        public Vector3 GetWaypointPosition(string id)
        {
            Transform waypointTransform = GetWaypoint(id);
            return waypointTransform != null ? waypointTransform.position : Vector3.zero;
        }
    }
}