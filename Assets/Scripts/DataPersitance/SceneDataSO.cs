using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneData", menuName = "Scene Management/Scene Data")]
public class SceneDataSO : ScriptableObject
{
    private string m_targetSceneName;
    public string TargetSceneName
    {
        get => m_targetSceneName;
        set => m_targetSceneName = value;
    }

    public string mainMenuScene;
    public string loadingScene;
    public string florestScene;
    public string dungeonScene;
}