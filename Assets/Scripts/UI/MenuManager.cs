using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private InputReaderSO _input;
    [SerializeField] private UIDocument _menuDocument;
    private string _gameSceneName = "Game";

    private Button _startButton;

    private void OnEnable()
    {
        _input.EnableUIMode();

        _startButton = _menuDocument.rootVisualElement.Q<Button>("start-button");
        _startButton.clicked += OnStartButtonClicked;

        // Focus on first button
        _menuDocument.rootVisualElement.Q<Button>().Focus();
    }

    private void OnDisable()
    {
        _startButton.clicked -= OnStartButtonClicked;
    }

    private void OnStartButtonClicked()
    {
        // Desativa inputs durante a transição
        _input.EnableGameplayMode();

        // Carrega a cena do jogo
        SceneManager.LoadScene(_gameSceneName);
    }
}