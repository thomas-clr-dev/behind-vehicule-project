using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayingSelectedMusic))]
public class PlayingSelectedMusicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Afficher l'Inspector par défaut
        DrawDefaultInspector();

        PlayingSelectedMusic script = (PlayingSelectedMusic)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Contrôles", EditorStyles.boldLabel);

        // Bouton Refresh
        if (GUILayout.Button("🔄 Rafraîchir les AudioSources", GUILayout.Height(30)))
        {
            script.RefreshAudioSources();
        }

        EditorGUILayout.Space(5);

        // Boutons de navigation
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("◀ Précédent", GUILayout.Height(30)))
        {
            script.SelectPrevious();
        }

        if (GUILayout.Button("▶ Suivant", GUILayout.Height(30)))
        {
            script.SelectNext();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // Bouton Appliquer
        if (GUILayout.Button("✔ Appliquer la sélection", GUILayout.Height(30)))
        {
            script.ApplySelection();
        }

        EditorGUILayout.Space(10);

        // Liste des AudioSources avec boutons individuels
        AudioSource[] audioSources = script.GetChildAudioSources();

        if (audioSources != null && audioSources.Length > 0)
        {
            EditorGUILayout.LabelField($"AudioSources disponibles ({audioSources.Length})", EditorStyles.boldLabel);

            int currentIndex = script.GetSelectedIndex();

            for (int i = 0; i < audioSources.Length; i++)
            {
                if (audioSources[i] == null) continue;

                EditorGUILayout.BeginHorizontal();

                // Indicateur de sélection
                string indicator = (i == currentIndex) ? "▶" : "  ";
                GUIStyle labelStyle = (i == currentIndex) ? EditorStyles.boldLabel : EditorStyles.label;

                EditorGUILayout.LabelField($"{indicator} [{i}]", GUILayout.Width(40));
                EditorGUILayout.LabelField(audioSources[i].gameObject.name, labelStyle);

                // Bouton pour sélectionner
                if (GUILayout.Button("Sélectionner", GUILayout.Width(100)))
                {
                    script.SelectByIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Aucune AudioSource trouvée. Cliquez sur 'Rafraîchir'.", MessageType.Warning);
        }
    }
}