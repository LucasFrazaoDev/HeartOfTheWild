// SceneDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "Scene Management/Scene Data")]
public class SceneDataSO : ScriptableObject
{
    public string targetSceneName;
}