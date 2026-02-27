using UnityEngine;

public class EnemyAbility : MonoBehaviour
{
    protected EnemyController _owner { get; private set; }

    protected virtual void Awake()
    {
        _owner = GetComponentInParent<EnemyController>();
    }
}
