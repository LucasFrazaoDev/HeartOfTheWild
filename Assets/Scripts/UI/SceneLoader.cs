using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument m_loadingDocument;
    [SerializeField] private float m_minLoadingTime = 5.0f;

    private ProgressBar m_loadingProgress;
    private static string s_targetScene;

    private void Awake()
    {
        var root = m_loadingDocument.rootVisualElement;
        m_loadingProgress = root.Q<ProgressBar>("loadingBar-progressBar");

        // Configura valores iniciais (importante!)
        m_loadingProgress.lowValue = 0f;
        m_loadingProgress.highValue = 1f;
        m_loadingProgress.value = 0f;
    }

    private void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    public static void LoadScene(string targetScene)
    {
        s_targetScene = targetScene;
        SceneManager.LoadScene("Loading");
    }

    private IEnumerator LoadTargetScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Game");
        op.allowSceneActivation = false;

        float elapsedTime = 0f;
        float progress = 0f;

        while (!op.isDone)
        {
            // Calcula o progresso real (0-0.9) e ajusta para (0-1)
            float loadProgress = Mathf.Clamp01(op.progress / 0.9f);

            // Progresso baseado no tempo mínimo
            float timeProgress = Mathf.Clamp01(elapsedTime / m_minLoadingTime);

            // Usa o menor valor entre os dois progressos
            progress = Mathf.Min(loadProgress, timeProgress);

            m_loadingProgress.value = progress;

            if (progress >= 1f)
            {
                op.allowSceneActivation = true;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}