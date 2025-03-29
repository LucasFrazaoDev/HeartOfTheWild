using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument m_loadingDocument;
    [SerializeField] private SceneDataSO m_sceneData;

    [Tooltip("Minimum time to load the screen")]
    [SerializeField] private float m_minLoadingTime = 3.0f;

    private ProgressBar m_loadingProgressBar;

    private const string k_loadingBar = "loadingBar-progressBar";

    private void Awake()
    {
        var root = m_loadingDocument.rootVisualElement;
        m_loadingProgressBar = root.Q<ProgressBar>(k_loadingBar);
    }

    private void OnEnable()
    {
        SetProgressBarValues();
    }

    private void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    private void SetProgressBarValues()
    {
        m_loadingProgressBar.lowValue = 0f;
        m_loadingProgressBar.highValue = 1f;
        m_loadingProgressBar.value = 0f;
    }

    private IEnumerator LoadTargetScene()
    {
        string sceneName = m_sceneData.TargetSceneName;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float elapsedTime = 0f;
        float progress = 0f;

        while (!op.isDone)
        {
            // Calcula o progresso real (0-0.9) e ajusta para (0-1)
            float loadProgress = Mathf.Clamp01(op.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsedTime / m_minLoadingTime);

            // Usa o menor valor entre os dois progressos
            progress = Mathf.Min(loadProgress, timeProgress);

            m_loadingProgressBar.value = progress;

            if (progress >= 1f)
                op.allowSceneActivation = true;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}