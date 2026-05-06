using UnityEngine;

/// <summary>
/// ScriptableObject contenant les données d'un dialogue/sous-titre
/// </summary>
[CreateAssetMenu(fileName = "NewSubtitle", menuName = "Subtitle System/Subtitle Data")]
public class SubtitleData : ScriptableObject
{
    [Header("Subtitle Content")]
    [TextArea(3, 10)]
    [Tooltip("Texte du sous-titre")]
    public string Text;

    [Header("Display Settings")]
    [Tooltip("Durée d'affichage en secondes (0 = calcul automatique basé sur la longueur)")]
    public float Duration = 0f;

    [Tooltip("Calculer automatiquement la durée (caractères par seconde)")]
    public bool AutoCalculateDuration = true;

    [Tooltip("Vitesse de lecture en caractères par seconde")]
    [Range(10f, 50f)]
    public float ReadingSpeed = 20f;

    [Header("Audio (Optional)")]
    [Tooltip("Clip audio à jouer avec ce sous-titre")]
    public AudioClip VoiceClip;

    [Header("Speaker Info (Optional)")]
    [Tooltip("Nom du personnage qui parle")]
    public string SpeakerName;

    [Tooltip("Couleur du texte pour ce personnage")]
    public Color TextColor = Color.white;

    /// <summary>
    /// Retourne la durée effective du sous-titre
    /// </summary>
    public float GetEffectiveDuration()
    {
        if (!AutoCalculateDuration && Duration > 0f)
            return Duration;

        // Calcul automatique basé sur la longueur du texte
        if (string.IsNullOrEmpty(Text))
            return 2f; // Durée minimale

        float calculatedDuration = Text.Length / ReadingSpeed;
        return Mathf.Max(2f, calculatedDuration); // Minimum 2 secondes
    }
}