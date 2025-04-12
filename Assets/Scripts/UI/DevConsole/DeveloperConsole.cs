using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class DeveloperConsole : MonoBehaviour
{
    public static DeveloperConsole Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private UIDocument m_devConsoleDoc;

    [Header("Input Reference")]
    [SerializeField] private InputReaderSO m_inputReader;

    private VisualElement m_root;
    private ScrollView m_scrollView;
    private TextField m_inputField;
    private bool m_isVisible;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUI();
    }

    private void InitializeUI()
    {
        m_root = m_devConsoleDoc.rootVisualElement;
        m_inputField = m_root.Q<TextField>("inputField");
        m_scrollView = m_root.Q<ScrollView>("scrollView");

        m_inputField.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
        m_inputField.RegisterCallback<NavigationSubmitEvent>(OnSubmit);
        m_inputField.value = "";

        m_root.style.display = DisplayStyle.None;
        m_isVisible = false;
    }

    private void OnEnable()
    {
        m_inputReader.OnToggleConsolePerformed += ToggleConsole;
    }

    private void OnDisable()
    {
        m_inputReader.OnToggleConsolePerformed -= ToggleConsole;
    }

    private void ToggleConsole()
    {
        m_isVisible = !m_isVisible;
        m_root.style.display = m_isVisible ? DisplayStyle.Flex : DisplayStyle.None;

        if (m_isVisible)
        {
            m_inputField.Focus();
            m_inputReader.EnableUIMode();

            m_inputField.schedule.Execute(() => {
                m_inputField.value = "";
                m_inputField.Q(TextField.textInputUssName).Focus();
            }).ExecuteLater(10); // Small delay
        }
        else
        {
            m_inputReader.EnableGameplayMode();
        }
    }

    private void OnInputKeyDown(KeyDownEvent evt)
    {
        switch (evt.keyCode)
        {
            case KeyCode.Return:
            case KeyCode.KeypadEnter:
                ProcessCommand();
                evt.StopPropagation();
                break;

            case KeyCode.Escape:
                ToggleConsole();
                evt.StopPropagation();
                break;
        }
    }

    private void OnSubmit(NavigationSubmitEvent evt)
    {
        ProcessCommand();
        evt.StopPropagation();
    }

    private void ProcessCommand()
    {
        if (string.IsNullOrWhiteSpace(m_inputField.value))
            return;

        AddLog(m_inputField.value);
        m_inputField.value = "";

        // Force focus back to the input field
        m_inputField.schedule.Execute(() => {
            m_inputField.Q(TextField.textInputUssName).Focus();
        }).ExecuteLater(100);
    }

    public void AddLog(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        Label logEntry = new Label(message);
        m_scrollView.Add(logEntry);

        // Auto-scroll
        m_scrollView.scrollOffset = new Vector2(
            0,
            m_scrollView.contentContainer.layout.height + 100f
        );
    }
}