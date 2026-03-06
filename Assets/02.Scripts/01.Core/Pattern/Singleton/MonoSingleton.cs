using UnityEngine;

public abstract class MonoSingleton<T> :MonoBehaviour, ISingleton where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance => _instance;

    //아래값들은 개발 단계에서는 인스펙터에서 조정. 추후 자식 스크립트 override로 고정
    [SerializeField] private ESingletonType _type =  ESingletonType.App;
    [SerializeField] private int _initOrder = 0;
    [SerializeField] private bool dontDestroyOnLoad = true;
    
    private bool _isInitialized = false;
    
    
    public ESingletonType Type => _type;
    public int InitOrder => _initOrder;
    public bool IsInitialized => _isInitialized;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        SingletonRegistry.Instance.Register(this);
        if (_initOrder == 0)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        OnInitialize();
        _isInitialized = true;
        Debug.Log($"{typeof(T)} is Initialized");
    }

    public void Shutdown(bool destroyObject = false)
    {
        if (ReferenceEquals(_instance, this) == false)
        {
            return;
        }

        if (_isInitialized)
        {
            OnShutdown();
            _isInitialized = false;
            Debug.Log($"{typeof(T)} is Initialized");
        }

        if (destroyObject)
        {
            if (SingletonRegistry.Instance != null)
            {
                SingletonRegistry.Instance.Unregister(this);
            }

            Destroy(gameObject);
        }
    }
    
    protected abstract void OnInitialize();
    protected abstract void OnShutdown();

    
    private void OnDestroy()
    {
        
        //중복오브젝트의 파괴일경우 _instance를 지우지 않도록
        if (ReferenceEquals(_instance, this))
        {
            _instance = null;
        }
    }
}
