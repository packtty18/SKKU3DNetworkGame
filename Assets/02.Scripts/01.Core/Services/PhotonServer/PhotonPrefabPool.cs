using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[DisallowMultipleComponent]
public class PhotonPrefabPool : MonoBehaviour, IPunPrefabPool
{
    [Serializable]
    private class PrefabEntry
    {
        public string prefabId;
        public GameObject prefab;
    }

    [SerializeField] private List<PrefabEntry> prefabs = new List<PrefabEntry>();

    private readonly Dictionary<string, GameObject> _prefabById = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, Queue<GameObject>> _poolById = new Dictionary<string, Queue<GameObject>>();
    private readonly Dictionary<GameObject, string> _instanceToId = new Dictionary<GameObject, string>();

    private void Awake()
    {
        BuildPrefabLookup();
    }

    private void BuildPrefabLookup()
    {
        _prefabById.Clear();

        foreach (PrefabEntry entry in prefabs)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.prefabId) || entry.prefab == null)
            {
                continue;
            }

            _prefabById[entry.prefabId] = entry.prefab;

            if (!_poolById.ContainsKey(entry.prefabId))
            {
                _poolById[entry.prefabId] = new Queue<GameObject>();
            }
        }
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (_prefabById.Count == 0)
        {
            BuildPrefabLookup();
        }

        if (!_prefabById.TryGetValue(prefabId, out GameObject sourcePrefab) || sourcePrefab == null)
        {
            Debug.LogError($"PhotonPrefabPool: prefabId '{prefabId}' is not registered.");
            return null;
        }

        Queue<GameObject> queue = _poolById[prefabId];
        GameObject instance = null;

        while (queue.Count > 0 && instance == null)
        {
            instance = queue.Dequeue();
        }

        if (instance == null)
        {
            instance = UnityEngine.Object.Instantiate(sourcePrefab, position, rotation);
            _instanceToId[instance] = prefabId;
        }
        else
        {
            instance.transform.SetPositionAndRotation(position, rotation);
        }

        // PUN expects pooled objects returned from IPunPrefabPool.Instantiate to be inactive.
        instance.SetActive(false);
        return instance;
    }

    public void Destroy(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return;
        }

        if (!_instanceToId.TryGetValue(gameObject, out string prefabId))
        {
            UnityEngine.Object.Destroy(gameObject);
            return;
        }

        if (!_poolById.TryGetValue(prefabId, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            _poolById[prefabId] = queue;
        }

        gameObject.SetActive(false);
        queue.Enqueue(gameObject);
    }
}
