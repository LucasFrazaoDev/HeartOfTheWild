using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private InputReaderSO m_input;
    [SerializeField] private UIDocument m_menuDocument;
    private string _gameSceneName = "Game";

    private Button _startButton;
    private Button _exitButton; // Novo botão de saída

    private void OnEnable()
    {
        m_input.EnableUIMode();

        _startButton = m_menuDocument.rootVisualElement.Q<Button>("start-button");
        _exitButton = m_menuDocument.rootVisualElement.Q<Button>("exit-button");

        _startButton.clicked += OnStartButtonClicked;
        _exitButton.clicked += OnExitButtonClicked; // Novo evento

        // Focus on first button
        _startButton.Focus();
    }

    private void OnDisable()
    {
        _startButton.clicked -= OnStartButtonClicked;
        _exitButton.clicked -= OnExitButtonClicked;
    }

    private void OnStartButtonClicked()
    {
        m_input.EnableGameplayMode();
        SceneManager.LoadScene("Loading");
    }

    // Novo método para sair do jogo
    private void OnExitButtonClicked()
    {
        ExitGame();
    }

    public void ExitGame()
    {
        // Desativa inputs
        m_input.EnableGameplayMode(); // Ou seu método para desativar todos inputs

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}