# MY NOTES

> ## Why should you use Addressables
> 1. `Flexibility`: from local to remote, from a monolithic bundle to more granular bundles
> 2. `Dependency Management`: automatically loads all dependencies of any assets you load
> 3. `Memory Management`: as you load assets into and out of your game in scripts, the system keeps track of memory allocation
> 4. `Efficient Content Packing`
> 5. `Cloud Build and Content Delivery`: integrated into Unity Gaming Services (UGS), specifically Cloud Content Delivery and Cloud Build
> 6. `Scriptable Build Pipeline (SBP)`
> 7. `Localization`
  
- Window > Asset Management > Addressables > Groups

##  Several Ways to Load Prefabs

### 1) Original Way - `Resources.LoadAsync` (Deprecated)
```csharp
public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField]
    private Transform m_HatAnchor;

    private ResourceRequest m_HatLoadingRequest;  // <--

    private void Start()
    {           
        SetHat(string.Format("Hat{0:00}", GameManager.s_ActiveHat));
    }

    public void SetHat(string hatKey)
    {
        m_HatLoadingRequest = Resources.LoadAsync(hatKey);  // <--
        m_HatLoadingRequest.completed += OnHatLoaded; // <--
    }

    private void OnHatLoaded(AsyncOperation asyncOperation)
    {
        Instantiate(m_HatLoadingRequest.asset as GameObject, m_HatAnchor, false);
    }

    private void OnDisable()
    {
        if (m_HatLoadingRequest != null)
        {
            m_HatLoadingRequest.completed -= OnHatLoaded;
        }
    }
}
```

### 2) `Addressables.LoadAssetAsync(string)`
```csharp
public class PlayerConfigurator : MonoBehaviour
{
    // ...

    [SerializeField]
    private string m_Address; // <--

    private AsyncOperationHandle<GameObject> m_HatLoadOpHandle; // <--

    private void Start() { ... }

    public void SetHat(string hatKey)
    {
        m_HatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(m_Address); // <--
        m_HatLoadOpHandle.Completed += OnHatLoadComplete; // <--
    }

    private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(asyncOperationHandle.Result, m_HatAnchor);
        }
    }

    private void OnDisable()
    {
        m_HatLoadOpHandle.Completed -= OnHatLoadComplete; // <--
    }
}
```

### 3) `AssetReference`
```csharp
public class PlayerConfigurator : MonoBehaviour
{
    // ...

    [SerializeField]
    private AssetReference m_HatAssetReference; // <--

    // ...

    public void SetHat(string hatKey)
    {
        if (!m_HatAssetReference.RuntimeKeyIsValid()) // <--
        {
            return;
        }

        m_HatLoadOpHandle = m_HatAssetReference.LoadAssetAsync<GameObject>(); // <--
        m_HatLoadOpHandle.Completed += OnHatLoadComplete;
    }

    // ...
}
```

### 4) `AssetReferenceGameObject`
```csharp
public class PlayerConfigurator : MonoBehaviour
{
    // ...

    [SerializeField]
    private AssetReferenceGameObject m_HatAssetReference; // <--

    // ...

    public void SetHat(string hatKey)
    {
        // ...

        m_HatLoadOpHandle = m_HatAssetReference.LoadAssetAsync(); // <--
        m_HatLoadOpHandle.Completed += OnHatLoadComplete;
    }

    // ...
}
```

## Load Scene
```csharp
private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;

public static void LoadNextLevel()
{
    // SceneManager.LoadSceneAsync("LoadingScene");

    m_SceneLoadOpHandle = Addressables.LoadSceneAsync("LoadingScene", activateOnLoad: true);
}
```