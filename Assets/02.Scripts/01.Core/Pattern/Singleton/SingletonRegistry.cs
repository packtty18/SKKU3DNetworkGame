using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SingletonRegistry : MonoBehaviour
{
    private static SingletonRegistry instance;
    //Lazy 생성 방식. 다른 Registry 없이 싱글톤이 생성되서 참조하면 즉시 생성
    public static SingletonRegistry Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("[SingletonRegistry]");
                instance = go.AddComponent<SingletonRegistry>();
                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }
    
    [ShowInInspector, ReadOnly] private readonly Dictionary<Type, ISingleton> singletons = new Dictionary<Type, ISingleton>();
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Register(ISingleton singleton)
    {
        Type type = singleton.GetType();
        if (singletons.ContainsKey(type))
        {
            Debug.LogWarning($"Singleton already registered: {type.Name}");
            return;
        }

        singletons.Add(type, singleton);
    }

    public void Unregister(ISingleton singleton)
    {
        Type type = singleton.GetType();
        singletons.Remove(type);
    }
    
    public bool TryGet<T>(out T result) where T : class, ISingleton
    {
        if (singletons.TryGetValue(typeof(T), out ISingleton singleton))
        {
            result = singleton as T;
            return true;
        }

        result = null;
        return false;
    }
    
    public void InitializeByType(ESingletonType type)
    {
        List<ISingleton> targets = singletons.Values
            .Where(s => s.Type == type)
            .OrderBy(s => s.InitOrder)
            .ToList();

        foreach (ISingleton singleton in targets)
        {
            if (singleton.IsInitialized == false)
            {
                singleton.Initialize();
            }
        }
    }

    public void ShutdownByType(ESingletonType type)
    {
        var targets = singletons.Values
            .Where(s => s.Type == type)
            .OrderByDescending(s => s.InitOrder);

        foreach (var singleton in targets)
        {
            if (singleton.IsInitialized)
            {
                singleton.Shutdown();
            }
        }
    }
    
    
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
