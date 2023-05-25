using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Used for the Hat selection logic
public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField]
    private Transform m_HatAnchor;

    private GameObject m_HatInstance;
    private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;

    private void Start()
    {
        LoadInRandomHat();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Destroy(m_HatInstance);
            Addressables.ReleaseInstance(m_HatLoadOpHandle);

            LoadInRandomHat();
        }
    }

    private void LoadInRandomHat()
    {
        int randomIndex = Random.Range(0, 6);
        string hatAddress = string.Format("Hat{0:00}", randomIndex);

        m_HatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(hatAddress);
        m_HatLoadOpHandle.Completed += OnHatLoadComplete;
    }

    private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_HatInstance = Instantiate(asyncOperationHandle.Result, m_HatAnchor);
        }
    }

    private void OnDisable()
    {
        m_HatLoadOpHandle.Completed -= OnHatLoadComplete;
    }
}