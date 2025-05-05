using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

public class DeveloperConsole : MonoBehaviour
{
    public static DeveloperConsole Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private UIDocument m_devConsoleDoc;

    [Header("Input Reference")]
    [SerializeField] private InputReaderSO m_inputReader;

    private VisualElement m_root;
    private TextField m_inputField;
    private ScrollView m_scrollView;
    private bool m_isVisible;

    // Commands system
    private Dictionary<string, Action<string[]>> m_commands = new();
    private Dictionary<string, string> m_commandDescriptions = new();
    private List<string> m_commandHistory = new();
    private int m_historyIndex = 0;

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
        RegisterDefaultCommands();
    }

    private void InitializeUI()
    {
        m_root = m_devConsoleDoc.rootVisualElement;
        m_inputField = m_root.Q<TextField>("inputField");
        m_scrollView = m_root.Q<ScrollView>("scrollView");

        m_inputField.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
        m_root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        m_inputReader.OnToggleConsolePerformed += ToggleConsole;
    }
    private void OnDisable()
    {
        m_inputReader.OnToggleConsolePerformed -= ToggleConsole;

        //m_inputField.UnregisterCallback<KeyDownEvent>(OnInputKeyDown);
    }

    private void ToggleConsole()
    {
        m_isVisible = !m_isVisible;
        m_root.style.display = m_isVisible ? DisplayStyle.Flex : DisplayStyle.None;

        if (m_isVisible)
        {
            // Solução definitiva para a aspa
            m_inputField.SetValueWithoutNotify(""); // Limpa sem disparar eventos
            m_inputReader.EnableUIMode();

            // Foco otimizado
            m_inputField.schedule.Execute(() => {
                m_inputField.Focus();
                m_inputField.value = "";
            }).ExecuteLater(10); // Delay maior para garantir
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
                if (!string.IsNullOrWhiteSpace(m_inputField.value))
                {
                    // Bloqueia completamente qualquer outro evento
                    evt.StopImmediatePropagation();

                    // Processa o comando
                    string command = m_inputField.value;
                    m_inputField.value = "";

                    // Agendamento para o próximo frame
                    m_inputField.schedule.Execute(() => {
                        ProcessCommand(command);
                        m_inputField.Focus();
                    }).StartingIn(10);
                }
                break;

            case KeyCode.UpArrow:
                NavigateHistory(1);
                evt.StopPropagation();
                break;

            case KeyCode.DownArrow:
                NavigateHistory(-1);
                evt.StopPropagation();
                break;
        }
    }

    private void NavigateHistory(int direction)
    {
        if (m_commandHistory.Count == 0) return;

        m_historyIndex = Mathf.Clamp(m_historyIndex + direction, 0, m_commandHistory.Count - 1);
        m_inputField.value = m_commandHistory[m_commandHistory.Count - 1 - m_historyIndex];
    }

    private void ProcessCommand(string commandInput)
    {
        string input = commandInput.Trim();
        AddLog($"> {input}");
        m_commandHistory.Add(input);

        string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string command = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (m_commands.TryGetValue(command, out var action))
        {
            try
            {
                action.Invoke(args);
            }
            catch (Exception e)
            {
                AddLog($"Erro: {e.Message}", Color.red);
            }
        }
        else
        {
            AddLog($"Comando desconhecido. Digite 'help' para ajuda.", Color.yellow);
        }
    }

    private void RegisterDefaultCommands()
    {
        // Comando help
        RegisterCommand("help", _ =>
        {
            AddLog("=== COMANDOS DISPONÍVEIS ===", Color.cyan);
            foreach (var cmd in m_commandDescriptions)
            {
                AddLog($"{cmd.Key.PadRight(10)} - {cmd.Value}");
            }
        }, "Mostra esta lista de comandos");

        // Comando noclip
        RegisterCommand("noclip", _ =>
        {
            var player = GameObject.FindWithTag("Player");
            if (!player || !player.TryGetComponent<PlayerStateMachine>(out var sm))
            {
                AddLog("Erro: Jogador não encontrado", Color.red);
                return;
            }

            sm.ToggleNoclip();
            AddLog($"Noclip {(sm.NoclipActive ? "ATIVADO" : "DESATIVADO")}",
                 sm.NoclipActive ? Color.green : Color.red);

            if (sm.NoclipActive)
            {
                AddLog("Controles:", Color.yellow);
                AddLog("WASD: Movimento", Color.white);
                AddLog("Espaço/Ctrl: Subir/Descer", Color.white);
            }
        }, "Ativa/desativa modo voo livre");

        RegisterCommand("clear", _ =>
        {
            m_scrollView.Clear();
        }, "Limpa o console");
    }

    private void RegisterCommand(string command, Action<string[]> action, string description = "")
    {
        string key = command.ToLower();
        m_commands[key] = action;
        m_commandDescriptions[key] = description;
    }

    public void AddLog(string message, Color? color = null)
    {
        var logEntry = new Label(message);
        logEntry.style.color = color ?? Color.white;
        m_scrollView.Add(logEntry);

        // Automatic scroll
        m_scrollView.scrollOffset = new Vector2(
            0,
            m_scrollView.contentContainer.worldBound.height
        );

        // Force update
        m_scrollView.schedule.Execute(() => {
            m_scrollView.scrollOffset = new Vector2(
                0,
                m_scrollView.contentContainer.worldBound.height
            );
        }).ExecuteLater(1);
    }
}