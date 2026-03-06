public interface ISingleton
{
    ESingletonType Type { get; }
    int InitOrder { get; }
    bool IsInitialized { get; }
    void Initialize();
    void Shutdown(bool destroyObject = false);
}