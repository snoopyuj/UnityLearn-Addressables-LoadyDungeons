using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

// Used for the Hat selection logic
public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField]
    private Transform m_HatAnchor;

    private GameObject m_HatInstance;
    private AsyncOperationHandle<IList<IResourceLocation>> m_HatsLocationsOpHandle;
    private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;
    private List<string> m_Keys = new() { "Hats", "Fancy" };

    private void Start()
    {
        m_HatsLocationsOpHandle = Addressables.LoadResourceLocationsAsync(m_Keys, Addressables.MergeMode.Intersection);
        m_HatsLocationsOpHandle.Completed += OnHatLocationsLoadComplete;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Destroy(m_HatInstance);
            Addressables.Release(m_HatLoadOpHandle);
            LoadInRandomHat(m_HatsLocationsOpHandle.Result);
        }
    }

    private void OnDisable()
    {
        m_HatLoadOpHandle.Completed -= OnHatLoadComplete;
        m_HatsLocationsOpHandle.Completed -= OnHatLocationsLoadComplete;
    }

    private void OnHatLocationsLoadComplete(AsyncOperationHandle<IList<IResourceLocation>> asyncOperationHandle)
    {
        Debug.Log("AsyncOperationHandle Status: " + asyncOperationHandle.Status);

        if (asyncOperationHandle.Status != AsyncOperationStatus.Succeeded)
        {
            return;
        }

        var results = asyncOperationHandle.Result;
        foreach (var r in results)
        {
            Debug.Log("Hat: " + r.PrimaryKey);
        }

        LoadInRandomHat(results);
    }

    private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_HatInstance = Instantiate(asyncOperationHandle.Result, m_HatAnchor);
        }
    }

    private void LoadInRandomHat(IList<IResourceLocation> resourceLocations)
    {
        var randomIndex = Random.Range(0, resourceLocations.Count);
        var randomHatPrefab = resourceLocations[randomIndex];

        m_HatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(randomHatPrefab);
        m_HatLoadOpHandle.Completed += OnHatLoadComplete;
    }
}