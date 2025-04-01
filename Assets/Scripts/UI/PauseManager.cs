using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private UIDocument m_pauseDocument;

    [Header("Game Data References")]
    [SerializeField] private InputReaderSO m_input;
    [SerializeField] private SceneDataSO m_sceneData;

    private VisualElement m_pauseContainer;
    private Button m_resumeButton;
    private Button m_backMainMenuButton;

    private bool m_isPaused = false;

    private const string k_pauseContainer = "pause-container";
    private const string k_resumeButton = "resume-button";
    private const string k_backMainMenuButton = "backMainMenu-button";

    private void Awake()
    {
        var root = m_pauseDocument.rootVisualElement;
        InitializeUIElements(root);
    }

    private void OnEnable()
    {
        m_pauseContainer.style.display = DisplayStyle.None;

        m_resumeButton.clicked += ResumeGame;
        m_backMainMenuButton.clicked += GoToMainMenu;

        m_input.OnPausePerformed += TogglePause;
    }

    private void OnDisable()
    {
        m_input.OnPausePerformed -= TogglePause;
    }

    private void InitializeUIElements(VisualElement root)
    {
        m_pauseContainer = root.Q<VisualElement>(k_pauseContainer);
        m_resumeButton = root.Q<Button>(k_resumeButton);
        m_backMainMenuButton = root.Q<Button>(k_backMainMenuButton);
    }

    private void TogglePause()
    {
        m_isPaused = !m_isPaused;

        if (m_isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        m_pauseContainer.style.display = DisplayStyle.Flex;
        m_input.EnableUIMode();
        m_resumeButton.Focus();
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        m_isPaused = false;
        m_pauseContainer.style.display = DisplayStyle.None;
        m_input.EnableGameplayMode();
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        m_sceneData.TargetSceneName = m_sceneData.mainMenuScene;
        SceneManager.LoadScene(m_sceneData.loadingScene);
    }
}