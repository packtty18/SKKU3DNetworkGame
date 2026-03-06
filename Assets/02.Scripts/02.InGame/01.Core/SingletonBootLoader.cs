using UnityEngine;

public class SingletonBootLoader : MonoBehaviour
{
    [SerializeField] private ESingletonType target;
    
    void Start()
    {
        SingletonRegistry.Instance.InitializeByType(target);
    }

}
