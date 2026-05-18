/// <summary>
/// Événement déclenché pour afficher un sous-titre
/// </summary>
public struct SubtitleShowEvent : IEvent
{
    public SubtitleData SubtitleData;
}

/// <summary>
/// Événement déclenché pour cacher les sous-titres
/// </summary>
public struct SubtitleHideEvent : IEvent
{
}

/// <summary>
/// Événement déclenché quand un sous-titre est terminé
/// </summary>
public struct SubtitleCompletedEvent : IEvent
{
    public SubtitleData CompletedSubtitle;
}