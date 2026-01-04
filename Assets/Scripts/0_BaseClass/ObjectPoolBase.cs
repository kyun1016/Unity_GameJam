using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Unity 2021+의 UnityEngine.Pool을 활용한 범용 오브젝트 풀 기본 클래스입니다.
/// </summary>
/// <typeparam name="T">풀링할 컴포넌트 타입</typeparam>
public abstract class ObjectPoolBase<T> : MonoBehaviour where T : Component
{
    [Header("Pool Settings")]
    [SerializeField] protected T _prefab;
    [Tooltip("초기 할당 크기")]
    [SerializeField] protected int _defaultCapacity = 10;
    [Tooltip("최대 할당 크기 (이 이상 넘어가면 Destroy)")]
    [SerializeField] protected int _maxSize = 100;
    [Tooltip("이미 반환된 아이템을 다시 반환하려는지 체크 (에러 방지용, 성능 영향 있음)")]
    [SerializeField] protected bool _collectionCheck = true;

    private IObjectPool<T> _pool;

    /// <summary>
    /// 외부에서 접근 가능한 풀 프로퍼티
    /// </summary>
    public IObjectPool<T> Pool
    {
        get
        {
            if (_pool == null)
                InitializePool();
            return _pool;
        }
    }

    protected virtual void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        _pool = new ObjectPool<T>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: _collectionCheck,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    /// <summary>
    /// 풀에서 새로운 아이템을 생성할 때 호출됩니다.
    /// </summary>
    protected virtual T CreatePooledItem()
    {
        if (_prefab == null)
        {
            Debug.LogError($"[{this.name}] Prefab is missing! Please assign a prefab in the inspector.");
            return null;
        }

        T instance = Instantiate(_prefab, transform);
        instance.gameObject.name = $"{_prefab.name}_Pooled";
        return instance;
    }

    /// <summary>
    /// 풀에서 아이템을 가져올 때 호출됩니다. (SetActive true)
    /// </summary>
    protected virtual void OnTakeFromPool(T item)
    {
        item.gameObject.SetActive(true);
    }

    /// <summary>
    /// 풀에 아이템을 반환할 때 호출됩니다. (SetActive false)
    /// </summary>
    protected virtual void OnReturnedToPool(T item)
    {
        item.gameObject.SetActive(false);
    }

    /// <summary>
    /// 풀의 최대 크기를 초과하여 아이템이 파괴될 때 호출됩니다.
    /// </summary>
    protected virtual void OnDestroyPoolObject(T item)
    {
        Destroy(item.gameObject);
    }

    /// <summary>
    /// 객체를 가져옵니다.
    /// </summary>
    public T Get()
    {
        return Pool.Get();
    }

    /// <summary>
    /// 객체를 반환합니다.
    /// </summary>
    public void Release(T item)
    {
        Pool.Release(item);
    }

    /// <summary>
    /// 풀을 비웁니다.
    /// </summary>
    public void Clear()
    {
        Pool.Clear();
    }
}
