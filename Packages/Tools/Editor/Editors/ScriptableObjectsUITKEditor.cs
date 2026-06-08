#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ScriptableObject), true)]
public class ScriptableObjectUITKEditor : MonoBehaviourUITKEditor
{
    // Hérite de tout — aucun code supplémentaire nécessaire.
    // MonoBehaviourUITKEditor gère déjà les deux cas via CreateInspectorGUI().
}
#endif