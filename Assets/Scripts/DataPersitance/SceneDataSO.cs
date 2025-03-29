using Unity.VisualScripting;
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

    public SceneAsset mainMenuScene;
    public SceneAsset loadingScene;
    public SceneAsset gameScene;
}