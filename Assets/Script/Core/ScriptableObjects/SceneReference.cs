using UnityEngine;

[CreateAssetMenu(fileName = "SceneReference", menuName = "ScriptableObjects/SceneReference", order = 1)]
public class SceneReference : ScriptableObject
{
    [SerializeField] private string sceneName;

    public string SceneName => sceneName;
}
