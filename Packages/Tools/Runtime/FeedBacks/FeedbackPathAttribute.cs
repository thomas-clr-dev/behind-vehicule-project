using System;
using System.Reflection;

/// <summary>
/// Déclare le chemin du feedback dans le menu "Add New Feedback".
/// Exemple : [FeedbackPath("Audio/Music Layer")]
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class FeedbackPathAttribute : Attribute
{
    public string Path { get; }

    public FeedbackPathAttribute(string path)
    {
        Path = path;
    }

    /// <summary>
    /// Retourne le chemin déclaré sur un type, ou null si absent.
    /// </summary>
    public static string GetPath(Type type)
    {
        FeedbackPathAttribute attr = type.GetCustomAttribute(typeof(FeedbackPathAttribute)) as FeedbackPathAttribute;
        return attr?.Path;
    }

    /// <summary>
    /// Retourne le nom court (dernière partie du chemin).
    /// Ex: "Audio/Music Layer" -> "Music Layer"
    /// </summary>
    public static string GetName(Type type)
    {
        string path = GetPath(type);
        if (path == null) return null;
        int lastSlash = path.LastIndexOf('/');
        return lastSlash >= 0 ? path.Substring(lastSlash + 1) : path;
    }
}