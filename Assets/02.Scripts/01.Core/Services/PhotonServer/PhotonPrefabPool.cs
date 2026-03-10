using System;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PrefabEntry
{
    public string prefabId;
    public GameObject prefab;
}

public class PhotonPrefabPool : MonoSingleton<PhotonPrefabPool>, IPunPrefabPool
{
    [SerializeField, RequiredIn(PrefabKind.PrefabAsset)] private List<PrefabEntry> _initEntry = new();

    private readonly Dictionary<string, GameObject> _prefabById = new ();
    private readonly Dictionary<string, Queue<GameObject>> _poolById = new ();
    private readonly Dictionary<GameObject, string> _instanceToId = new ();
    

    protected override void OnInitialize()
    {
        BuildPrefabLookup(_initEntry);
        PhotonNetwork.PrefabPool = this;
    }

    private void BuildPrefabLookup(List<PrefabEntry> entries)
    {
        DestroyAllInstance();
        _prefabById.Clear();
        _poolById.Clear();
        _instanceToId.Clear();

        foreach (PrefabEntry entry in entries)
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
            Debug.LogError("PhotonPrefabPool : prefab pool is EMPTY");
            return null;
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
    
    private void DestroyAllInstance()
    {
        foreach (KeyValuePair<string, Queue<GameObject>> pair in _poolById)
        {
            Queue<GameObject> queue = pair.Value;

            while (queue.Count > 0)
            {
                GameObject instance = queue.Dequeue();

                if (instance != null)
                {
                    UnityEngine.Object.Destroy(instance);
                }
            }
        }

        foreach (GameObject instance in new List<GameObject>(_instanceToId.Keys))
        {
            if (instance != null)
            {
                UnityEngine.Object.Destroy(instance);
            }
        }
    }
    
    protected override void OnShutdown()
    {
        if (ReferenceEquals(PhotonNetwork.PrefabPool, this))
        {
            PhotonNetwork.PrefabPool = new DefaultPool();
        }

        DestroyAllInstance();

        _prefabById.Clear();
        _poolById.Clear();
        _instanceToId.Clear();
    }

    
}
