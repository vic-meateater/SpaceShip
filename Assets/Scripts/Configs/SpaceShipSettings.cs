using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SpaceShipSettings", menuName = "Geekbrains/Settings/Space Ship Settings")]
    public class SpaceShipSettings : ScriptableObject
    {
        public float Acceleration => acceleration;
        public float ShipSpeed => shipSpeed;
        public float Faster => faster;
        public float NormalFov => normalFov;
        public float FasterFov => fasterFov;
        public float ChangeFovSpeed => changeFovSpeed;
        public float WorldScreenHeight => _worldScreenHeight;
        public int RenderTextureHeight => _renderTextureHeight;
        public List<GameObject> SpawnPoints => _spawnPoints;
        public GameObject Prefab => _prefab;
        public GameObject SpawnPointsHolder => _spawnPointsHolder;

        [SerializeField, Range(.01f, 0.1f)] private float acceleration;
        [SerializeField, Range(1f, 2000f)] private float shipSpeed;
        [SerializeField, Range(1f, 5f)] private int faster;
        [SerializeField, Range(.01f, 179)] private float normalFov = 60;
        [SerializeField, Range(.01f, 179)] private float fasterFov = 30;
        [SerializeField, Range(.1f, 5f)] private float changeFovSpeed = .5f;

        [SerializeField, HideInInspector] float _worldScreenHeight = 5;
        [SerializeField, HideInInspector] int _renderTextureHeight = 1080;
        [SerializeField, HideInInspector] private List<GameObject> _spawnPoints;
        [SerializeField, HideInInspector] GameObject _prefab;
        [SerializeField, HideInInspector] GameObject _spawnPointsHolder;

        public void SetWorldScreenHeight(float value)
        {
            _worldScreenHeight = value;
        }

        public void SetRenderTextureHeight(int value)
        {
            _renderTextureHeight = value;
        }

        public void SetPrefab(GameObject prefab)
        {
            _prefab = prefab;
        }

        public void SetPointsHolder(GameObject holder)
        {
            _spawnPointsHolder = holder;
        }

        public void AddSpawnPoint(GameObject point)
        {
            if (_spawnPoints == null) _spawnPoints = new List<GameObject>();

            if (_spawnPoints.Contains(point)) return;

            _spawnPoints.Add(point);
        }

        public void RemoveSpawnPoint(GameObject point)
        {
            _spawnPoints.Remove(point);
        }

        public void SetXCoord(int index, float value)
        {
            var pos = _spawnPoints[index].transform.position;
            pos.x = value;
            _spawnPoints[index].transform.position = pos;
        }

        public void SetZCoord(int index, float value)
        {
            var pos = _spawnPoints[index].transform.position;
            pos.z = value;
            _spawnPoints[index].transform.position = pos;
        }

    }
}
