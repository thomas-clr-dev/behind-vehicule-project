using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject contenant une séquence de sous-titres
/// </summary>
[CreateAssetMenu(fileName = "NewSubtitleSequence", menuName = "Subtitle System/Subtitle Sequence")]
public class SubtitleSequence : ScriptableObject
{
    [Tooltip("Liste des sous-titres dans l'ordre")]
    public List<SubtitleData> Subtitles = new List<SubtitleData>();

    [Tooltip("Délai entre chaque sous-titre (en secondes)")]
    public float DelayBetweenSubtitles = 0.5f;
}