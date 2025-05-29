using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReaderSO m_input;
    [SerializeField] private SceneDataSO m_sceneData;
    [SerializeField] private UIDocument m_menuDocument;

    private Button m_startButton;
    private Button m_settingsButton;
    private Button m_exitButton;

    private const string k_startButton = "start-button";
    private const string k_settingsButton = "settings-button";
    private const string k_exitButton = "exit-button";

    private void OnEnable()
    {
        m_input.EnableUIMode();

        m_startButton = m_menuDocument.rootVisualElement.Q<Button>(k_startButton);
        m_settingsButton = m_menuDocument.rootVisualElement.Q<Button>(k_settingsButton);
        m_exitButton = m_menuDocument.rootVisualElement.Q<Button>(k_exitButton);

        m_startButton.clicked += OnStartButtonClicked;
        m_settingsButton.clicked += OnSettingsButtonClicked;
        m_exitButton.clicked += OnExitButtonClicked;

        // Focus on first button
        m_startButton.Focus();
    }

    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void OnDisable()
    {
        m_startButton.clicked -= OnStartButtonClicked;
        m_settingsButton.clicked -= OnSettingsButtonClicked;
        m_exitButton.clicked -= OnExitButtonClicked;
    }

    private void OnStartButtonClicked()
    {
        m_input.EnableGameplayMode();
        m_sceneData.TargetSceneName = m_sceneData.dungeonScene;
        SceneManager.LoadScene(m_sceneData.loadingScene);
    }

    private void OnSettingsButtonClicked()
    {
        
    }

    private void OnExitButtonClicked()
    {
        ExitGame();
    }

    public void ExitGame()
    {
        m_input.EnableGameplayMode();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}