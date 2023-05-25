using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;

    private AsyncOperation m_SceneOperation;

    [SerializeField]
    private Slider m_LoadingSlider;

    [SerializeField]
    private GameObject m_PlayButton, m_LoadingText;

    private void Awake()
    {
        StartCoroutine(LoadNextLevel("Level_0" + GameManager.s_CurrentLevel));
    }

    private IEnumerator LoadNextLevel(string level)
    {
        m_SceneLoadOpHandle = Addressables.LoadSceneAsync(level, activateOnLoad: true);

        while (!m_SceneLoadOpHandle.IsDone)
        {
            var progress = m_SceneLoadOpHandle.PercentComplete;

            m_LoadingSlider.value = progress;

            if (progress >= 0.9f && !m_PlayButton.activeInHierarchy)
            {
                m_PlayButton.SetActive(true);
            }

            yield return null;
        }

        Debug.Log($"Loaded Level {level}");
    }
}